using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Chef.Extensions.Type
{
    public delegate T ObjectActivator<T>(params object[] args);

    public static class ObjectActivatorBuilder
    {
        public static ObjectActivator<T> Build<T>()
        {
            var ctor = typeof(T).GetConstructors()[0];

            return GenerateObjectActivator<T>(ctor);
        }

        public static ObjectActivator<T> Build<T>(int index)
        {
            var ctor = typeof(T).GetConstructors()[index];

            return GenerateObjectActivator<T>(ctor);
        }

        public static ObjectActivator<T> Build<T>(System.Type attributeType)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute))) return Build<T>(new[] { attributeType });

            var ctor = typeof(T).GetConstructors().First(c => c.CustomAttributes.Any(a => a.AttributeType == attributeType));

            return GenerateObjectActivator<T>(ctor);
        }

        public static ObjectActivator<T> Build<T>(params System.Type[] parameterTypes)
        {
            return GenerateObjectActivator<T>(typeof(T).GetConstructor(parameterTypes));
        }

        private static ObjectActivator<T> GenerateObjectActivator<T>(ConstructorInfo ctor)
        {
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

            var lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            return (ObjectActivator<T>)lambda.Compile();
        }
    }
}