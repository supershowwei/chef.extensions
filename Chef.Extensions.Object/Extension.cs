using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Chef.Extensions.Object
{
    public static class Extension
    {
        private static readonly HashSet<System.Type> NumericTypes = new HashSet<System.Type>
                                                                    {
                                                                        typeof(byte),
                                                                        typeof(sbyte),
                                                                        typeof(short),
                                                                        typeof(ushort),
                                                                        typeof(int),
                                                                        typeof(uint),
                                                                        typeof(long),
                                                                        typeof(ulong),
                                                                        typeof(float),
                                                                        typeof(double),
                                                                        typeof(decimal),
                                                                    };

        private static readonly ConcurrentDictionary<string, Delegate> ObjectConverter = new ConcurrentDictionary<string, Delegate>();

        public static bool IsNotNull(this object me)
        {
            return me != null;
        }

        public static T? ToNullable<T>(this object me)
            where T : struct, IConvertible
        {
            if (me == null) return default(T?);

            return (T?)Convert.ChangeType(me, typeof(T));
        }

        public static ExpandoObject ToExpando(this object me)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (var property in me.GetType().GetProperties())
            {
                expando.Add(property.Name, property.GetValue(me));
            }

            return (ExpandoObject)expando;
        }

        public static bool IsNumeric(this object me)
        {
            if (me == null) return false;

            return NumericTypes.Contains(me.GetType());
        }

        public static bool Exists<Ta, Tb>(this Ta me, IEnumerable<Tb> collection, Func<Ta, Tb, bool> predicate)
        {
            return collection.Any(item => predicate(me, item));
        }

        public static bool NotExists<Ta, Tb>(this Ta me, IEnumerable<Tb> collection, Func<Ta, Tb, bool> predicate)
        {
            return collection.All(item => !predicate(me, item));
        }

        public static TTarget To<TTarget>(this object me)
        {
            if (me == null) return default(TTarget);

            if (me is IConvertible) return (TTarget)Convert.ChangeType(me, typeof(TTarget));

            var sourceType = me.GetType();
            var targetType = typeof(TTarget);

            var converterKey = string.Concat(sourceType.FullName, " -> ", targetType.FullName);

            var converter = (Func<object, TTarget>)ObjectConverter.GetOrAdd(
                converterKey,
                key =>
                    {
                        var sourceProperties = sourceType.GetProperties().ToDictionary(p => p.Name, p => p);
                        var targetProperties = targetType.GetProperties().ToDictionary(p => p.Name, p => p);

                        var sourceParam = Expression.Parameter(typeof(object), "obj");
                        var sourceVariable = Expression.Variable(sourceType, "source");

                        var sourceAssign = Expression.Assign(sourceVariable, Expression.Convert(sourceParam, sourceType));

                        var memberBindings = targetProperties.Where(
                                p =>
                                    {
                                        if (!sourceProperties.ContainsKey(p.Key)) return false;
                                        if (p.Value.PropertyType != sourceProperties[p.Key].PropertyType) return false;

                                        return true;
                                    })
                            .Select(p => Expression.Bind(p.Value, Expression.Property(sourceVariable, sourceProperties[p.Key])));

                        var memberInit = Expression.MemberInit(Expression.New(targetType), memberBindings);

                        var returnLabel = Expression.Label(targetType);

                        var block = Expression.Block(
                            new[] { sourceVariable },
                            sourceAssign,
                            Expression.Return(returnLabel, memberInit),
                            Expression.Label(returnLabel, Expression.Default(targetType)));

                        return Expression.Lambda(block, sourceParam).Compile();
                    });

            return converter(me);
        }

        public static TTarget To<TTarget>(this object me, TTarget existed)
        {
            if (me == null) return default(TTarget);

            var sourceType = me.GetType();
            var targetType = typeof(TTarget);

            var converterKey = string.Concat(sourceType.FullName, " -> (Existed)", targetType.FullName);

            var converter = (Func<object, TTarget, TTarget>)ObjectConverter.GetOrAdd(
                converterKey,
                key =>
                    {
                        var sourceProperties = sourceType.GetProperties().ToDictionary(p => p.Name, p => p);
                        var targetProperties = targetType.GetProperties().ToDictionary(p => p.Name, p => p);

                        var sourceParam = Expression.Parameter(typeof(object), "obj");
                        var targetParam = Expression.Parameter(targetType, "target");
                        var sourceVariable = Expression.Variable(sourceType, "source");

                        var expressions = new List<Expression>();

                        expressions.Add(Expression.Assign(sourceVariable, Expression.Convert(sourceParam, sourceType)));

                        foreach (var targetProperty in targetProperties)
                        {
                            if (!sourceProperties.ContainsKey(targetProperty.Key)) continue;
                            if (targetProperty.Value.PropertyType != sourceProperties[targetProperty.Key].PropertyType) continue;

                            var targetPropertyExpr = Expression.Property(targetParam, targetProperty.Value.Name);

                            Expression conditionAssign = Expression.Condition(
                                Expression.Equal(targetPropertyExpr, Expression.Default(targetProperty.Value.PropertyType)),
                                Expression.Property(sourceVariable, sourceProperties[targetProperty.Key]),
                                targetPropertyExpr);

                            expressions.Add(Expression.Assign(targetPropertyExpr, conditionAssign));
                        }

                        var returnLabel = Expression.Label(targetType);

                        expressions.Add(Expression.Return(returnLabel, targetParam));
                        expressions.Add(Expression.Label(returnLabel, Expression.Default(targetType)));

                        var block = Expression.Block(new[] { sourceVariable }, expressions);

                        return Expression.Lambda(block, sourceParam, targetParam).Compile();
                    });

            return converter(me, existed);
        }

        public static TTarget To<TSource, TTarget>(this TSource me, Func<TSource, TTarget> convert)
        {
            if (me == null) return default(TTarget);

            return convert(me);
        }

        public static Boole<T> Is<T>(this T me, Func<T, bool> booleFunc)
        {
            return new Boole<T>(me, booleFunc(me));
        }

        public static Boole<T> Is<T>(this T me, bool booleValue)
        {
            return new Boole<T>(me, booleValue);
        }

        public static Boole<T> And<T>(this Boole<T> me, Func<T, bool> booleFunc)
        {
            return !me.Value ? me : new Boole<T>(me.Logic, me.Value && booleFunc(me.Logic));
        }

        public static Boole<T> And<T>(this Boole<T> me, bool booleValue)
        {
            return !me.Value ? me : new Boole<T>(me.Logic, me.Value && booleValue);
        }

        public static Boole<T> Or<T>(this Boole<T> me, Func<T, bool> booleFunc)
        {
            return me.Value ? me : new Boole<T>(me.Logic, me.Value || booleFunc(me.Logic));
        }

        public static Boole<T> Or<T>(this Boole<T> me, bool booleValue)
        {
            return me.Value ? me : new Boole<T>(me.Logic, me.Value || booleValue);
        }
    }
}