using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, object> RowParsers = new ConcurrentDictionary<string, object>();
        private static readonly MD5 MD5 = MD5.Create();

        public Func<IDataReader, T> GetRowParser<T>(string discriminator, IDataReader reader, string sql)
        {
            var baseType = typeof(T);
            var concrete = reader.GetString(reader.GetOrdinal(discriminator));
            var key = Hash($"{concrete}_{baseType.Name}_{sql}");

            return (Func<IDataReader, T>)RowParsers.GetOrAdd(key, k => reader.GetRowParser<T>(FindConcreteType(concrete, baseType)));
        }

        private static string Hash(string value)
        {
            return BitConverter.ToString(MD5.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }

        private static Type FindConcreteType(string typeName, Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                .GetTypes()
                .Single(t => t.IsSubclassOf(baseType) && t.Name.Equals(typeName));
        }
    }
}