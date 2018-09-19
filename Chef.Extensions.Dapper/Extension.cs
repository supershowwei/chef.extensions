using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Chef.Extensions.Dapper
{
    public static class Extension
    {
        private static readonly Dictionary<string, object> RowParsers = new Dictionary<string, object>();

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
                    var parser = TryGetRowParser<T>(discriminator, reader);

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
            var result = new List<T>();

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = TryGetRowParser<T>(discriminator, reader);

                    result.Add(parser(reader));
                }
            }

            return result.Single();
        }

        private static Func<IDataReader, T> TryGetRowParser<T>(string discriminator, IDataReader reader)
        {
            var baseType = typeof(T);
            var concrete = reader.GetString(reader.GetOrdinal(discriminator));
            var key = $"{concrete} : {baseType.Name}";

            if (RowParsers.ContainsKey(key)) return (Func<IDataReader, T>)RowParsers[key];

            var concreteType = FindConcreteType(concrete, baseType);

            lock (RowParsers)
            {
                if (!RowParsers.ContainsKey(key))
                {
                    RowParsers[key] = reader.GetRowParser<T>(concreteType);
                }
            }

            return (Func<IDataReader, T>)RowParsers[key];
        }

        private static Type FindConcreteType(string typeName, Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                .GetTypes()
                .Single(t => t.IsSubclassOf(baseType) && t.Name.Equals(typeName));
        }
    }
}