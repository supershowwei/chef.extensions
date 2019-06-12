using System;
using System.Linq;
using System.Linq.Expressions;

namespace Chef.Extensions.LiteDB.Extensions
{
    internal delegate object ObjectActivator(params object[] args);

    internal static class TypeExtension
    {
        public static ObjectActivator GetActivator(this Type me)
        {
            var ctor = me.GetConstructors().First();
            var parameterInfos = ctor.GetParameters();

            var param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = new Expression[parameterInfos.Length];

            for (var i = 0; i < parameterInfos.Length; i++)
            {
                Expression index = Expression.Constant(i);
                var paramType = parameterInfos[i].ParameterType;

                Expression paramAccessorExp = Expression.ArrayIndex(param, index);

                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            var newExp = Expression.New(ctor, argsExp);

            var lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);

            return (ObjectActivator)lambda.Compile();
        }
    }
}