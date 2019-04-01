using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LiteDB;

namespace Chef.Extensions.LiteDB
{
    public static class Extension
    {
        private static readonly Type BsonIdAttr = typeof(BsonIdAttribute);

        private static readonly Func<Type, BsonValue, object> Deserialize =
            (Func<Type, BsonValue, object>)Delegate.CreateDelegate(
                typeof(Func<Type, BsonValue, object>),
                BsonMapper.Global,
                typeof(BsonMapper).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single(x => x.Name.Equals("Deserialize") && !x.IsGenericMethod));

        private static readonly Dictionary<Type, PropertyInfo> IdentityProps = new Dictionary<Type, PropertyInfo>();

        private static readonly Dictionary<string, BackingField> BackingFields = new Dictionary<string, BackingField>();

        public static T ToImmutability<T>(this BsonDocument me)
        {
            if (me == null) return default(T);

            var result = Activator.CreateInstance<T>();
            var resultType = typeof(T);

            var identityProp = GetIdentityProperty(resultType);
            var id = Deserialize(identityProp.PropertyType, me["_id"]);

            SetBackingFieldValue(identityProp.Name, id, result, resultType);

            foreach (var prop in resultType.GetProperties())
            {
                if (prop.Name.Equals(identityProp.Name)) continue;

                if (me.Keys.Contains(prop.Name))
                {
                    object value = null;

                    if (IsList(me, prop))
                    {
                        value = DeserializeList(prop.PropertyType, me[prop.Name].AsArray);
                    }
                    else if (IsDictionary(me, prop))
                    {
                        var keyType = prop.PropertyType.GetTypeInfo().GetGenericArguments()[0];
                        var valueType = prop.PropertyType.GetTypeInfo().GetGenericArguments()[1];

                        value = DeserializeDictionary(keyType, valueType, me[prop.Name].AsDocument);
                    }
                    else
                    {
                        value = Deserialize(prop.PropertyType, me[prop.Name]);
                    }

                    SetBackingFieldValue(prop.Name, value, result, resultType);
                }
            }

            return result;
        }

        public static BsonDocument ToDocument<T>(this T me)
        {
            return BsonMapper.Global.ToDocument(me);
        }

        public static IEnumerable<T> FindAsImmutability<T>(
            this LiteCollection<T> me,
            Query query,
            int skip = 0,
            int limit = int.MaxValue)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var engine = (LazyLoad<LiteEngine>)me.GetType()
                .GetField("_engine", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(me);

            var includes = (List<string>)me.GetType()
                .GetField("_includes", BindingFlags.Instance | BindingFlags.NonPublic)
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

            var visitor = me.GetType()
                .GetField("_visitor", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(me);

            var visit = visitor.GetType().GetMethod("Visit");

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

        public static IEnumerable<T> FindAll<T>(this LiteCollection<T> me)
        {
            return FindAsImmutability(me, Query.All());
        }

        private static PropertyInfo GetIdentityProperty(Type type)
        {
            if (!IdentityProps.ContainsKey(type))
            {
                lock (IdentityProps)
                {
                    if (!IdentityProps.ContainsKey(type))
                    {
                        var prop = type.GetProperties()
                            .SingleOrDefault(p => p.CustomAttributes.Any(a => a.AttributeType == BsonIdAttr));

                        if (prop == null) prop = type.GetProperty("Id");

                        IdentityProps.Add(type, prop);
                    }
                }
            }

            return IdentityProps[type];
        }

        private static void SetBackingFieldValue(string name, object value, object obj, Type declaringType)
        {
            var key = BackingField.GenerateKey(name, declaringType);

            if (!BackingFields.ContainsKey(key))
            {
                lock (BackingFields)
                {
                    if (!BackingFields.ContainsKey(key))
                    {
                        BackingFields.Add(key, new BackingField(name, declaringType));
                    }
                }
            }

            BackingFields[key].SetValue(obj, value);
        }

        private static bool IsList(BsonDocument doc, PropertyInfo prop)
        {
            return doc[prop.Name].IsArray && !prop.PropertyType.IsArray;
        }

        private static object DeserializeList(Type type, BsonArray value)
        {
            var itemType = type.GetTypeInfo().GenericTypeArguments[0];
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));

            foreach (var item in value)
            {
                list.Add(Deserialize(itemType, item));
            }

            return list;
        }

        private static bool IsDictionary(BsonDocument me, PropertyInfo prop)
        {
            return me[prop.Name].IsDocument && prop.PropertyType.IsGenericType
                   && prop.PropertyType.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>);
        }

        private static object DeserializeDictionary(Type keyType, Type valueType, BsonDocument value)
        {
            var dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));

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