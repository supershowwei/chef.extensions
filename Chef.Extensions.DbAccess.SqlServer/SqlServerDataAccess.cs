using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using Chef.Extensions.Dapper;
using Chef.Extensions.DbAccess.SqlServer;
using Chef.Extensions.DbAccess.SqlServer.Extensions;
using Dapper;

namespace Chef.Extensions.DbAccess
{
    public abstract class SqlServerDataAccess
    {
        protected static readonly Regex ColumnValueRegex = new Regex(@"(\[[^\]]+\]) [^\s]+ ([_0-9a-zA-Z]+\.)?([@\{\[]=?[^,\s\}\)]+(_[\d]+)?\]?\}?)");
        protected static readonly Regex ColumnRegex = new Regex(@"\[[^\]]+\]");

        protected SqlServerDataAccess()
        {
        }
    }

    public abstract class SqlServerDataAccess<T> : SqlServerDataAccess, IDataAccess<T>
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> RequiredColumns = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private readonly string connectionString;
        private readonly string tableName;
        private readonly string alias;

        protected SqlServerDataAccess(string connectionString)
        {
            this.connectionString = connectionString;

            this.tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;
            this.alias = Regex.Replace(typeof(T).Name, "[^A-Z]", string.Empty).ToLower();

            if (string.IsNullOrEmpty(this.alias)) this.alias = typeof(T).Name.Left(3);
        }

        public virtual T QueryOne(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null)
        {
            return this.QueryOneAsync(predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<T> QueryOneAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null)
        {
            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;

            if (selector != null)
            {
                sql += selector.ToSelectList(this.alias);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {this.tableName} [{this.alias}] WITH (NOLOCK)";
            sql += predicate.ToWhereStatement(this.alias, out var parameters);
            sql += orderings.ToOrderByStatement(this.alias);
            sql += ";";

            return this.ExecuteQueryOneAsync<T>(sql, parameters);
        }

        public virtual List<T> Query(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null)
        {
            return this.QueryAsync(predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<List<T>> QueryAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null)
        {
            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;

            if (selector != null)
            {
                sql += selector.ToSelectList(this.alias);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {this.tableName} [{this.alias}] WITH (NOLOCK)";
            sql += predicate.ToWhereStatement(this.alias, out var parameters);
            sql += orderings.ToOrderByStatement(this.alias);
            sql += ";";

            return this.ExecuteQueryAsync<T>(sql, parameters);
        }

        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return this.CountAsync(predicate).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder sql = $@"
SELECT COUNT(*)
FROM {this.tableName} [{this.alias}] WITH (NOLOCK)";
            sql += predicate.ToWhereStatement(this.alias, out var parameters);
            sql += ";";

            return this.ExecuteQueryOneAsync<int>(sql, parameters);
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return this.ExistsAsync(predicate).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder sql = $@"
SELECT
    CAST(CASE
        WHEN
            EXISTS (SELECT
                    1
                FROM {this.tableName} [{this.alias}] WITH (NOLOCK)";
            sql += predicate.ToWhereStatement(this.alias, out var parameters);
            sql += @") THEN 1
        ELSE 0
    END AS BIT);";

            return this.ExecuteQueryOneAsync<bool>(sql, parameters);
        }

        public virtual int Insert(T value)
        {
            return this.InsertAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> InsertAsync(T value)
        {
            var requiredColumns = RequiredColumns.GetOrAdd(
                typeof(T),
                type => type.GetProperties().Where(p => Attribute.IsDefined(p, typeof(RequiredAttribute))).ToArray());

            if (requiredColumns.Length == 0) throw new ArgumentException("There must be at least one [Required] column.");

            var columnList = requiredColumns.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            return this.ExecuteCommandAsync(sql, value);
        }

        public virtual int Insert(Expression<Func<T>> setter)
        {
            return this.InsertAsync(setter).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> InsertAsync(Expression<Func<T>> setter)
        {
            var columnList = setter.ToColumnList(out var valueList, out var parameters);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            return this.ExecuteCommandAsync(sql, parameters);
        }

        public virtual int Insert(IEnumerable<T> values)
        {
            return this.InsertAsync(values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> InsertAsync(IEnumerable<T> values)
        {
            var requiredColumns = RequiredColumns.GetOrAdd(
                typeof(T),
                type => type.GetProperties().Where(p => Attribute.IsDefined(p, typeof(RequiredAttribute))).ToArray());

            if (requiredColumns.Length == 0) throw new ArgumentException("There must be at least one [Required] column.");

            var columnList = requiredColumns.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            return Transaction.Current != null ? this.ExecuteCommandAsync(sql, values) : this.ExecuteTransactionalCommandAsync(sql, values);
        }

        public virtual int Insert(Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            return this.InsertAsync(setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> InsertAsync(Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            var columnList = setterTemplate.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            return Transaction.Current != null ? this.ExecuteCommandAsync(sql, values) : this.ExecuteTransactionalCommandAsync(sql, values);
        }

        public virtual int BulkInsert(IEnumerable<T> values)
        {
            return this.BulkInsertAsync(values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> BulkInsertAsync(IEnumerable<T> values)
        {
            var (tableType, tableVariable) = this.ConvertToTableValuedParameters(values);

            if (tableType == null || tableVariable == null)
            {
                throw new NullReferenceException($"If want to use Bulk- related methods, must override '{nameof(this.ConvertToTableValuedParameters)}' method to create table value for User Defined Table Types.");
            }

            var requiredColumns = RequiredColumns.GetOrAdd(
                typeof(T),
                type => type.GetProperties().Where(p => Attribute.IsDefined(p, typeof(RequiredAttribute))).ToArray());

            if (requiredColumns.Length == 0) throw new ArgumentException("There must be at least one [Required] column.");

            var columnList = requiredColumns.ToColumnList(out _);

            SqlBuilder sql = $@"
INSERT INTO {this.tableName}({columnList})
    SELECT {ColumnRegex.Replace(columnList, "$0 = tvp.$0")}
    FROM @TableVariable tvp;";

            return this.ExecuteCommandAsync(sql, new { TableVariable = tableVariable.AsTableValuedParameter(tableType) });
        }

        public virtual int BulkInsert(Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            return this.BulkInsertAsync(setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> BulkInsertAsync(Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            var (tableType, tableVariable) = this.ConvertToTableValuedParameters(values);

            if (tableType == null || tableVariable == null)
            {
                throw new NullReferenceException($"If want to use Bulk- related methods, must override '{nameof(this.ConvertToTableValuedParameters)}' method to create table value for User Defined Table Types.");
            }

            var columnList = setterTemplate.ToColumnList(out _);

            SqlBuilder sql = $@"
INSERT INTO {this.tableName}({columnList})
    SELECT {ColumnRegex.Replace(columnList, "$0 = tvp.$0")}
    FROM @TableVariable tvp;";

            return this.ExecuteCommandAsync(sql, new { TableVariable = tableVariable.AsTableValuedParameter(tableType) });
        }

        public virtual int Update(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
        {
            return this.UpdateAsync(predicate, setter).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
        {
            SqlBuilder sql = $@"
UPDATE {this.tableName}
SET ";
            sql += setter.ToSetStatements(out var parameters);
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition(parameters);
            sql += ";";

            return this.ExecuteCommandAsync(sql, parameters);
        }

        public virtual int Update(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            return this.UpdateAsync(predicateTemplate, setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> UpdateAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            var sql = new SqlBuilder();

            sql += $@"
UPDATE {this.tableName}
SET ";
            sql += setterTemplate.ToSetStatements();
            sql += @"
WHERE ";
            sql += predicateTemplate.ToSearchCondition();
            sql += ";";

            return Transaction.Current != null ? this.ExecuteCommandAsync(sql, values) : this.ExecuteTransactionalCommandAsync(sql, values);
        }

        public virtual int BulkUpdate(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            return this.BulkUpdateAsync(predicateTemplate, setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> BulkUpdateAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            var (tableType, tableVariable) = this.ConvertToTableValuedParameters(values);

            if (tableType == null || tableVariable == null)
            {
                throw new NullReferenceException($"If want to use Bulk- related methods, must override '{nameof(this.ConvertToTableValuedParameters)}' method to create table value for User Defined Table Types.");
            }

            var columnList = setterTemplate.ToColumnList(out _);
            var searchCondition = predicateTemplate.ToSearchCondition();

            var sql = $@"
UPDATE {this.tableName}
SET {ColumnRegex.Replace(columnList, "$0 = tvp.$0")}
FROM {this.tableName} t
INNER JOIN @TableVariable tvp
    ON {ColumnValueRegex.Replace(searchCondition, "t.$1 = tvp.$1")};";

            return this.ExecuteCommandAsync(sql, new { TableVariable = tableVariable.AsTableValuedParameter(tableType) });
        }

        public virtual int Upsert(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
        {
            return this.UpsertAsync(predicate, setter).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> UpsertAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
        {
            SqlBuilder sql = $@"
UPDATE {this.tableName}
SET ";
            sql += setter.ToSetStatements(out var parameters);
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition(parameters);
            sql += ";";

            var (columnList, valueList) = ResolveColumnList(sql);

            sql.Append("\r\n");
            sql += $@"
IF @@rowcount = 0
    BEGIN
        INSERT INTO {this.tableName}({columnList})
            VALUES ({valueList});
    END";

            return Transaction.Current != null ? this.ExecuteCommandAsync(sql, parameters) : this.ExecuteTransactionalCommandAsync(sql, parameters);
        }

        public virtual int Upsert(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            return this.UpsertAsync(predicateTemplate, setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> UpsertAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            SqlBuilder sql = $@"
UPDATE {this.tableName}
SET ";
            sql += setterTemplate.ToSetStatements();
            sql += @"
WHERE ";
            sql += predicateTemplate.ToSearchCondition();
            sql += ";";

            var (columnList, valueList) = ResolveColumnList(sql);

            sql.Append("\r\n");
            sql += $@"
IF @@rowcount = 0
    BEGIN
        INSERT INTO {this.tableName}({columnList})
            VALUES ({valueList});
    END";

            return Transaction.Current != null ? this.ExecuteCommandAsync(sql, values) : this.ExecuteTransactionalCommandAsync(sql, values);
        }

        public virtual int BulkUpsert(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            return this.BulkUpsertAsync(predicateTemplate, setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> BulkUpsertAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            var (tableType, tableVariable) = this.ConvertToTableValuedParameters(values);

            if (tableType == null || tableVariable == null)
            {
                throw new NullReferenceException($"If want to use Bulk- related methods, must override '{nameof(this.ConvertToTableValuedParameters)}' method to create table value for User Defined Table Types.");
            }

            var columnList = setterTemplate.ToColumnList(out _);
            var searchCondition = predicateTemplate.ToSearchCondition();

            SqlBuilder sql = $@"
UPDATE {this.tableName}
SET {ColumnRegex.Replace(columnList, "$0 = tvp.$0")}
FROM {this.tableName} t
INNER JOIN @TableVariable tvp
    ON {ColumnValueRegex.Replace(searchCondition, "t.$1 = tvp.$1")};";

            (columnList, _) = ResolveColumnList(sql);

            sql.Append("\r\n");
            sql += $@"
INSERT INTO {this.tableName}({columnList})
    SELECT {ColumnRegex.Replace(columnList, "tvp.$0")}
    FROM @TableVariable tvp
    WHERE NOT EXISTS (SELECT
                1
            FROM {this.tableName} t WITH (NOLOCK)
            WHERE {ColumnValueRegex.Replace(searchCondition, "t.$1 = tvp.$1")});";

            return Transaction.Current != null
                       ? this.ExecuteCommandAsync(sql, new { TableVariable = tableVariable.AsTableValuedParameter(tableType) })
                       : this.ExecuteTransactionalCommandAsync(sql,  new { TableVariable = tableVariable.AsTableValuedParameter(tableType) });
        }

        public virtual int Delete(Expression<Func<T, bool>> predicate)
        {
            return this.DeleteAsync(predicate).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder sql = $@"
DELETE FROM {this.tableName}
WHERE ";
            sql += predicate.ToSearchCondition(out var parameters);

            return this.ExecuteCommandAsync(sql, parameters);
        }

        protected virtual async Task<TResult> ExecuteQueryOneAsync<TResult>(string sql, IDictionary<string, object> parameters)
        {
            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QuerySingleOrDefaultAsync<TResult>(sql, parameters);

                return result;
            }
        }

        protected virtual async Task<List<TResult>> ExecuteQueryAsync<TResult>(string sql, IDictionary<string, object> parameters)
        {
            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<TResult>(sql, parameters);

                return result.ToList();
            }
        }

        protected virtual async Task<int> ExecuteCommandAsync(string sql, object param)
        {
            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.ExecuteAsync(sql, param);

                return result;
            }
        }

        protected virtual async Task<int> ExecuteTransactionalCommandAsync(string sql, object param)
        {
            int result;
            using (var db = new SqlConnection(this.connectionString))
            {
                await db.OpenAsync();

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        result = await db.ExecuteAsync(sql, param, transaction: tx);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return result;
        }

        protected virtual (string, DataTable) ConvertToTableValuedParameters(IEnumerable<T> values)
        {
            return (null, null);
        }

        private static (string, string) ResolveColumnList(string sql)
        {
            var columnList = new Dictionary<string, string>();

            foreach (var match in ColumnValueRegex.Matches(sql).Cast<Match>())
            {
                if (columnList.ContainsKey(match.Groups[1].Value)) continue;

                columnList.Add(match.Groups[1].Value, match.Groups[3].Value);
            }

            return (string.Join(", ", columnList.Keys), string.Join(", ", columnList.Values));
        }
    }
}