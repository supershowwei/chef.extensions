using System;
using System.Collections.Generic;
using System.Reflection;
using Dapper;

namespace Chef.Extensions.Dapper
{
    internal class TypeHandlers
    {
        private static readonly Lazy<Dictionary<Type, SqlMapper.ITypeHandler>> Lazy =
            new Lazy<Dictionary<Type, SqlMapper.ITypeHandler>>(
                () =>
                    {
                        var typeHandlersField = typeof(SqlMapper).GetField(
                            "typeHandlers",
                            BindingFlags.NonPublic | BindingFlags.Static);

                        return typeHandlersField == null
                                   ? new Dictionary<Type, SqlMapper.ITypeHandler>()
                                   : (Dictionary<Type, SqlMapper.ITypeHandler>)typeHandlersField.GetValue(null);
                    });

        private TypeHandlers()
        {
        }

        public static Dictionary<Type, SqlMapper.ITypeHandler> Instance => Lazy.Value;
    }
}