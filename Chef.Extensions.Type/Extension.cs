﻿using System.Linq;
using System.Linq.Expressions;

namespace Chef.Extensions.Type
{
    public delegate object ObjectActivator(params object[] args);

    public static class Extension
    {
        public static bool IsUserDefined(this System.Type me)
        {
            return !me.IsValueType && !me.IsPrimitive && (me.Namespace == null || !me.Namespace.StartsWith("System"));
        }

        public static string[] GetPropertyNames(this System.Type me)
        {
            return me.GetProperties().Select(x => x.Name).ToArray();
        }

        public static string[] GetPropertyNames(this System.Type me, string prefix)
        {
            return me.GetProperties().Select(x => string.Concat(prefix, x.Name)).ToArray();
        }

        public static ObjectActivator GetActivator(this System.Type me)
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