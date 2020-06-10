using System;

namespace Chef.Extensions.DbAccess.SqlServer
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class UserDefinedTableAttribute : Attribute
    {
        public UserDefinedTableAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}