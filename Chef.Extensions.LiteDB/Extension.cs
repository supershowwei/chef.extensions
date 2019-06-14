using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Chef.Extensions.LiteDB.Extensions;
using LiteDB;

namespace Chef.Extensions.LiteDB
{
    public static class Extension
    {
        private static readonly Type BsonIdAttr = typeof(BsonIdAttribute);

        private static readonly Func<Type, BsonValue, object> Deserialize = CreateDeserializationDelegate();

        private static readonly Dictionary<Type, ParameterInfo[]> ConstructorParameters = new Dictionary<Type, ParameterInfo[]>();

        private static readonly Dictionary<Type, PropertyInfo> IdentityProps = new Dictionary<Type, PropertyInfo>();

        private static readonly Dictionary<Type, ObjectActivator> ObjectActivators = new Dictionary<Type, ObjectActivator>();

        private static readonly Dictionary<Type, FieldInfo> EngineFields = new Dictionary<Type, FieldInfo>();

        private static readonly Dictionary<Type, FieldInfo> IncludingFields = new Dictionary<Type, FieldInfo>();

        private static readonly Dictionary<Type, FieldInfo> VisitorFields = new Dictionary<Type, FieldInfo>();

        private static readonly Dictionary<Type, MethodInfo> VisitMethods = new Dictionary<Type, MethodInfo>();

        private static readonly Dictionary<Type, Type[]> KeyValueTypes = new Dictionary<Type, Type[]>();

        public static T ToImmutability<T>(this BsonDocument me)
        {
            if (me == null) return default(T);

            var type = typeof(T);

            var constructorParams = ConstructorParameters.SafeGetOrAdd(type, () => type.GetConstructors().First().GetParameters());

            var identityProp = IdentityProps.SafeGetOrAdd(type, () => GetIdentityProperty(type));

            var args = constructorParams.Select(p => GetArgument(p, identityProp.Name, me)).ToArray();

            return (T)ObjectActivators.SafeGetOrAdd(type, () => type.GetActivator())(args);
        }

        public static BsonDocument ToDocument<T>(this T me)
        {
            return BsonMapper.Global.ToDocument(me);
        }

        public static IEnumerable<T> FindAllAsImmutability<T>(this LiteCollection<T> me)
        {
            return FindAsImmutability(me, Query.All());
        }

        public static IEnumerable<T> FindAsImmutability<T>(
            this LiteCollection<T> me,
            Query query,
            int skip = 0,
            int limit = int.MaxValue)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var type = typeof(T);

            var engine = (LazyLoad<LiteEngine>)EngineFields.SafeGetOrAdd(
                    type,
                    () => me.GetType().GetField("_engine", BindingFlags.Instance | BindingFlags.NonPublic))
                .GetValue(me);

            var includes = (List<string>)IncludingFields.SafeGetOrAdd(
                    type,
                    () => me.GetType().GetField("_includes", BindingFlags.Instance | BindingFlags.NonPublic))
                .GetValue(me);

            var docs = engine.Value.Find(me.Name, query, includes.ToArray(), skip, limit);

            foreach (var doc in docs)
            {
                yield return ToImmutability<T>(doc);
            }
        }

        public static IEnumerable<T> FindAsImmutability<T>(
            this LiteCollection<T> me,
            Expression<Func<T, bool>> predicate,
            int skip = 0,
            int limit = int.MaxValue)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var type = typeof(T);

            var visitor = VisitorFields.SafeGetOrAdd(
                    type,
                    () => me.GetType().GetField("_visitor", BindingFlags.Instance | BindingFlags.NonPublic))
                .GetValue(me);

            var visit = VisitMethods.SafeGetOrAdd(type, () => visitor.GetType().GetMethod("Visit"));

            return FindAsImmutability(me, (Query)visit.Invoke(visitor, new object[] { predicate }), skip, limit);
        }

        public static T FindAsImmutabilityById<T>(this LiteCollection<T> me, BsonValue id)
        {
            if (id == null || id.IsNull) throw new ArgumentNullException(nameof(id));

            return FindAsImmutability(me, Query.EQ("_id", id)).SingleOrDefault();
        }

        public static T FindOneAsImmutability<T>(this LiteCollection<T> me, Query query)
        {
            return FindAsImmutability(me, query).FirstOrDefault();
        }

        public static T FindOneAsImmutability<T>(this LiteCollection<T> me, Expression<Func<T, bool>> predicate)
        {
            return FindAsImmutability(me, predicate).FirstOrDefault();
        }

        private static Func<Type, BsonValue, object> CreateDeserializationDelegate()
        {
            var deserialize = typeof(BsonMapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Single(
                    x =>
                        {
                            if (x.IsGenericMethod) return false;
                            if (!x.Name.Equals("Deserialize")) return false;

                            var parameters = x.GetParameters();

                            if (parameters.Length != 2) return false;
                            if (parameters[0].ParameterType != typeof(Type)) return false;
                            if (parameters[1].ParameterType != typeof(BsonValue)) return false;

                            return true;
                        });

            var delg = Delegate.CreateDelegate(typeof(Func<Type, BsonValue, object>), BsonMapper.Global, deserialize);

            return (Func<Type, BsonValue, object>)delg;
        }

        private static PropertyInfo GetIdentityProperty(Type type)
        {
            var prop = type.GetProperties().SingleOrDefault(p => p.CustomAttributes.Any(a => a.AttributeType == BsonIdAttr));

            if (prop == null) prop = type.GetProperty("Id");

            return prop;
        }

        private static object GetArgument(ParameterInfo param, string identityName, BsonDocument doc)
        {
            if (identityName.Equals(param.Name, StringComparison.OrdinalIgnoreCase))
            {
                return Deserialize(param.ParameterType, doc["_id"]);
            }
            else if (doc.Keys.Any(k => k.Equals(param.Name, StringComparison.OrdinalIgnoreCase), out var key))
            {
                var value = doc[key];

                if (IsList(value, param))
                {
                    return DeserializeList(param.ParameterType, value.AsArray);
                }
                else if (IsDictionary(value, param))
                {
                    var keyValueType = KeyValueTypes.SafeGetOrAdd(
                        param.ParameterType,
                        () => new[]
                              {
                                  param.ParameterType.GetTypeInfo().GetGenericArguments()[0],
                                  param.ParameterType.GetTypeInfo().GetGenericArguments()[1]
                              });

                    return DeserializeDictionary(keyValueType[0], keyValueType[1], value.AsDocument);
                }
                else
                {
                    return Deserialize(param.ParameterType, value);
                }
            }
            else
            {
                return null;
            }
        }

        private static bool IsList(BsonValue value, ParameterInfo param)
        {
            return value.IsArray && !param.ParameterType.IsArray;
        }

        private static object DeserializeList(Type type, BsonArray value)
        {
            var itemType = type.GetTypeInfo().GenericTypeArguments[0];

            var listType = typeof(List<>).MakeGenericType(itemType);
            var list = (IList)ObjectActivators.SafeGetOrAdd(listType, () => listType.GetActivator())();

            foreach (var item in value)
            {
                list.Add(Deserialize(itemType, item));
            }

            return list;
        }

        private static bool IsDictionary(BsonValue value, ParameterInfo param)
        {
            return value.IsDocument && param.ParameterType.IsGenericType
                   && param.ParameterType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>);
        }

        private static object DeserializeDictionary(Type keyType, Type valueType, BsonDocument value)
        {
            var dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var dict = (IDictionary)ObjectActivators.SafeGetOrAdd(dictType, () => dictType.GetActivator())();

            foreach (var key in value.Keys)
            {
                var k = keyType.GetTypeInfo().IsEnum ? Enum.Parse(keyType, key) : Convert.ChangeType(key, keyType);
                var v = Deserialize(valueType, value[key]);

                dict.Add(k, v);
            }

            return dict;
        }
    }
}