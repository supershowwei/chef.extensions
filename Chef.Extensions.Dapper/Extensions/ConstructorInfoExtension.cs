using System.Data;
using System.Linq;
using System.Reflection;

namespace Chef.Extensions.Dapper.Extensions
{
    internal static class ConstructorInfoExtension
    {
        public static object[] GetArguments(this ConstructorInfo me, IDataReader reader)
        {
            return me.GetParameters()
                .Select(
                    p =>
                        {
                            if (reader.HasField(p.Name))
                            {
                                return TypeHandlers.Instance.TryGetValue(p.ParameterType, out var typeHandler)
                                           ? typeHandler.Parse(p.ParameterType, reader[p.Name])
                                           : reader[p.Name];
                            }

                            return null;
                        })
                .ToArray();
        }
    }
}