using System;
using System.Collections.Generic;
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

        protected virtual Expression<Func<T, object>> DefaultSelector { get; } = null;

        protected virtual Expression<Func<T>> RequiredColumns { get; } = null;

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
            if (selector == null && this.DefaultSelector == null)
            {
                throw new NullReferenceException($"If there is no '{nameof(selector)}', must override '{nameof(this.DefaultSelector)}' property to determine the default selector.");
            }

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;
            sql += (selector ?? this.DefaultSelector).ToSelectList(this.alias);
            sql += $@"
FROM {this.tableName} {this.alias} WITH (NOLOCK)";
            sql += predicate.ToWhereStatement(this.alias, out var parameters);
            sql += orderings.ToOrderByStatement(this.alias);
            sql += ";";

            return this.ExecuteQueryOneAsync(sql, parameters);
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
            if (selector == null && this.DefaultSelector == null)
            {
                throw new NullReferenceException($"If there is no '{nameof(selector)}', must override '{nameof(this.DefaultSelector)}' property to determine the default selector.");
            }

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;
            sql += (selector ?? this.DefaultSelector).ToSelectList(this.alias);
            sql += $@"
FROM {this.tableName} {this.alias} WITH (NOLOCK)";
            sql += predicate.ToWhereStatement(this.alias, out var parameters);
            sql += orderings.ToOrderByStatement(this.alias);
            sql += ";";

            return this.ExecuteQueryAsync(sql, parameters);
        }

        public virtual void Insert(T value)
        {
            this.InsertAsync(value).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task InsertAsync(T value)
        {
            if (this.RequiredColumns == null)
            {
                throw new NullReferenceException($"Must override '{nameof(this.RequiredColumns)}' property to determine the required columns.");
            }

            var columnList = this.RequiredColumns.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            return this.ExecuteCommandAsync(sql, value);
        }

        public virtual void Insert(Expression<Func<T>> setter)
        {
            this.InsertAsync(setter).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task InsertAsync(Expression<Func<T>> setter)
        {
            var columnList = setter.ToColumnList(out var valueList, out var parameters);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            return this.ExecuteCommandAsync(sql, parameters);
        }

        public virtual void Insert(IEnumerable<T> values)
        {
            this.InsertAsync(values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task InsertAsync(IEnumerable<T> values)
        {
            if (this.RequiredColumns == null)
            {
                throw new NullReferenceException($"Must override '{nameof(this.RequiredColumns)}' property to determine the required columns.");
            }

            var columnList = this.RequiredColumns.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            return Transaction.Current != null ? this.ExecuteCommandAsync(sql, values) : this.ExecuteTransactionalCommandAsync(sql, values);
        }

        public virtual void Insert(Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            this.InsertAsync(setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task InsertAsync(Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            var columnList = setterTemplate.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            return Transaction.Current != null ? this.ExecuteCommandAsync(sql, values) : this.ExecuteTransactionalCommandAsync(sql, values);
        }

        public virtual void BulkInsert(Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            this.BulkInsertAsync(setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task BulkInsertAsync(Expression<Func<T>> setterTemplate, IEnumerable<T> values)
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

        public virtual void Update(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
        {
            this.UpdateAsync(predicate, setter).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
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

        public virtual void Update(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            this.UpdateAsync(predicateTemplate, setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task UpdateAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
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

        public virtual void BulkUpdate(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            this.BulkUpdateAsync(predicateTemplate, setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task BulkUpdateAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
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

        public virtual void Upsert(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
        {
            this.UpsertAsync(predicate, setter).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task UpsertAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
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

        public virtual void Upsert(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            this.UpsertAsync(predicateTemplate, setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task UpsertAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
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

        public virtual void BulkUpsert(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
        {
            this.BulkUpsertAsync(predicateTemplate, setterTemplate, values).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task BulkUpsertAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values)
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

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            this.DeleteAsync(predicate).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder sql = $@"
DELETE FROM {this.tableName}
WHERE ";
            sql += predicate.ToSearchCondition(out var parameters);

            return this.ExecuteCommandAsync(sql, parameters);
        }

        protected virtual async Task<T> ExecuteQueryOneAsync(string sql, IDictionary<string, object> parameters)
        {
            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QuerySingleOrDefaultAsync<T>(sql, parameters);

                return result;
            }
        }

        protected virtual async Task<List<T>> ExecuteQueryAsync(string sql, IDictionary<string, object> parameters)
        {
            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T>(sql, parameters);

                return result.ToList();
            }
        }

        protected virtual async Task ExecuteCommandAsync(string sql, object param)
        {
            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, param);
            }
        }

        protected virtual async Task ExecuteTransactionalCommandAsync(string sql, object param)
        {
            using (var db = new SqlConnection(this.connectionString))
            {
                await db.OpenAsync();

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        await db.ExecuteAsync(sql, param, transaction: tx);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
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