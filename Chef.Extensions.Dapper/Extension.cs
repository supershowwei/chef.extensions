using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chef.Extensions.Dapper.Extensions;
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
            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    yield return parser(reader);
                }
            }
        }

        public static async Task<IEnumerable<T>> PolymorphicQueryAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var reader = await cnn.ExecuteReaderAsync(sql, param);

            return reader.PolymorphicExecuteReaderSync<T>(sql, discriminator);
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

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);
                }
            }

            if (count == 0) throw new InvalidOperationException("Sequence contains no elements.");

            return result;
        }

        public static async Task<T> PolymorphicQuerySingleAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);
            var count = 0;

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

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

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);
                }
            }

            return result;
        }

        public static async Task<T> PolymorphicQuerySingleOrDefaultAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                var count = 0;
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

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
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    return parser(reader);
                }
            }

            throw new InvalidOperationException("Sequence contains no elements.");
        }

        public static async Task<T> PolymorphicQueryFirstAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

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
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);

                    break;
                }
            }

            return result;
        }

        public static async Task<T> PolymorphicQueryFirstOrDefaultAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);

                    break;
                }
            }

            return result;
        }

        public static void PolymorphicInsert(this IDbConnection cnn, string sql, object param)
        {
            HierarchyInsert(cnn, sql, param);
        }

        public static Task PolymorphicInsertAsync(this IDbConnection cnn, string sql, object param)
        {
            return HierarchyInsertAsync(cnn, sql, param);
        }

        public static void HierarchyInsert(this IDbConnection cnn, string sql, object param)
        {
            if (param == null) throw new ArgumentException($"'{nameof(param)}' is null.");

            var props = Regex.Matches(sql, "@([^\\s,)]+)").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

            if (props.Count == 0) throw new ArgumentException($"'{nameof(sql)}' has no parameters.");

            if (param is IEnumerable objs)
            {
                var parameters = new List<object>();

                foreach (var obj in objs)
                {
                    parameters.Add(GetParameter(props, obj));
                }

                if (!parameters.Any()) throw new ArgumentException($"'{nameof(param)}' is empty.");

                cnn.Execute(sql, parameters);
            }
            else
            {
                cnn.Execute(sql, GetParameter(props, param));
            }
        }

        public static Task HierarchyInsertAsync(this IDbConnection cnn, string sql, object param)
        {
            if (param == null) throw new ArgumentException($"'{nameof(param)}' is null.");

            var props = Regex.Matches(sql, "@([^\\s,)]+)").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

            if (props.Count == 0) throw new ArgumentException($"'{nameof(sql)}' has no parameters.");

            if (param is IEnumerable objs)
            {
                var parameters = new List<object>();

                foreach (var obj in objs)
                {
                    parameters.Add(GetParameter(props, obj));
                }

                if (!parameters.Any()) throw new ArgumentException($"'{nameof(param)}' is empty.");

                return cnn.ExecuteAsync(sql, parameters);
            }
            else
            {
                return cnn.ExecuteAsync(sql, GetParameter(props, param));
            }
        }

        public static IEnumerable<T> QueryAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    yield return (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }
        }

        public static async Task<IEnumerable<T>> QueryAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var reader = await cnn.ExecuteReaderAsync(sql, param);

            return reader.ExecuteReaderAsImmutabilitySync<T>(sql);
        }

        public static T QuerySingleAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);
            var count = 0;

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            if (count == 0) throw new InvalidOperationException("Sequence contains no elements.");

            return result;
        }

        public static async Task<T> QuerySingleAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);
            var count = 0;

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            if (count == 0) throw new InvalidOperationException("Sequence contains no elements.");

            return result;
        }

        public static T QuerySingleOrDefaultAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                var count = 0;
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            return result;
        }

        public static async Task<T> QuerySingleOrDefaultAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                var count = 0;
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            return result;
        }

        public static T QueryFirstAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    return (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            throw new InvalidOperationException("Sequence contains no elements.");
        }

        public static async Task<T> QueryFirstAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    return (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            throw new InvalidOperationException("Sequence contains no elements.");
        }

        public static T QueryFirstOrDefaultAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));

                    break;
                }
            }

            return result;
        }

        public static async Task<T> QueryFirstOrDefaultAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));

                    break;
                }
            }

            return result;
        }

        /// <summary>
        ///     Length of the string is default 4000
        /// </summary>
        public static DbString ToVarchar(this string me)
        {
            return new DbString { Value = me, IsAnsi = true };
        }

        /// <summary>
        ///     Length of the string -1 for max
        /// </summary>
        public static DbString ToVarchar(this string me, int length)
        {
            return new DbString { Value = me, Length = length, IsAnsi = true };
        }

        /// <summary>
        ///     Length of the string is default 4000
        /// </summary>
        public static DbString ToChar(this string me)
        {
            return new DbString { Value = me, IsAnsi = true, IsFixedLength = true };
        }

        /// <summary>
        ///     Length of the string -1 for max
        /// </summary>
        public static DbString ToChar(this string me, int length)
        {
            return new DbString { Value = me, Length = length, IsAnsi = true, IsFixedLength = true };
        }

        /// <summary>
        ///     Length of the string is default 4000
        /// </summary>
        public static DbString ToNVarchar(this string me)
        {
            return new DbString { Value = me };
        }

        /// <summary>
        ///     Length of the string -1 for max
        /// </summary>
        public static DbString ToNVarchar(this string me, int length)
        {
            return new DbString { Value = me, Length = length };
        }

        /// <summary>
        ///     Length of the string is default 4000
        /// </summary>
        public static DbString ToNChar(this string me)
        {
            return new DbString { Value = me, IsFixedLength = true };
        }

        /// <summary>
        ///     Length of the string -1 for max
        /// </summary>
        public static DbString ToNChar(this string me, int length)
        {
            return new DbString { Value = me, Length = length, IsFixedLength = true };
        }

        private static IEnumerable<T> PolymorphicExecuteReaderSync<T>(this IDataReader me, string sql, string discriminator)
        {
            using (me)
            {
                while (me.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, me, sql);

                    yield return parser(me);
                }
            }
        }

        private static IEnumerable<T> ExecuteReaderAsImmutabilitySync<T>(this IDataReader me, string sql)
        {
            using (me)
            {
                while (me.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    yield return (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(me));
                }
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