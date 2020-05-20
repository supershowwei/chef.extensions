using System;

namespace Chef.Extensions.DbAccess
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ConnectionStringAttribute : Attribute
    {
        public ConnectionStringAttribute(string nameOrConnectionString)
        {
            this.ConnectionString = nameOrConnectionString;
        }

        public string ConnectionString { get; }
    }
}