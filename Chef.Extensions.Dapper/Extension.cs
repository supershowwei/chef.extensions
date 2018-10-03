using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;

namespace Chef.Extensions.Dapper
{
    public static class Extension
    {
        private static readonly IRowParserProvider DefaultRowParserProvider = new DefaultRowParserProvider();
        private static IRowParserProvider userDefinedRowParserProvider;

        public static IRowParserProvider RowParserProvider
        {
            private get { return userDefinedRowParserProvider ?? DefaultRowParserProvider; }
            set { userDefinedRowParserProvider = value; }
        }

        public static IEnumerable<T> PolymorphicQuery<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = new List<T>();

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader);

                    result.Add(parser(reader));
                }
            }

            return result;
        }

        public static T PolymorphicQuerySingle<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);
            var count = 0;

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader);

                    result = parser(reader);
                }
            }

            if (count == 0) throw new InvalidOperationException("Sequence contains no elements.");

            return result;
        }

        public static T PolymorphicQuerySingleOrDefault<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                var count = 0;
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader);

                    result = parser(reader);
                }
            }

            return result;
        }

        public static T PolymorphicQueryFirst<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader);

                    return parser(reader);
                }
            }

            throw new InvalidOperationException("Sequence contains no elements.");
        }

        public static T PolymorphicQueryFirstOrDefault<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader);

                    result = parser(reader);

                    break;
                }
            }

            return result;
        }

        public static void PolymorphicInsert(this IDbConnection cnn, string sql, object param)
        {
            if (param == null) throw new ArgumentException($"'{nameof(param)}' is null.");

            var props = Regex.Matches(sql, "@([^\\s,)]+)").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

            if (props.Count == 0) throw new ArgumentException($"'{nameof(sql)}' has no parameters.");

            if (param is IEnumerable objs)
            {
                var parameters = new List<object>();

                var any = false;
                foreach (var obj in objs)
                {
                    any = true;

                    parameters.Add(GetParameter(props, obj));
                }

                if (!any) throw new ArgumentException($"'{nameof(param)}' is empty.");

                cnn.Execute(sql, parameters);
            }
            else
            {
                cnn.Execute(sql, GetParameter(props, param));
            }
        }

        private static ExpandoObject GetParameter(List<string> props, object obj)
        {
            var expando = (IDictionary<string, object>)new ExpandoObject();

            foreach (var prop in props)
            {
                expando[prop] = GetObjValue(prop, obj);
            }

            return (ExpandoObject)expando;
        }

        private static object GetObjValue(string propName, object obj)
        {
            var objType = obj.GetType();

            int underscoreIndex;
            if ((underscoreIndex = propName.IndexOf('_')) < 0)
            {
                var objProp = objType.GetProperty(propName);

                return objProp == null ? null : objProp.GetValue(obj);
            }
            else
            {
                var objProp = objType.GetProperty(propName.Substring(0, underscoreIndex));

                return objProp == null
                           ? null
                           : GetObjValue(propName.Substring(underscoreIndex + 1), objProp.GetValue(obj));
            }
        }
    }
}