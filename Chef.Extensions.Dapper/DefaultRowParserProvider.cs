using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Dapper;

namespace Chef.Extensions.Dapper
{
    internal class DefaultRowParserProvider : IRowParserProvider
    {
        private static readonly Dictionary<string, object> RowParsers = new Dictionary<string, object>();

        public Func<IDataReader, T> GetRowParser<T>(string discriminator, IDataReader reader, string sql)
        {
            var baseType = typeof(T);
            var concrete = reader.GetString(reader.GetOrdinal(discriminator));
            var key = Hash($"{concrete}_{baseType.Name}_{sql}");

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

        private static string Hash(string value)
        {
            using (var md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(value)));
            }
        }

        private static Type FindConcreteType(string typeName, Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                .GetTypes()
                .Single(t => t.IsSubclassOf(baseType) && t.Name.Equals(typeName));
        }
    }
}