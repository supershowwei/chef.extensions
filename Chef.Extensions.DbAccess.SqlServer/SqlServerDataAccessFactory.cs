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

        private static readonly ConcurrentDictionary<string, IEnumerable<DataColumn>> UserDefinedTables = new ConcurrentDictionary<string, IEnumerable<DataColumn>>();

        private SqlServerDataAccessFactory()
        {
        }

        public static SqlServerDataAccessFactory Instance => Lazy.Value;

        public IDataAccess<T> Create<T>()
        {
            var connectionStringAttributes = typeof(T).GetCustomAttributes<ConnectionStringAttribute>(true);

            if (connectionStringAttributes.Any() && connectionStringAttributes.Count() > 1)
            {
                throw new ArgumentException("Must indicate connection string.");
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
            var connectionStringAttribute = typeof(T).GetCustomAttributes<ConnectionStringAttribute>(true).SingleOrDefault(x => x.ConnectionString == nameOrConnectionString);

            if (connectionStringAttribute != null)
            {
                var connectionString = ConnectionStrings.ContainsKey(connectionStringAttribute.ConnectionString)
                                           ? ConnectionStrings[connectionStringAttribute.ConnectionString]
                                           : connectionStringAttribute.ConnectionString;

                return new SqlServerDataAccess<T>(connectionString);
            }

            return new SqlServerDataAccess<T>(nameOrConnectionString);
        }

        public void AddConnectionString(string name, string value)
        {
            ConnectionStrings.TryAdd(name, value);
        }

        public void AddUserDefinedTable(string name, IDictionary<string, Type> columns)
        {
            UserDefinedTables.TryAdd(name, columns.Select(x => new DataColumn(x.Key, x.Value)));
        }

        public IEnumerable<DataColumn> GetUserDefinedTable(string name)
        {
            return UserDefinedTables.ContainsKey(name) ? UserDefinedTables[name] : null;
        }

        internal string GetConnectionString(string nameOrConnectionString)
        {
            return ConnectionStrings.ContainsKey(nameOrConnectionString)
                       ? ConnectionStrings[nameOrConnectionString]
                       : nameOrConnectionString;
        }
    }
}