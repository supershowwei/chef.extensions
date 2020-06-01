using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Chef.Extensions.DbAccess.SqlServer
{
    public class SqlServerDataAccessFactory : IDataAccessFactory
    {
        private static readonly Lazy<SqlServerDataAccessFactory> Lazy = new Lazy<SqlServerDataAccessFactory>(() => new SqlServerDataAccessFactory());

        private static readonly ConcurrentDictionary<string, string> ConnectionStrings = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<Type, (string, IEnumerable<DataColumn>)> UserDefinedTables = new ConcurrentDictionary<Type, (string, IEnumerable<DataColumn>)>();

        private SqlServerDataAccessFactory()
        {
        }

        public static SqlServerDataAccessFactory Instance => Lazy.Value;

        public IDataAccess<T> Create<T>()
        {
            var connectionStringAttributes = typeof(T).GetCustomAttributes<ConnectionStringAttribute>(true);

            if (connectionStringAttributes.Any() && connectionStringAttributes.Count() > 1)
            {
                throw new ArgumentException("Must indicate named connection string.");
            }

            var connectionStringAttribute = connectionStringAttributes.SingleOrDefault();

            if (connectionStringAttribute == null)
            {
                throw new ArgumentException("Must add connection string.");
            }

            var connectionString = ConnectionStrings.ContainsKey(connectionStringAttribute.ConnectionString)
                                       ? ConnectionStrings[connectionStringAttribute.ConnectionString]
                                       : connectionStringAttribute.ConnectionString;

            return new SqlServerDataAccess<T>(connectionString);
        }

        public IDataAccess<T> Create<T>(string nameOrConnectionString)
        {
            var connectionStringAttributes = typeof(T).GetCustomAttributes<ConnectionStringAttribute>(true);

            var connectionStringAttribute = connectionStringAttributes.SingleOrDefault(x => x.ConnectionString == nameOrConnectionString);

            if (connectionStringAttribute == null)
            {
                throw new ArgumentException("Must add connection string.");
            }

            var connectionString = ConnectionStrings.ContainsKey(connectionStringAttribute.ConnectionString)
                                       ? ConnectionStrings[connectionStringAttribute.ConnectionString]
                                       : connectionStringAttribute.ConnectionString;

            return new SqlServerDataAccess<T>(connectionString);
        }

        public void AddConnectionString(string name, string value)
        {
            ConnectionStrings.TryAdd(name, value);
        }

        public void AddUserDefinedTable<T>(string type, IDictionary<string, Type> columns)
        {
            UserDefinedTables.TryAdd(typeof(T), (type, columns.Select(x => new DataColumn(x.Key, x.Value))));
        }

        public (string, IEnumerable<DataColumn>) GetUserDefinedTable<T>()
        {
            return UserDefinedTables.ContainsKey(typeof(T)) ? UserDefinedTables[typeof(T)] : (null, null);
        }

        internal string GetConnectionString(string nameOrConnectionString)
        {
            return ConnectionStrings.ContainsKey(nameOrConnectionString)
                       ? ConnectionStrings[nameOrConnectionString]
                       : nameOrConnectionString;
        }
    }
}