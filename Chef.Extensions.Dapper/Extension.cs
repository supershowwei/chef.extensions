using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Chef.Extensions.Dapper.Extensions;
using Dapper;
using NotMappedAttribute = System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute;

namespace Chef.Extensions.Dapper
{
    public static class Extension
    {
        private static readonly IRowParserProvider DefaultRowParserProvider = new DefaultRowParserProvider();

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
                                                             {
                                                                 typeof(bool),
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

        private static IRowParserProvider userDefinedRowParserProvider;

        public static IRowParserProvider RowParserProvider
        {
            private get { return userDefinedRowParserProvider ?? DefaultRowParserProvider; }
            set { userDefinedRowParserProvider = value; }
        }

        public static IEnumerable<T> PolymorphicQuery<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    yield return parser(reader);
                }
            }
        }

        public static async Task<IEnumerable<T>> PolymorphicQueryAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var reader = await cnn.ExecuteReaderAsync(sql, param);

            return reader.PolymorphicExecuteReaderSync<T>(sql, discriminator);
        }

        public static T PolymorphicQuerySingle<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);
            var count = 0;

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);
                }
            }

            if (count == 0) throw new InvalidOperationException("Sequence contains no elements.");

            return result;
        }

        public static async Task<T> PolymorphicQuerySingleAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);
            var count = 0;

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);
                }
            }

            if (count == 0) throw new InvalidOperationException("Sequence contains no elements.");

            return result;
        }

        public static T PolymorphicQuerySingleOrDefault<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                var count = 0;
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);
                }
            }

            return result;
        }

        public static async Task<T> PolymorphicQuerySingleOrDefaultAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                var count = 0;
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);
                }
            }

            return result;
        }

        public static T PolymorphicQueryFirst<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    return parser(reader);
                }
            }

            throw new InvalidOperationException("Sequence contains no elements.");
        }

        public static async Task<T> PolymorphicQueryFirstAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    return parser(reader);
                }
            }

            throw new InvalidOperationException("Sequence contains no elements.");
        }

        public static T PolymorphicQueryFirstOrDefault<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);

                    break;
                }
            }

            return result;
        }

        public static async Task<T> PolymorphicQueryFirstOrDefaultAsync<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = default(T);

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader, sql);

                    result = parser(reader);

                    break;
                }
            }

            return result;
        }

        public static void PolymorphicInsert(this IDbConnection cnn, string sql, object param)
        {
            HierarchyInsert(cnn, sql, param);
        }

        public static Task PolymorphicInsertAsync(this IDbConnection cnn, string sql, object param)
        {
            return HierarchyInsertAsync(cnn, sql, param);
        }

        public static void HierarchyInsert(this IDbConnection cnn, string sql, object param)
        {
            if (param == null) throw new ArgumentException($"'{nameof(param)}' is null.");

            var props = Regex.Matches(sql, "@([^\\s,)]+)").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

            if (props.Count == 0) throw new ArgumentException($"'{nameof(sql)}' has no parameters.");

            if (param is IEnumerable objs)
            {
                var parameters = new List<object>();

                foreach (var obj in objs)
                {
                    parameters.Add(GetParameter(props, obj));
                }

                if (!parameters.Any()) throw new ArgumentException($"'{nameof(param)}' is empty.");

                cnn.Execute(sql, parameters);
            }
            else
            {
                cnn.Execute(sql, GetParameter(props, param));
            }
        }

        public static Task HierarchyInsertAsync(this IDbConnection cnn, string sql, object param)
        {
            if (param == null) throw new ArgumentException($"'{nameof(param)}' is null.");

            var props = Regex.Matches(sql, "@([^\\s,)]+)").Cast<Match>().Select(m => m.Groups[1].Value).ToList();

            if (props.Count == 0) throw new ArgumentException($"'{nameof(sql)}' has no parameters.");

            if (param is IEnumerable objs)
            {
                var parameters = new List<object>();

                foreach (var obj in objs)
                {
                    parameters.Add(GetParameter(props, obj));
                }

                if (!parameters.Any()) throw new ArgumentException($"'{nameof(param)}' is empty.");

                return cnn.ExecuteAsync(sql, parameters);
            }
            else
            {
                return cnn.ExecuteAsync(sql, GetParameter(props, param));
            }
        }

        public static IEnumerable<T> QueryAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    yield return (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }
        }

        public static async Task<IEnumerable<T>> QueryAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var reader = await cnn.ExecuteReaderAsync(sql, param);

            return reader.ExecuteReaderAsImmutabilitySync<T>(sql);
        }

        public static T QuerySingleAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);
            var count = 0;

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            if (count == 0) throw new InvalidOperationException("Sequence contains no elements.");

            return result;
        }

        public static async Task<T> QuerySingleAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);
            var count = 0;

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            if (count == 0) throw new InvalidOperationException("Sequence contains no elements.");

            return result;
        }

        public static T QuerySingleOrDefaultAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                var count = 0;
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            return result;
        }

        public static async Task<T> QuerySingleOrDefaultAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                var count = 0;
                while (reader.Read())
                {
                    if (++count > 1) throw new InvalidOperationException("Sequence contains more than one element.");

                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            return result;
        }

        public static T QueryFirstAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    return (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            throw new InvalidOperationException("Sequence contains no elements.");
        }

        public static async Task<T> QueryFirstAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    return (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));
                }
            }

            throw new InvalidOperationException("Sequence contains no elements.");
        }

        public static T QueryFirstOrDefaultAsImmutability<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));

                    break;
                }
            }

            return result;
        }

        public static async Task<T> QueryFirstOrDefaultAsImmutabilityAsync<T>(this IDbConnection cnn, string sql, object param = null)
        {
            var result = default(T);

            var reader = await cnn.ExecuteReaderAsync(sql, param);

            using (reader)
            {
                while (reader.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    result = (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(reader));

                    break;
                }
            }

            return result;
        }

        /// <summary>
        ///     Length of the string is default 4000
        /// </summary>
        public static DbString ToVarchar(this string me)
        {
            return new DbString { Value = me, IsAnsi = true };
        }

        /// <summary>
        ///     Length of the string -1 for max
        /// </summary>
        public static DbString ToVarchar(this string me, int length)
        {
            return new DbString { Value = me, Length = length, IsAnsi = true };
        }

        /// <summary>
        ///     Length of the string is default 4000
        /// </summary>
        public static DbString ToChar(this string me)
        {
            return new DbString { Value = me, IsAnsi = true, IsFixedLength = true };
        }

        /// <summary>
        ///     Length of the string -1 for max
        /// </summary>
        public static DbString ToChar(this string me, int length)
        {
            return new DbString { Value = me, Length = length, IsAnsi = true, IsFixedLength = true };
        }

        /// <summary>
        ///     Length of the string is default 4000
        /// </summary>
        public static DbString ToNVarchar(this string me)
        {
            return new DbString { Value = me };
        }

        /// <summary>
        ///     Length of the string -1 for max
        /// </summary>
        public static DbString ToNVarchar(this string me, int length)
        {
            return new DbString { Value = me, Length = length };
        }

        /// <summary>
        ///     Length of the string is default 4000
        /// </summary>
        public static DbString ToNChar(this string me)
        {
            return new DbString { Value = me, IsFixedLength = true };
        }

        /// <summary>
        ///     Length of the string -1 for max
        /// </summary>
        public static DbString ToNChar(this string me, int length)
        {
            return new DbString { Value = me, Length = length, IsFixedLength = true };
        }

        public static string ToSelectList(this PropertyInfo[] me)
        {
            return ToSelectList(me, string.Empty);
        }

        public static string ToSelectList(this PropertyInfo[] me, string alias)
        {
            if (me == null || me.Length == 0) throw new ArgumentException($"'{nameof(me)}' can not be null or empty.");

            alias = string.IsNullOrEmpty(alias) ? alias : string.Concat("[", alias, "]");

            var sb = new StringBuilder();

            foreach (var propertyInfo in me)
            {
                if (Attribute.IsDefined(propertyInfo, typeof(NotMappedAttribute))) continue;

                var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute?.Name;

                sb.AliasAppend(string.IsNullOrEmpty(columnName) ? $"[{propertyInfo.Name}], " : $"[{columnName}] AS [{propertyInfo.Name}], ", alias);
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public static string ToSelectList<T>(this Expression<Func<T, object>> me)
        {
            return ToSelectList(me, string.Empty);
        }

        public static string ToSelectList<T>(this Expression<Func<T, object>> me, string alias)
        {
            var memberExprs = GetMemberExpressions(me.Body);

            alias = string.IsNullOrEmpty(alias) ? alias : string.Concat("[", alias, "]");

            var sb = new StringBuilder();

            foreach (var memberExpr in memberExprs)
            {
                if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute))) continue;

                var columnAttribute = memberExpr.Member.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute?.Name;

                sb.AliasAppend(string.IsNullOrEmpty(columnName) ? $"[{memberExpr.Member.Name}], " : $"[{columnName}] AS [{memberExpr.Member.Name}], ", alias);
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public static string ToSelectList<T, TSecond>(this Expression<Func<T, TSecond, object>> me, out string splitOn)
        {
            return ToSelectList(me, new string[] { }, out splitOn);
        }

        public static string ToSelectList<T, TSecond>(this Expression<Func<T, TSecond, object>> me, string[] aliases, out string splitOn)
        {
            return GenerateJoinSelectList(me.Body, me.Parameters, aliases, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, object>> me, out string splitOn)
        {
            return ToSelectList(me, new string[] { }, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, object>> me, string[] aliases, out string splitOn)
        {
            return GenerateJoinSelectList(me.Body, me.Parameters, aliases, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, object>> me, out string splitOn)
        {
            return ToSelectList(me, new string[] { }, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, object>> me, string[] aliases, out string splitOn)
        {
            return GenerateJoinSelectList(me.Body, me.Parameters, aliases, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> me, out string splitOn)
        {
            return ToSelectList(me, new string[] { }, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> me, string[] aliases, out string splitOn)
        {
            return GenerateJoinSelectList(me.Body, me.Parameters, aliases, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> me, out string splitOn)
        {
            return ToSelectList(me, new string[] { }, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> me, string[] aliases, out string splitOn)
        {
            return GenerateJoinSelectList(me.Body, me.Parameters, aliases, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> me, out string splitOn)
        {
            return ToSelectList(me, new string[] { }, out splitOn);
        }

        public static string ToSelectList<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> me, string[] aliases, out string splitOn)
        {
            return GenerateJoinSelectList(me.Body, me.Parameters, aliases, out splitOn);
        }

        public static string ToSearchCondition<T>(this Expression<Func<T, bool>> me)
        {
            return ToSearchCondition(me, string.Empty, null);
        }

        public static string ToSearchCondition<T>(this Expression<Func<T, bool>> me, string alias)
        {
            return ToSearchCondition(me, alias, null);
        }

        public static string ToSearchCondition<T>(this Expression<Func<T, bool>> me, out IDictionary<string, object> parameters)
        {
            return ToSearchCondition(me, string.Empty, out parameters);
        }

        public static string ToSearchCondition<T>(this Expression<Func<T, bool>> me, string alias, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, alias, parameters);
        }

        public static string ToSearchCondition<T>(this Expression<Func<T, bool>> me, IDictionary<string, object> parameters)
        {
            return ToSearchCondition(me, string.Empty, parameters);
        }

        public static string ToSearchCondition<T>(this Expression<Func<T, bool>> me, string alias, IDictionary<string, object> parameters)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, new[] { alias });

            var sb = new StringBuilder();

            ParseCondition(me.Body, aliasMap, sb, parameters);

            return sb.ToString();
        }

        public static string ToSearchCondition<T, TSecond>(this Expression<Func<T, TSecond, bool>> me, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond>(this Expression<Func<T, TSecond, bool>> me, string[] aliases, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, aliases, parameters);
        }

        public static string ToSearchCondition<T, TSecond>(this Expression<Func<T, TSecond, bool>> me, IDictionary<string, object> parameters)
        {
            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond>(this Expression<Func<T, TSecond, bool>> me, string[] aliases, IDictionary<string, object> parameters)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, aliases);

            var sb = new StringBuilder();

            ParseCondition(me.Body, aliasMap, sb, parameters);

            return sb.ToString();
        }

        public static string ToSearchCondition<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, bool>> me, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, bool>> me, string[] aliases, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, aliases, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, bool>> me, IDictionary<string, object> parameters)
        {
            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, bool>> me, string[] aliases, IDictionary<string, object> parameters)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, aliases);

            var sb = new StringBuilder();

            ParseCondition(me.Body, aliasMap, sb, parameters);

            return sb.ToString();
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, bool>> me, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, bool>> me, string[] aliases, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, aliases, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, bool>> me, IDictionary<string, object> parameters)
        {
            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, bool>> me, string[] aliases, IDictionary<string, object> parameters)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, aliases);

            var sb = new StringBuilder();

            ParseCondition(me.Body, aliasMap, sb, parameters);

            return sb.ToString();
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> me, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> me, string[] aliases, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, aliases, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> me, IDictionary<string, object> parameters)
        {
            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> me, string[] aliases, IDictionary<string, object> parameters)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, aliases);

            var sb = new StringBuilder();

            ParseCondition(me.Body, aliasMap, sb, parameters);

            return sb.ToString();
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> me, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> me, string[] aliases, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, aliases, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> me, IDictionary<string, object> parameters)
        {
            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> me, string[] aliases, IDictionary<string, object> parameters)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, aliases);

            var sb = new StringBuilder();

            ParseCondition(me.Body, aliasMap, sb, parameters);

            return sb.ToString();
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> me, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> me, string[] aliases, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSearchCondition(me, aliases, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> me, IDictionary<string, object> parameters)
        {
            return ToSearchCondition(me, new string[] { }, parameters);
        }

        public static string ToSearchCondition<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> me, string[] aliases, IDictionary<string, object> parameters)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, aliases);

            var sb = new StringBuilder();

            ParseCondition(me.Body, aliasMap, sb, parameters);

            return sb.ToString();
        }

        public static string ToInnerJoin<TRight>(this LambdaExpression me)
        {
            return ToInnerJoin<TRight>(me, new string[] { });
        }

        public static string ToInnerJoin<TRight>(this LambdaExpression me, string[] aliases)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, aliases);

            var rightTable = typeof(TRight).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(TRight).Name;
            var rightTableAlias = aliasMap[me.Parameters.Last().Name];

            var sb = new StringBuilder();

            sb.Append($"INNER JOIN {rightTable}");

            if (!string.IsNullOrEmpty(rightTableAlias)) sb.Append($" {rightTableAlias}");

            sb.Append(" WITH (NOLOCK) ON ");

            ParseJoinCodition(me.Body, aliasMap, sb);

            return sb.ToString();
        }

        public static string ToLeftJoin<TRight>(this LambdaExpression me)
        {
            return ToLeftJoin<TRight>(me, new string[] { });
        }

        public static string ToLeftJoin<TRight>(this LambdaExpression me, string[] aliases)
        {
            var aliasMap = GenerateAliasMap(me.Parameters, aliases);

            var rightTable = typeof(TRight).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(TRight).Name;
            var rightTableAlias = aliasMap[me.Parameters.Last().Name];

            var sb = new StringBuilder();

            sb.Append($"LEFT JOIN {rightTable}");

            if (!string.IsNullOrEmpty(rightTableAlias)) sb.Append($" {rightTableAlias}");

            sb.Append(" WITH (NOLOCK) ON ");

            ParseJoinCodition(me.Body, aliasMap, sb);

            return sb.ToString();
        }

        public static string ToColumnList(this PropertyInfo[] me, out string valueList)
        {
            if (me == null || me.Length == 0) throw new ArgumentException($"'{nameof(me)}' can not be null or empty.");

            var columnListBuilder = new StringBuilder();
            var valueListBuilder = new StringBuilder();

            foreach (var propertyInfo in me)
            {
                if (Attribute.IsDefined(propertyInfo, typeof(NotMappedAttribute)))
                {
                    throw new ArgumentException("Member can not applied [NotMapped].");
                }

                var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                var parameterName = propertyInfo.Name;
                var columnName = columnAttribute?.Name ?? parameterName;
                var parameterType = propertyInfo.PropertyType;

                columnListBuilder.Append($"[{columnName}], ");
                valueListBuilder.Append($"{GenerateParameterStatement(parameterName, parameterType, null)}, ");
            }

            columnListBuilder.Remove(columnListBuilder.Length - 2, 2);
            valueListBuilder.Remove(valueListBuilder.Length - 2, 2);

            valueList = valueListBuilder.ToString();

            return columnListBuilder.ToString();
        }

        public static string ToColumnList<T>(this Expression<Func<T>> me, out string valueList)
        {
            return ToColumnList(me, out valueList, null);
        }

        public static string ToColumnList<T>(this Expression<Func<T>> me, out string valueList, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToColumnList(me, out valueList, parameters);
        }

        public static string ToColumnList<T>(this Expression<Func<T>> me, out string valueList, IDictionary<string, object> parameters)
        {
            if (!(me.Body is MemberInitExpression memberInitExpr)) throw new ArgumentException("Must be member initializer.");

            var columnListBuilder = new StringBuilder();
            var valueListBuilder = new StringBuilder();

            foreach (var binding in memberInitExpr.Bindings)
            {
                if (!(binding is MemberAssignment memberAssignment)) throw new ArgumentException("Must be member assignment.");

                if (Attribute.IsDefined(memberAssignment.Member, typeof(NotMappedAttribute)))
                {
                    throw new ArgumentException("Member can not applied [NotMapped].");
                }

                var columnAttribute = memberAssignment.Member.GetCustomAttribute<ColumnAttribute>();
                var parameterName = memberAssignment.Member.Name;
                var columnName = columnAttribute?.Name ?? parameterName;
                var parameterType = (memberAssignment.Member as PropertyInfo)?.PropertyType ?? memberAssignment.Member.DeclaringType;

                if (parameters != null)
                {
                    SetParameter(memberAssignment.Member, ExtractConstant(memberAssignment.Expression), columnAttribute, parameters, out parameterName);
                }

                columnListBuilder.Append($"[{columnName}], ");
                valueListBuilder.Append($"{GenerateParameterStatement(parameterName, parameterType, parameters)}, ");
            }

            columnListBuilder.Remove(columnListBuilder.Length - 2, 2);
            valueListBuilder.Remove(valueListBuilder.Length - 2, 2);

            valueList = valueListBuilder.ToString();

            return columnListBuilder.ToString();
        }

        public static string ToSetStatements<T>(this Expression<Func<T>> me)
        {
            return ToSetStatements(me, string.Empty, null);
        }

        public static string ToSetStatements<T>(this Expression<Func<T>> me, string alias)
        {
            return ToSetStatements(me, alias, null);
        }

        public static string ToSetStatements<T>(this Expression<Func<T>> me, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSetStatements(me, string.Empty, parameters);
        }

        public static string ToSetStatements<T>(this Expression<Func<T>> me, string alias, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            return ToSetStatements(me, alias, parameters);
        }

        public static string ToSetStatements<T>(this Expression<Func<T>> me, IDictionary<string, object> parameters)
        {
            return ToSetStatements(me, string.Empty, parameters);
        }

        public static string ToSetStatements<T>(this Expression<Func<T>> me, string alias, IDictionary<string, object> parameters)
        {
            if (!(me.Body is MemberInitExpression memberInitExpr)) throw new ArgumentException("Must be member initializer.");

            alias = string.IsNullOrEmpty(alias) ? alias : string.Concat("[", alias, "]");

            var sb = new StringBuilder();

            foreach (var binding in memberInitExpr.Bindings)
            {
                if (!(binding is MemberAssignment memberAssignment)) throw new ArgumentException("Must be member assignment.");

                if (Attribute.IsDefined(memberAssignment.Member, typeof(NotMappedAttribute)))
                {
                    throw new ArgumentException("Member can not applied [NotMapped].");
                }

                var columnAttribute = memberAssignment.Member.GetCustomAttribute<ColumnAttribute>();
                var parameterName = memberAssignment.Member.Name;
                var columnName = columnAttribute?.Name ?? parameterName;
                var parameterType = (memberAssignment.Member as PropertyInfo)?.PropertyType ?? memberAssignment.Member.DeclaringType;

                if (parameters != null)
                {
                    SetParameter(memberAssignment.Member, ExtractConstant(memberAssignment.Expression), columnAttribute, parameters, out parameterName);
                }

                sb.AliasAppend($"[{columnName}] = {GenerateParameterStatement(parameterName, parameterType, parameters)}, ", alias);
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public static string ToOrderAscending<T>(this Expression<Func<T, object>> me)
        {
            return ToOrderAscending(me, string.Empty);
        }

        public static string ToOrderAscending<T>(this Expression<Func<T, object>> me, string alias)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderAscending(memberExpr, GenerateAliasMap(me.Parameters, new[] { alias }));
        }

        public static string ToOrderDescending<T>(this Expression<Func<T, object>> me)
        {
            return ToOrderDescending(me, string.Empty);
        }

        public static string ToOrderDescending<T>(this Expression<Func<T, object>> me, string alias)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderDescending(memberExpr, GenerateAliasMap(me.Parameters, new[] { alias }));
        }

        public static string ToOrderAscending<T, TSecond>(this Expression<Func<T, TSecond, object>> me)
        {
            return ToOrderAscending(me, new string[] { });
        }

        public static string ToOrderAscending<T, TSecond>(this Expression<Func<T, TSecond, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderAscending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderDescending<T, TSecond>(this Expression<Func<T, TSecond, object>> me)
        {
            return ToOrderDescending(me, new string[] { });
        }

        public static string ToOrderDescending<T, TSecond>(this Expression<Func<T, TSecond, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderDescending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderAscending<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, object>> me)
        {
            return ToOrderAscending(me, new string[] { });
        }

        public static string ToOrderAscending<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderAscending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderDescending<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, object>> me)
        {
            return ToOrderDescending(me, new string[] { });
        }

        public static string ToOrderDescending<T, TSecond, TThird>(this Expression<Func<T, TSecond, TThird, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderDescending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderAscending<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, object>> me)
        {
            return ToOrderAscending(me, new string[] { });
        }

        public static string ToOrderAscending<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderAscending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderDescending<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, object>> me)
        {
            return ToOrderDescending(me, new string[] { });
        }

        public static string ToOrderDescending<T, TSecond, TThird, TFourth>(this Expression<Func<T, TSecond, TThird, TFourth, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderDescending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderAscending<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> me)
        {
            return ToOrderAscending(me, new string[] { });
        }

        public static string ToOrderAscending<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderAscending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderDescending<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> me)
        {
            return ToOrderDescending(me, new string[] { });
        }

        public static string ToOrderDescending<T, TSecond, TThird, TFourth, TFifth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderDescending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderAscending<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> me)
        {
            return ToOrderAscending(me, new string[] { });
        }

        public static string ToOrderAscending<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderAscending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderDescending<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> me)
        {
            return ToOrderDescending(me, new string[] { });
        }

        public static string ToOrderDescending<T, TSecond, TThird, TFourth, TFifth, TSixth>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderDescending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderAscending<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> me)
        {
            return ToOrderAscending(me, new string[] { });
        }

        public static string ToOrderAscending<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderAscending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        public static string ToOrderDescending<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> me)
        {
            return ToOrderDescending(me, new string[] { });
        }

        public static string ToOrderDescending<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> me, string[] aliases)
        {
            var memberExpr = ExtractMember(me.Body);

            return ToOrderDescending(memberExpr, GenerateAliasMap(me.Parameters, aliases));
        }

        private static IEnumerable<T> PolymorphicExecuteReaderSync<T>(this IDataReader me, string sql, string discriminator)
        {
            using (me)
            {
                while (me.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, me, sql);

                    yield return parser(me);
                }
            }
        }

        private static IEnumerable<T> ExecuteReaderAsImmutabilitySync<T>(this IDataReader me, string sql)
        {
            using (me)
            {
                while (me.Read())
                {
                    var constructor = typeof(T).GetConstructors().First();

                    yield return (T)Activator.CreateInstance(typeof(T), constructor.GetArguments(me));
                }
            }
        }

        private static ExpandoObject GetParameter(List<string> props, object obj)
        {
            var expando = (IDictionary<string, object>)new ExpandoObject();

            foreach (var prop in props)
            {
                expando[prop] = GetObjValue(prop, obj);
            }

            return (ExpandoObject)expando;
        }

        private static object GetObjValue(string propName, object obj)
        {
            var objType = obj.GetType();

            int underscoreIndex;
            if ((underscoreIndex = propName.IndexOf('_')) < 0)
            {
                var objProp = objType.GetProperty(propName);

                return objProp == null ? null : objProp.GetValue(obj);
            }
            else
            {
                var objProp = objType.GetProperty(propName.Substring(0, underscoreIndex));

                return objProp == null
                           ? null
                           : GetObjValue(propName.Substring(underscoreIndex + 1), objProp.GetValue(obj));
            }
        }

        private static IDictionary<string, string> GenerateAliasMap(IList<ParameterExpression> parameterExprs, string[] aliases)
        {
            var aliasMap = new Dictionary<string, string>();

            for (var i = 0; i < parameterExprs.Count; i++)
            {
                if ((i + 1) > aliases.Length)
                {
                    aliasMap.Add(parameterExprs[i].Name, string.Empty);
                }
                else
                {
                    var alias = aliases[i];

                    alias = string.IsNullOrEmpty(alias) ? alias : string.Concat("[", alias, "]");

                    aliasMap.Add(parameterExprs[i].Name, alias);
                }
            }

            return aliasMap;
        }

        private static void ParseCondition(Expression expr, IDictionary<string, string> aliasMap, StringBuilder sb, IDictionary<string, object> parameters)
        {
            if (expr is BinaryExpression binaryExpr)
            {
                if (binaryExpr.NodeType == ExpressionType.AndAlso || binaryExpr.NodeType == ExpressionType.OrElse)
                {
                    sb.Append("(");

                    ParseCondition(binaryExpr.Left, aliasMap, sb, parameters);

                    switch (binaryExpr.NodeType)
                    {
                        case ExpressionType.AndAlso:
                            sb.Append(") AND (");
                            break;

                        case ExpressionType.OrElse:
                            sb.Append(") OR (");
                            break;
                    }

                    ParseCondition(binaryExpr.Right, aliasMap, sb, parameters);

                    sb.Append(")");
                }
                else
                {
                    if (!(binaryExpr.Left is MemberExpression left))
                    {
                        if (!(binaryExpr.Left is UnaryExpression unaryExpr))
                        {
                            throw new ArgumentException("Left expression must be MemberExpression.");
                        }

                        if ((left = unaryExpr.Operand as MemberExpression) == null)
                        {
                            throw new ArgumentException("Left expression must be MemberExpression.");
                        }
                    }

                    if (!(left.Expression is ParameterExpression parameterExpr))
                    {
                        throw new ArgumentException("Parameter expression must be placed left.");
                    }

                    if (Attribute.IsDefined(left.Member, typeof(NotMappedAttribute)))
                    {
                        throw new ArgumentException("Member can not applied [NotMapped].");
                    }

                    var alias = aliasMap[parameterExpr.Name];
                    var columnAttribute = left.Member.GetCustomAttribute<ColumnAttribute>();
                    var parameterName = left.Member.Name;
                    var columnName = columnAttribute?.Name ?? parameterName;
                    var parameterType = (left.Member as PropertyInfo)?.PropertyType ?? left.Member.DeclaringType;

                    if (parameters != null)
                    {
                        SetParameter(left.Member, ExtractConstant(binaryExpr.Right), columnAttribute, parameters, out parameterName);
                    }

                    if (parameters != null && parameters[parameterName] == null)
                    {
                        switch (binaryExpr.NodeType)
                        {
                            case ExpressionType.Equal:
                                sb.AliasAppend($"[{columnName}] IS NULL", alias);
                                break;

                            case ExpressionType.NotEqual:
                                sb.AliasAppend($"[{columnName}] IS NOT NULL", alias);
                                break;

                            default: throw new ArgumentException("Invalid NodeType.");
                        }
                    }
                    else
                    {
                        sb.AliasAppend($"[{columnName}] {MapOperator(binaryExpr.NodeType)} {GenerateParameterStatement(parameterName, parameterType, parameters)}", alias);
                    }
                }
            }
            else if (expr is MethodCallExpression methodCallExpr)
            {
                var methodFullName = methodCallExpr.Method.GetFullName();

                if (methodFullName.EndsWith(".Equals"))
                {
                    var argumentExpr = (MemberExpression)methodCallExpr.Object;

                    if (Attribute.IsDefined(argumentExpr.Member, typeof(NotMappedAttribute)))
                    {
                        throw new ArgumentException("Member can not applied [NotMapped].");
                    }

                    var parameterExpr = (ParameterExpression)argumentExpr.Expression;

                    var alias = aliasMap[parameterExpr.Name];
                    var columnAttribute = argumentExpr.Member.GetCustomAttribute<ColumnAttribute>();
                    var parameterName = argumentExpr.Member.Name;
                    var columnName = columnAttribute?.Name ?? parameterName;
                    var parameterType = (argumentExpr.Member as PropertyInfo)?.PropertyType ?? argumentExpr.Member.DeclaringType;
                    
                    if (parameters != null)
                    {
                        SetParameter(argumentExpr.Member, ExtractConstant(methodCallExpr.Arguments[0]), columnAttribute, parameters, out parameterName);
                    }

                    sb.AliasAppend($"[{columnName}] = {GenerateParameterStatement(parameterName, parameterType, parameters)}", alias);
                }
                else if (methodFullName.Equals("System.Linq.Enumerable.Contains"))
                {
                    var argumentExpr = (MemberExpression)methodCallExpr.Arguments[1];

                    if (Attribute.IsDefined(argumentExpr.Member, typeof(NotMappedAttribute)))
                    {
                        throw new ArgumentException("Member can not applied [NotMapped].");
                    }

                    var parameterExpr = (ParameterExpression)argumentExpr.Expression;

                    var alias = aliasMap[parameterExpr.Name];
                    var columnAttribute = argumentExpr.Member.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttribute?.Name ?? argumentExpr.Member.Name;
                    var parameterType = (argumentExpr.Member as PropertyInfo)?.PropertyType ?? argumentExpr.Member.DeclaringType;

                    var array = ExtractArray(methodCallExpr);

                    foreach (var item in array)
                    {
                        if (parameters == null) throw new ArgumentException($"'{nameof(parameters)}' can not be null.");

                        SetParameter(argumentExpr.Member, item, columnAttribute, parameters, out var parameterName);

                        sb.AliasAppend($"[{columnName}] = {GenerateParameterStatement(parameterName, parameterType, parameters)} OR ", alias);
                    }

                    sb.Remove(sb.Length - 4, 4);
                }
                else if (methodFullName.IsLikeOperator())
                {
                    var argumentExpr = (MemberExpression)methodCallExpr.Object;

                    if (Attribute.IsDefined(argumentExpr.Member, typeof(NotMappedAttribute)))
                    {
                        throw new ArgumentException("Member can not applied [NotMapped].");
                    }

                    var parameterExpr = (ParameterExpression)argumentExpr.Expression;

                    var alias = aliasMap[parameterExpr.Name];
                    var columnAttribute = argumentExpr.Member.GetCustomAttribute<ColumnAttribute>();
                    var parameterName = argumentExpr.Member.Name;
                    var columnName = columnAttribute?.Name ?? parameterName;
                    var parameterType = (argumentExpr.Member as PropertyInfo)?.PropertyType ?? argumentExpr.Member.DeclaringType;

                    if (parameters != null)
                    {
                        SetParameter(argumentExpr.Member, ExtractConstant(methodCallExpr.Arguments[0]), columnAttribute, parameters, out parameterName);
                    }

                    if (methodFullName.Equals("System.String.Contains"))
                    {
                        sb.AliasAppend($"[{columnName}] LIKE '%' + {GenerateParameterStatement(parameterName, parameterType, parameters)} + '%'", alias);
                    }
                    else if (methodFullName.Equals("System.String.StartsWith"))
                    {
                        sb.AliasAppend($"[{columnName}] LIKE {GenerateParameterStatement(parameterName, parameterType, parameters)} + '%'", alias);
                    }
                    else if (methodFullName.Equals("System.String.EndsWith"))
                    {
                        sb.AliasAppend($"[{columnName}] LIKE '%' + {GenerateParameterStatement(parameterName, parameterType, parameters)}", alias);
                    }
                }
            }
        }

        private static void ParseJoinCodition(Expression expr, IDictionary<string, string> aliasMap, StringBuilder sb)
        {
            if (expr is BinaryExpression binaryExpr)
            {
                if (binaryExpr.NodeType == ExpressionType.AndAlso || binaryExpr.NodeType == ExpressionType.OrElse)
                {
                    sb.Append("(");

                    ParseJoinCodition(binaryExpr.Left, aliasMap, sb);

                    switch (binaryExpr.NodeType)
                    {
                        case ExpressionType.AndAlso:
                            sb.Append(") AND (");
                            break;

                        case ExpressionType.OrElse:
                            sb.Append(") OR (");
                            break;
                    }

                    ParseJoinCodition(binaryExpr.Right, aliasMap, sb);

                    sb.Append(")");
                }
                else
                {
                    ParseJoinCodition(binaryExpr.Left, aliasMap, sb);

                    sb.Append($" {MapOperator(binaryExpr.NodeType)} ");

                    ParseJoinCodition(binaryExpr.Right, aliasMap, sb);
                }
            }
            else
            {
                if (!(expr is MemberExpression memberExpr))
                {
                    throw new ArgumentException("Expression must be MemberExpression.");
                }

                if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
                {
                    throw new ArgumentException("Member can not applied [NotMapped].");
                }

                var parameterExpr = (ParameterExpression)memberExpr.Expression;

                var alias = aliasMap[parameterExpr.Name];
                var columnAttribute = memberExpr.Member.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute?.Name ?? memberExpr.Member.Name;

                sb.AliasAppend($"[{columnName}]", alias);
            }
        }

        private static void SetParameter(MemberInfo member, object value, ColumnAttribute columnAttribute, IDictionary<string, object> parameters, out string parameterName)
        {
            parameterName = CreateUniqueParameterName(member.Name, parameters);

            if (value != null && !string.IsNullOrEmpty(columnAttribute?.TypeName))
            {
                parameters[parameterName] = CreateDbString(
                    (string)value,
                    columnAttribute.TypeName,
                    member.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength ?? -1);
            }
            else
            {
                parameters[parameterName] = value;
            }
        }

        private static string CreateUniqueParameterName(string memberName, IDictionary<string, object> parameters)
        {
            var index = 0;

            string parameterName;
            while (parameters.ContainsKey(parameterName = $"{memberName}_{index++}"))
            {
            }

            return parameterName;
        }

        private static object ExtractConstant(Expression expr)
        {
            if (expr == null) return null;

            if (expr is MemberExpression memberExpr)
            {
                if (memberExpr.Member.MemberType == MemberTypes.Field)
                {
                    return ((FieldInfo)memberExpr.Member).GetValue((memberExpr.Expression as ConstantExpression)?.Value);
                }

                if (memberExpr.Member.MemberType == MemberTypes.Property)
                {
                    return ((PropertyInfo)memberExpr.Member).GetValue(ExtractConstant(memberExpr.Expression));
                }
            }

            if (expr is ConstantExpression constantExpr)
            {
                return constantExpr.Value;
            }

            if (expr is UnaryExpression unaryExpr)
            {
                return ExtractConstant(unaryExpr.Operand);
            }

            throw new ArgumentException("Right expression's node type must be Field or Property'");
        }

        private static DbString CreateDbString(string value, string typeName, int length)
        {
            switch (typeName.ToUpperInvariant())
            {
                case "VARCHAR": return new DbString { Value = value, IsAnsi = true, Length = length };
                case "CHAR": return new DbString { Value = value, IsFixedLength = true, IsAnsi = true, Length = length };
                case "NCHAR": return new DbString { Value = value, IsFixedLength = true, Length = length };
                case "NVARCHAR":
                default: return new DbString { Value = value, Length = length };
            }
        }

        private static IEnumerable ExtractArray(MethodCallExpression methodCallExpr)
        {
            switch (methodCallExpr.Arguments[0])
            {
                case NewArrayExpression newArrayExpr: return newArrayExpr.Expressions.Select(e => ExtractConstant(e)).ToArray();
                case MemberExpression arrayExpr: return (IEnumerable)ExtractConstant(arrayExpr);
                default: throw new ArgumentException("Must be a array variable or array initializer.");
            }
        }

        private static string MapOperator(ExpressionType exprType)
        {
            switch (exprType)
            {
                case ExpressionType.Equal: return "=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                default: throw new ArgumentException("Invalid NodeType.");
            }
        }

        private static IEnumerable<MemberExpression> GetMemberExpressions(Expression expr)
        {
            if (expr is NewExpression newExpr) return newExpr.Arguments.OfType<MemberExpression>();

            if (expr is MemberInitExpression memberInitExpr)
            {
                return memberInitExpr.Bindings.Select(x => (MemberExpression)((MemberAssignment)x).Expression);
            }

            throw new ArgumentException("Selector must be a NewExpression or MemberInitExpression.");
        }

        private static string GenerateJoinSelectList(Expression body, IList<ParameterExpression> parameters, string[] aliases, out string splitOn)
        {
            var memberExprs = GetMemberExpressions(body);

            var splitOnList = new List<string>();
            var aliasMap = GenerateAliasMap(parameters, aliases);
            var selectorContainers = parameters.ToDictionary(x => x.Name, x => new List<string>());

            foreach (var memberExpr in memberExprs)
            {
                if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute))) continue;

                var parameterExpr = (ParameterExpression)memberExpr.Expression;

                var alias = aliasMap[parameterExpr.Name];
                var selectorContainer = selectorContainers[parameterExpr.Name];
                var columnAttribute = memberExpr.Member.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute?.Name;

                var statement = string.IsNullOrEmpty(columnName)
                                    ? $"[{memberExpr.Member.Name}]"
                                    : $"[{columnName}] AS [{memberExpr.Member.Name}]";

                if (!string.IsNullOrEmpty(alias)) statement = string.Concat($"{alias}.", statement);

                if (!selectorContainer.Any()) splitOnList.Add(memberExpr.Member.Name);

                selectorContainer.Add(statement);
            }

            if (splitOnList.Count < selectorContainers.Count) throw new InvalidOperationException("Selected columns must cover all joined tables.");

            splitOn = string.Join(",", splitOnList.Skip(1));

            return string.Join(", ", selectorContainers.SelectMany(x => x.Value));
        }

        private static string GenerateParameterStatement(string parameterName, Type parameterType, IDictionary<string, object> parameters)
        {
            if (parameters?[parameterName] != null)
            {
                parameterType = parameters[parameterName].GetType();
            }

            if (NumericTypes.Contains(parameterType)) return $"{{={parameterName}}}";

            return $"@{parameterName}";
        }

        private static MemberExpression ExtractMember(Expression expr)
        {
            switch (expr)
            {
                case UnaryExpression unaryExpr: return (MemberExpression)unaryExpr.Operand;
                case MemberExpression memberExpr: return memberExpr;
                default: throw new ArgumentException("Body expression must be MemberExpression.");
            }
        }

        private static string ToOrderAscending(MemberExpression memberExpr, IDictionary<string, string> aliasMap)
        {
            if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
            {
                throw new ArgumentException("Member can not applied [NotMapped].");
            }

            var parameterExpr = (ParameterExpression)memberExpr.Expression;

            var alias = aliasMap[parameterExpr.Name];
            var columnAttribute = memberExpr.Member.GetCustomAttribute<ColumnAttribute>();
            var columnName = columnAttribute?.Name ?? memberExpr.Member.Name;

            return string.IsNullOrEmpty(alias) ? $"[{columnName}] ASC" : $"{alias}.[{columnName}] ASC";
        }

        private static string ToOrderDescending(MemberExpression memberExpr, IDictionary<string, string> aliasMap)
        {
            if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
            {
                throw new ArgumentException("Member can not applied [NotMapped].");
            }

            var parameterExpr = (ParameterExpression)memberExpr.Expression;

            var alias = aliasMap[parameterExpr.Name];
            var columnAttribute = memberExpr.Member.GetCustomAttribute<ColumnAttribute>();
            var columnName = columnAttribute?.Name ?? memberExpr.Member.Name;

            return string.IsNullOrEmpty(alias) ? $"[{columnName}] DESC" : $"{alias}.[{columnName}] DESC";
        }
    }
}