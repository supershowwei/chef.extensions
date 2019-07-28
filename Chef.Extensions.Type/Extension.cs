using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
            var ctor = me.GetConstructors()[0];

            return GenerateObjectActivator(ctor);
        }

        public static ObjectActivator GetActivator(this System.Type me, int index)
        {
            var ctor = me.GetConstructors()[index];
            
            return GenerateObjectActivator(ctor);
        }

        public static ObjectActivator GetActivator(this System.Type me, System.Type attributeType)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute))) return GetActivator(me, new[] { attributeType });

            var ctor = me.GetConstructors().First(c => c.CustomAttributes.Any(a => a.AttributeType == attributeType));

            return GenerateObjectActivator(ctor);
        }

        public static ObjectActivator GetActivator(this System.Type me, params System.Type[] parameterTypes)
        {
            return GenerateObjectActivator(me.GetConstructor(parameterTypes));
        }

        private static ObjectActivator GenerateObjectActivator(ConstructorInfo ctor)
        {
            var param = Expression.Parameter(typeof(object[]), "args");

            var argsExp = ctor.GetParameters()
                .Select((p, i) => (Expression)Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), p.ParameterType));

            var newExp = Expression.New(ctor, argsExp);

            var lambda = Expression.Lambda(typeof(ObjectActivator), newExp, param);

            return (ObjectActivator)lambda.Compile();
        }
    }
}