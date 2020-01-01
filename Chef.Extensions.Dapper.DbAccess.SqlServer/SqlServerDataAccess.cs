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
using Chef.Extensions.Dapper.DbAccess.SqlServer;
using Chef.Extensions.Dapper.DbAccess.SqlServer.Extensions;
using Dapper;

namespace Chef.Extensions.Dapper.DbAccess
{
    public abstract class SqlServerDataAccess
    {
        protected static readonly Regex ColumnValueRegex = new Regex(@"(\[[^\]]+\]) [^\s]+ ([_0-9a-zA-Z]+\.)?([@\{\[]=?[^,\s\}\)]+(_[\d]+)?\]?\}?)");
        protected static readonly Regex ColumnRegex = new Regex(@"\[[^\]]+\]");

        protected SqlServerDataAccess()
        {
        }
    }

    public abstract class SqlServerDataAccess<T> : SqlServerDataAccess
    {
        private readonly string connectionString;
        private readonly string tableName;
        private readonly string alias;

        protected SqlServerDataAccess(string connectionString)
        {
            this.connectionString = connectionString;

            this.tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;
            this.alias = Regex.Replace(typeof(T).Name, "[^A-Z]", string.Empty).ToLower();
        }

        protected abstract Expression<Func<T, object>> DefaultSelector { get; }

        protected abstract Expression<Func<T>> RequiredColumns { get; }

        public virtual async Task<T> QueryOneAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null)
        {
            if (selector == null && this.DefaultSelector == null)
            {
                throw new NullReferenceException($"At least one of '{nameof(selector)}' and '{nameof(this.DefaultSelector)}' is not null.");
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

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QuerySingleOrDefaultAsync<T>(sql, parameters);

                return result;
            }
        }

        public virtual async Task<List<T>> QueryAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null)
        {
            if (selector == null && this.DefaultSelector == null)
            {
                throw new NullReferenceException($"At least one of '{nameof(selector)}' and '{nameof(this.DefaultSelector)}' is not null.");
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

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T>(sql, parameters);

                return result.ToList();
            }
        }

        public virtual async Task InsertAsync(T value)
        {
            var columnList = this.RequiredColumns.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, value);
            }
        }

        public virtual async Task InsertAsync(Expression<Func<T>> setter)
        {
            var columnList = setter.ToColumnList(out var valueList, out var parameters);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, parameters);
            }
        }

        public virtual async Task InsertAsync(IEnumerable<T> values)
        {
            var columnList = this.RequiredColumns.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.OpenAsync();

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        await db.ExecuteAsync(sql, values, transaction: tx);

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

        public virtual async Task InsertAsync(Expression<Func<T>> setter, IEnumerable<T> values)
        {
            var columnList = setter.ToColumnList(out var valueList);

            var sql = $@"
INSERT INTO {this.tableName}({columnList})
    VALUES ({valueList});";

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.OpenAsync();

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        await db.ExecuteAsync(sql, values, transaction: tx);

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

        public virtual async Task BulkInsertAsync(Expression<Func<T>> setter, IEnumerable<T> values)
        {
            var columnList = setter.ToColumnList(out _);

            SqlBuilder sql = $@"
INSERT INTO {this.tableName}({columnList})
    SELECT {ColumnRegex.Replace(columnList, "$0 = tvp.$0")}
    FROM @TableVariable tvp;";

            var (tableType, tableVariable) = this.ConvertToTableValuedParameters(values);

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, new { TableVariable = tableVariable.AsTableValuedParameter(tableType) });
            }
        }

        public virtual async Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
        {
            SqlBuilder sql = $@"
UPDATE {this.tableName}
SET ";
            sql += setter.ToSetStatements(out var parameters);
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition(parameters);
            sql += ";";

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, parameters);
            }
        }

        public virtual async Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter, IEnumerable<T> values)
        {
            var sql = new SqlBuilder();

            sql += $@"
UPDATE {this.tableName}
SET ";
            sql += setter.ToSetStatements();
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition();
            sql += ";";

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.OpenAsync();

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        await db.ExecuteAsync(sql, values, transaction: tx);

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

        public virtual async Task BulkUpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter, IEnumerable<T> values)
        {
            var columnList = setter.ToColumnList(out _);
            var searchCondition = predicate.ToSearchCondition();

            var sql = $@"
UPDATE {this.tableName}
SET {ColumnRegex.Replace(columnList, "$0 = tvp.$0")}
FROM {this.tableName} t
INNER JOIN @TableVariable tvp
    ON {ColumnValueRegex.Replace(searchCondition, "t.$1 = tvp.$1")};";

            var (tableType, tableVariable) = this.ConvertToTableValuedParameters(values);

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, new { TableVariable = tableVariable.AsTableValuedParameter(tableType) });
            }
        }

        public virtual async Task UpsertAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter)
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

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.OpenAsync();

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        await db.ExecuteAsync(sql, parameters, transaction: tx);

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

        public virtual async Task UpsertAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter, IEnumerable<T> values)
        {
            SqlBuilder sql = $@"
UPDATE {this.tableName}
SET ";
            sql += setter.ToSetStatements();
            sql += @"
WHERE ";
            sql += predicate.ToSearchCondition();
            sql += ";";

            var (columnList, valueList) = ResolveColumnList(sql);

            sql.Append("\r\n");
            sql += $@"
IF @@rowcount = 0
    BEGIN
        INSERT INTO {this.tableName}({columnList})
            VALUES ({valueList});
    END";

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.OpenAsync();

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        await db.ExecuteAsync(sql, values, transaction: tx);

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

        public virtual async Task BulkUpsertAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter, IEnumerable<T> values)
        {
            var columnList = setter.ToColumnList(out _);
            var searchCondition = predicate.ToSearchCondition();

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

            var (tableType, tableVariable) = this.ConvertToTableValuedParameters(values);

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.OpenAsync();

                using (var tx = db.BeginTransaction())
                {
                    try
                    {
                        await db.ExecuteAsync(sql, new { TableVariable = tableVariable.AsTableValuedParameter(tableType) }, transaction: tx);

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

        public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            SqlBuilder sql = $@"
DELETE FROM {this.tableName}
WHERE ";
            sql += predicate.ToSearchCondition(out var parameters);

            using (var db = new SqlConnection(this.connectionString))
            {
                await db.ExecuteAsync(sql, parameters);
            }
        }

        protected abstract (string, DataTable) ConvertToTableValuedParameters(IEnumerable<T> values);

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