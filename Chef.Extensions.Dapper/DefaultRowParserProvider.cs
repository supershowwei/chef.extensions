using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;

namespace Chef.Extensions.Dapper
{
    internal class DefaultRowParserProvider : IRowParserProvider
    {
        private static readonly Dictionary<string, object> RowParsers = new Dictionary<string, object>();

        public Func<IDataReader, T> GetRowParser<T>(string discriminator, IDataReader reader)
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

        private static System.Type FindConcreteType(string typeName, System.Type baseType)
        {
            return Assembly.GetAssembly(baseType)
                .GetTypes()
                .Single(t => t.IsSubclassOf(baseType) && t.Name.Equals(typeName));
        }
    }
}