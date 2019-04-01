using System;
using System.Data;

namespace Chef.Extensions.Dapper.Extensions
{
    internal static class IDataReaderExtension
    {
        public static bool HasField(this IDataReader me, string name)
        {
            for (var i = 0; i < me.FieldCount; i++)
            {
                if (me.GetName(i).Equals(name, StringComparison.OrdinalIgnoreCase)) return true;
            }

            return false;
        }
    }
}