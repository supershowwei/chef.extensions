using System;
using System.Collections;
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
using Chef.Extensions.Dapper.Extensions;
using Dapper;

namespace Chef.Extensions.Dapper
{
    public static class Extension
    {
        private static readonly IRowParserProvider DefaultRowParserProvider = new DefaultRowParserProvider();

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
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

        private static readonly Dictionary<Type, PropertyInfo[]> PropertyCollection = new Dictionary<Type, PropertyInfo[]>();

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

        public static void PolymorphicInsert(this IDbConnection cnn, string sql, object param)
        {
            HierarchyInsert(cnn, sql, param);
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
            var sb = new StringBuilder();

            ParseCondition(me.Body, alias, sb, parameters);

            return sb.ToString();
        }

        public static string ToSelectList<T>(this Expression<Func<T, object>> me)
        {
            return ToSelectList(me, string.Empty);
        }

        public static string ToSelectList<T>(this Expression<Func<T, object>> me, string alias)
        {
            var sb = new StringBuilder();
            var targetType = typeof(T);

            foreach (var returnProp in me.Body.Type.GetCacheProperties())
            {
                var property = targetType.GetProperty(returnProp.Name);
                var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute?.Name;

                if (!string.IsNullOrEmpty(alias)) sb.Append($"{alias}.");

                sb.Append(string.IsNullOrEmpty(columnName) ? $"[{property.Name}], " : $"[{columnName}] AS [{property.Name}], ");
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
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

            var sb = new StringBuilder();

            foreach (var binding in memberInitExpr.Bindings)
            {
                if (!(binding is MemberAssignment memberAssignment)) throw new ArgumentException("Must be member assignment.");

                var columnAttribute = memberAssignment.Member.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute?.Name ?? memberAssignment.Member.Name;
                var parameterName = CreateUniqueParameterName(memberAssignment.Member.Name, parameters);

                if (!string.IsNullOrEmpty(columnAttribute?.TypeName))
                {
                    parameters[parameterName] = CreateDbString(
                        (string)ExtractConstant(memberAssignment.Expression),
                        columnAttribute.TypeName,
                        memberAssignment.Member.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength ?? -1);
                }
                else
                {
                    parameters[parameterName] = ExtractConstant(memberAssignment.Expression);
                }

                if (!string.IsNullOrEmpty(alias)) sb.Append($"{alias}.");

                sb.Append($"[{columnName}] = {GenerateParameterStatement(parameterName, parameters)}, ");
            }

            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }

        public static string ToInsertionStatement<T>(this Expression<Func<T>> me, out IDictionary<string, object> parameters)
        {
            if (!(me.Body is MemberInitExpression memberInitExpr)) throw new ArgumentException("Must be member initializer.");

            parameters = new Dictionary<string, object>();

            var tableType = typeof(T);
            var tableAttribute = tableType.GetCustomAttribute<TableAttribute>();
            var tableName = tableAttribute?.Name ?? tableType.Name;

            var columnListBuilder = new StringBuilder($"INSERT INTO [{tableName}](");
            var valuesBuilder = new StringBuilder(" VALUES (");

            foreach (var binding in memberInitExpr.Bindings)
            {
                if (!(binding is MemberAssignment memberAssignment)) throw new ArgumentException("Must be member assignment.");

                var columnAttribute = memberAssignment.Member.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute?.Name ?? memberAssignment.Member.Name;
                var parameterName = CreateUniqueParameterName(memberAssignment.Member.Name, parameters);

                if (!string.IsNullOrEmpty(columnAttribute?.TypeName))
                {
                    parameters[parameterName] = CreateDbString(
                        (string)ExtractConstant(memberAssignment.Expression),
                        columnAttribute.TypeName,
                        memberAssignment.Member.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength ?? -1);
                }
                else
                {
                    parameters[parameterName] = ExtractConstant(memberAssignment.Expression);
                }

                columnListBuilder.Append($"[{columnName}], ");
                valuesBuilder.Append($"{GenerateParameterStatement(parameterName, parameters)}, ");
            }

            columnListBuilder.Remove(columnListBuilder.Length - 2, 2);
            valuesBuilder.Remove(valuesBuilder.Length - 2, 2);

            columnListBuilder.Append(")");
            valuesBuilder.Append(")");

            return string.Concat(columnListBuilder.ToString(), valuesBuilder.ToString());
        }

        public static string ToSelectionStatement<T>(this Expression<Func<T, bool>> me, out IDictionary<string, object> parameters)
        {
            return ToSelectionStatement(me, null, string.Empty, "WITH (NOLOCK)", out parameters);
        }

        public static string ToSelectionStatement<T>(
            this Expression<Func<T, bool>> me,
            Expression<Func<T, object>> selectList,
            out IDictionary<string, object> parameters)
        {
            return ToSelectionStatement(me, selectList, string.Empty, "WITH (NOLOCK)", out parameters);
        }

        public static string ToSelectionStatement<T>(
            this Expression<Func<T, bool>> me,
            Expression<Func<T, object>> selectList,
            string alias,
            out IDictionary<string, object> parameters)
        {
            return ToSelectionStatement(me, selectList, alias, "WITH (NOLOCK)", out parameters);
        }

        public static string ToSelectionStatement<T>(
            this Expression<Func<T, bool>> me,
            Expression<Func<T, object>> selectList,
            string alias,
            string hint,
            out IDictionary<string, object> parameters)
        {
            var tableType = typeof(T);
            var tableAttribute = tableType.GetCustomAttribute<TableAttribute>();
            var tableName = $"[{tableAttribute?.Name ?? tableType.Name}]";

            var sb = new StringBuilder();

            sb.Append("SELECT ");

            if (selectList != null)
            {
                sb.Append(ToSelectList(selectList, string.IsNullOrEmpty(alias) ? tableName : alias));
            }
            else
            {
                sb.Append(string.IsNullOrEmpty(alias) ? tableName : alias);
                sb.Append(".*");
            }

            sb.Append($" FROM {tableName}");
            sb.Append(string.IsNullOrEmpty(alias) ? string.Empty : $" {alias}");
            sb.Append(string.IsNullOrEmpty(hint) ? string.Empty : $" {hint}");
            sb.Append($" WHERE {ToSearchCondition(me, string.IsNullOrEmpty(alias) ? tableName : alias, out parameters)}");

            return sb.ToString();
        }

        public static string ToUpdateStatement<T>(
            this Expression<Func<T>> me,
            Expression<Func<T, bool>> predicate,
            out IDictionary<string, object> parameters)
        {
            var tableType = typeof(T);
            var tableAttribute = tableType.GetCustomAttribute<TableAttribute>();
            var tableName = $"[{tableAttribute?.Name ?? tableType.Name}]";

            return $"UPDATE {tableName} SET {ToSetStatements(me, out parameters)} WHERE {ToSearchCondition(predicate, parameters)}";
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

        private static void ParseCondition(Expression expr, string alias, StringBuilder sb, IDictionary<string, object> parameters)
        {
            if (expr is BinaryExpression binaryExpr)
            {
                if (binaryExpr.NodeType == ExpressionType.AndAlso || binaryExpr.NodeType == ExpressionType.OrElse)
                {
                    sb.Append("(");

                    ParseCondition(binaryExpr.Left, alias, sb, parameters);

                    switch (binaryExpr.NodeType)
                    {
                        case ExpressionType.AndAlso:
                            sb.Append(") AND (");
                            break;

                        case ExpressionType.OrElse:
                            sb.Append(") OR (");
                            break;
                    }

                    ParseCondition(binaryExpr.Right, alias, sb, parameters);

                    sb.Append(")");
                }
                else
                {
                    if (!(binaryExpr.Left is MemberExpression left))
                    {
                        throw new ArgumentException("Left expression must be MemberExpression.");
                    }

                    if (left.Expression.NodeType != ExpressionType.Parameter)
                    {
                        throw new ArgumentException("Parameter expression must be placed left.");
                    }

                    var columnAttribute = left.Member.GetCustomAttribute<ColumnAttribute>();
                    var columnName = columnAttribute?.Name ?? left.Member.Name;
                    var parameterName = CreateUniqueParameterName(left.Member.Name, parameters);

                    if (!string.IsNullOrEmpty(columnAttribute?.TypeName))
                    {
                        parameters[parameterName] = CreateDbString(
                            (string)ExtractConstant(binaryExpr.Right),
                            columnAttribute.TypeName,
                            left.Member.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength ?? -1);
                    }
                    else
                    {
                        parameters[parameterName] = ExtractConstant(binaryExpr.Right);
                    }

                    if (!string.IsNullOrEmpty(alias)) sb.Append($"{alias}.");

                    sb.Append($"[{columnName}] {MapOperator(binaryExpr.NodeType)} {GenerateParameterStatement(parameterName, parameters)}");
                }
            }
            else if (expr is MethodCallExpression methodCallExpr && methodCallExpr.Method.Name.Equals("Contains"))
            {
                var parameterExpr = (MemberExpression)methodCallExpr.Arguments[1];

                var columnAttribute = parameterExpr.Member.GetCustomAttribute<ColumnAttribute>();
                var columnName = columnAttribute?.Name ?? parameterExpr.Member.Name;

                var array = ExtractArray(methodCallExpr);

                foreach (var item in array)
                {
                    var parameterName = CreateUniqueParameterName(parameterExpr.Member.Name, parameters);

                    if (!string.IsNullOrEmpty(columnAttribute?.TypeName))
                    {
                        parameters[parameterName] = CreateDbString(
                            (string)item,
                            columnAttribute.TypeName,
                            parameterExpr.Member.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength ?? -1);
                    }
                    else
                    {
                        parameters[parameterName] = item;
                    }

                    if (!string.IsNullOrEmpty(alias)) sb.Append($"{alias}.");

                    sb.Append($"[{columnName}] = {GenerateParameterStatement(parameterName, parameters)} OR ");
                }

                sb.Remove(sb.Length - 4, 4);
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
            if (expr is MemberExpression memberExpr)
            {
                if (memberExpr.Member.MemberType == MemberTypes.Field)
                {
                    return ((FieldInfo)memberExpr.Member).GetValue((memberExpr.Expression as ConstantExpression)?.Value);
                }

                if (memberExpr.Member.MemberType == MemberTypes.Property)
                {
                    return ((PropertyInfo)memberExpr.Member).GetValue(ExtractConstant((MemberExpression)memberExpr.Expression));
                }
            }

            if (expr is ConstantExpression constantExpr)
            {
                return constantExpr.Value;
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

        private static string GenerateParameterStatement(string parameterName, IDictionary<string, object> parameters)
        {
            var parameter = parameters[parameterName];

            if (parameter is bool || NumericTypes.Contains(parameter.GetType())) return $"{{={parameterName}}}";

            return $"@{parameterName}";
        }

        private static PropertyInfo[] GetCacheProperties(this Type me)
        {
            if (!PropertyCollection.ContainsKey(me))
            {
                lock (PropertyCollection)
                {
                    if (!PropertyCollection.ContainsKey(me))
                    {
                        PropertyCollection[me] = me.GetProperties();
                    }
                }
            }

            return PropertyCollection[me];
        }
    }
}