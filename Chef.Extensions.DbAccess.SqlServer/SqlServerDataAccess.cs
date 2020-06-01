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

    public class SqlServerDataAccess<T> : SqlServerDataAccess, IDataAccess<T>
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> RequiredColumns = new ConcurrentDictionary<Type, PropertyInfo[]>();
        private static readonly ConcurrentDictionary<string, Delegate> Setters = new ConcurrentDictionary<string, Delegate>();
        private static readonly Regex ServerRegex = new Regex(@"(Server|Data Source)=([^;]+)", RegexOptions.IgnoreCase);
        private static readonly Regex DatabaseRegex = new Regex(@"(Database|Initial Catalog)=([^;]+)", RegexOptions.IgnoreCase);
        private readonly string connectionString;
        private readonly string tableName;
        private readonly string alias;

        public SqlServerDataAccess(string connectionString)
        {
            this.connectionString = connectionString;

            this.tableName = typeof(T).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(T).Name;
            this.alias = GenerateAlias(typeof(T), 1);
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
            var (sql, parameters) = GenerateQueryStatement(this.tableName, this.alias, predicate, orderings, selector, top);

            return this.ExecuteQueryOneAsync<T>(sql, parameters);
        }

        public virtual T QueryOne<TSecond>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            Expression<Func<T, TSecond, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, object>> selector = null,
            int? top = null)
        {
            return this.QueryOneAsync(secondJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<T> QueryOneAsync<TSecond>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            Expression<Func<T, TSecond, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, T>(
                                 sql,
                                 (first, second) =>
                                     {
                                         secondSetter(first, second);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.SingleOrDefault();
            }
        }

        public virtual T QueryOne<TSecond, TThird>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            Expression<Func<T, TSecond, TThird, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, object>> selector = null,
            int? top = null)
        {
            return this.QueryOneAsync(secondJoin, thirdJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<T> QueryOneAsync<TSecond, TThird>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            Expression<Func<T, TSecond, TThird, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, T>(
                                 sql,
                                 (first, second, third) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.SingleOrDefault();
            }
        }

        public virtual T QueryOne<TSecond, TThird, TFourth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, object>> selector = null,
            int? top = null)
        {
            return this.QueryOneAsync(secondJoin, thirdJoin, fourthJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<T> QueryOneAsync<TSecond, TThird, TFourth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                fourthJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, TFourth, T>(
                                 sql,
                                 (first, second, third, fourth) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);
                                         fourthSetter(first, second, third, fourth);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.SingleOrDefault();
            }
        }

        public virtual T QueryOne<TSecond, TThird, TFourth, TFifth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> selector = null,
            int? top = null)
        {
            return this.QueryOneAsync(secondJoin, thirdJoin, fourthJoin, fifthJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<T> QueryOneAsync<TSecond, TThird, TFourth, TFifth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                fourthJoin,
                fifthJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, TFourth, TFifth, T>(
                                 sql,
                                 (first, second, third, fourth, fifth) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);
                                         fourthSetter(first, second, third, fourth);
                                         fifthSetter(first, second, third, fourth, fifth);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.SingleOrDefault();
            }
        }

        public virtual T QueryOne<TSecond, TThird, TFourth, TFifth, TSixth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> selector = null,
            int? top = null)
        {
            return this.QueryOneAsync(secondJoin, thirdJoin, fourthJoin, fifthJoin, sixthJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<T> QueryOneAsync<TSecond, TThird, TFourth, TFifth, TSixth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter, sixthSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                fourthJoin,
                fifthJoin,
                sixthJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, TFourth, TFifth, TSixth, T>(
                                 sql,
                                 (first, second, third, fourth, fifth, sixth) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);
                                         fourthSetter(first, second, third, fourth);
                                         fifthSetter(first, second, third, fourth, fifth);
                                         sixthSetter(first, second, third, fourth, fifth, sixth);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.SingleOrDefault();
            }
        }

        public virtual T QueryOne<TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>, JoinType) seventhJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> selector = null,
            int? top = null)
        {
            return this.QueryOneAsync(secondJoin, thirdJoin, fourthJoin, fifthJoin, sixthJoin, seventhJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<T> QueryOneAsync<TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>, JoinType) seventhJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter, sixthSetter, seventhSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                fourthJoin,
                fifthJoin,
                sixthJoin,
                seventhJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, T>(
                                 sql,
                                 (first, second, third, fourth, fifth, sixth, seventh) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);
                                         fourthSetter(first, second, third, fourth);
                                         fifthSetter(first, second, third, fourth, fifth);
                                         sixthSetter(first, second, third, fourth, fifth, sixth);
                                         seventhSetter(first, second, third, fourth, fifth, sixth, seventh);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.SingleOrDefault();
            }
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
            var (sql, parameters) = GenerateQueryStatement(this.tableName, this.alias, predicate, orderings, selector, top);

            return this.ExecuteQueryAsync<T>(sql, parameters);
        }

        public virtual List<T> Query<TSecond>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            Expression<Func<T, TSecond, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, object>> selector = null,
            int? top = null)
        {
            return this.QueryAsync(secondJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<List<T>> QueryAsync<TSecond>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            Expression<Func<T, TSecond, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, T>(
                                 sql,
                                 (first, second) =>
                                     {
                                         secondSetter(first, second);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.ToList();
            }
        }

        public virtual List<T> Query<TSecond, TThird>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            Expression<Func<T, TSecond, TThird, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, object>> selector = null,
            int? top = null)
        {
            return this.QueryAsync(secondJoin, thirdJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<List<T>> QueryAsync<TSecond, TThird>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            Expression<Func<T, TSecond, TThird, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, T>(
                                 sql,
                                 (first, second, third) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.ToList();
            }
        }

        public virtual List<T> Query<TSecond, TThird, TFourth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, object>> selector = null,
            int? top = null)
        {
            return this.QueryAsync(secondJoin, thirdJoin, fourthJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<List<T>> QueryAsync<TSecond, TThird, TFourth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                fourthJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, TFourth, T>(
                                 sql,
                                 (first, second, third, fourth) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);
                                         fourthSetter(first, second, third, fourth);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.ToList();
            }
        }

        public virtual List<T> Query<TSecond, TThird, TFourth, TFifth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> selector = null,
            int? top = null)
        {
            return this.QueryAsync(secondJoin, thirdJoin, fourthJoin, fifthJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<List<T>> QueryAsync<TSecond, TThird, TFourth, TFifth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                fourthJoin,
                fifthJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, TFourth, TFifth, T>(
                                 sql,
                                 (first, second, third, fourth, fifth) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);
                                         fourthSetter(first, second, third, fourth);
                                         fifthSetter(first, second, third, fourth, fifth);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.ToList();
            }
        }

        public virtual List<T> Query<TSecond, TThird, TFourth, TFifth, TSixth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> selector = null,
            int? top = null)
        {
            return this.QueryAsync(secondJoin, thirdJoin, fourthJoin, fifthJoin, sixthJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<List<T>> QueryAsync<TSecond, TThird, TFourth, TFifth, TSixth>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter, sixthSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                fourthJoin,
                fifthJoin,
                sixthJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, TFourth, TFifth, TSixth, T>(
                                 sql,
                                 (first, second, third, fourth, fifth, sixth) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);
                                         fourthSetter(first, second, third, fourth);
                                         fifthSetter(first, second, third, fourth, fifth);
                                         sixthSetter(first, second, third, fourth, fifth, sixth);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.ToList();
            }
        }

        public virtual List<T> Query<TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>, JoinType) seventhJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> selector = null,
            int? top = null)
        {
            return this.QueryAsync(secondJoin, thirdJoin, fourthJoin, fifthJoin, sixthJoin, seventhJoin, predicate, orderings, selector, top).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual async Task<List<T>> QueryAsync<TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>, JoinType) seventhJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> selector = null,
            int? top = null)
        {
            var (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter, sixthSetter, seventhSetter) = GenerateQueryStatement(
                this.tableName,
                this.alias,
                secondJoin,
                thirdJoin,
                fourthJoin,
                fifthJoin,
                sixthJoin,
                seventhJoin,
                predicate,
                orderings,
                selector,
                top);

            using (var db = new SqlConnection(this.connectionString))
            {
                var result = await db.QueryAsync<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, T>(
                                 sql,
                                 (first, second, third, fourth, fifth, sixth, seventh) =>
                                     {
                                         secondSetter(first, second);
                                         thirdSetter(first, second, third);
                                         fourthSetter(first, second, third, fourth);
                                         fifthSetter(first, second, third, fourth, fifth);
                                         sixthSetter(first, second, third, fourth, fifth, sixth);
                                         seventhSetter(first, second, third, fourth, fifth, sixth, seventh);

                                         return first;
                                     },
                                 parameters,
                                 splitOn: splitOn);

                return result.ToList();
            }
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

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(this.alias, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

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

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(this.alias, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

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

        private static string GenerateAlias(Type type, int suffixNo)
        {
            var alias = Regex.Replace(type.Name, "[^A-Z]", string.Empty).ToLower();

            if (string.IsNullOrEmpty(alias) || alias.Length < 2) alias = type.Name.Left(3).ToLower();

            alias = string.Concat(alias, "_", suffixNo.ToString());

            return alias;
        }

        private static (string, IDictionary<string, object>) GenerateQueryStatement(
            string tableName,
            string alias,
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
                sql += selector.ToSelectList(alias);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {tableName} [{alias}] WITH (NOLOCK)";

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(alias, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

            var orderExpressions = orderings.ToOrderExpressions(alias);

            if (!string.IsNullOrEmpty(orderExpressions))
            {
                sql += @"
ORDER BY ";
                sql += orderExpressions;
            }

            sql += ";";

            return (sql, parameters);
        }

        private static Action<T, TSecond> GetOrCreateSetter<TSecond>(Expression<Func<T, TSecond>> lambdaExpr)
        {
            var memberExpr = (MemberExpression)lambdaExpr.Body;

            if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
            {
                throw new ArgumentException("Member can not applied [NotMapped].");
            }

            var propertyInfo = (PropertyInfo)memberExpr.Member;

            var setterKey = string.Concat("(", typeof(T).FullName, ", ", typeof(TSecond).FullName, ") -> [0]:", propertyInfo.DeclaringType.FullName, ".", propertyInfo.Name);

            var setter = (Action<T, TSecond>)Setters.GetOrAdd(
                setterKey,
                key =>
                    {
                        var instanceParam = Expression.Parameter(propertyInfo.DeclaringType);
                        var argumentParam = Expression.Parameter(propertyInfo.PropertyType);

                        var parameters = new[] { instanceParam, argumentParam };

                        return Expression.Lambda<Action<T, TSecond>>(
                                Expression.Call(
                                    instanceParam,
                                    propertyInfo.GetSetMethod(),
                                    Expression.Convert(argumentParam, propertyInfo.PropertyType)),
                                parameters)
                            .Compile();
                    });

            return setter;
        }

        private static Action<T, TSecond, TThird> GetOrCreateSetter<TSecond, TThird>(Expression<Func<T, TSecond, TThird>> lambdaExpr)
        {
            var memberExpr = (MemberExpression)lambdaExpr.Body;

            if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
            {
                throw new ArgumentException("Member can not applied [NotMapped].");
            }

            var parameterExpr = (ParameterExpression)memberExpr.Expression;

            var argumentIndex = lambdaExpr.Parameters.FindIndex(x => x.Name == parameterExpr.Name);

            var propertyInfo = (PropertyInfo)memberExpr.Member;

            var setterKey = string.Concat("(", typeof(T).FullName, ", ", typeof(TSecond).FullName, ", ", typeof(TThird).FullName, ") -> [", argumentIndex, "]:", propertyInfo.DeclaringType.FullName, ".", propertyInfo.Name);

            var setter = (Action<T, TSecond, TThird>)Setters.GetOrAdd(
                setterKey,
                key =>
                    {
                        var instanceParam = Expression.Parameter(propertyInfo.DeclaringType);
                        var argumentParam = Expression.Parameter(propertyInfo.PropertyType);

                        var parameters = new[]
                                         {
                                             argumentIndex == 0 ? instanceParam : Expression.Parameter(typeof(T)),
                                             argumentIndex == 1 ? instanceParam : Expression.Parameter(typeof(TSecond)),
                                             argumentParam
                                         };

                        return Expression.Lambda<Action<T, TSecond, TThird>>(
                                Expression.Call(
                                    instanceParam,
                                    propertyInfo.GetSetMethod(),
                                    Expression.Convert(argumentParam, propertyInfo.PropertyType)),
                                parameters)
                            .Compile();
                    });

            return setter;
        }

        private static Action<T, TSecond, TThird, TFourth> GetOrCreateSetter<TSecond, TThird, TFourth>(Expression<Func<T, TSecond, TThird, TFourth>> lambdaExpr)
        {
            var memberExpr = (MemberExpression)lambdaExpr.Body;

            if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
            {
                throw new ArgumentException("Member can not applied [NotMapped].");
            }

            var parameterExpr = (ParameterExpression)memberExpr.Expression;

            var argumentIndex = lambdaExpr.Parameters.FindIndex(x => x.Name == parameterExpr.Name);

            var propertyInfo = (PropertyInfo)memberExpr.Member;

            var setterKey = string.Concat("(", typeof(T).FullName, ", ", typeof(TSecond).FullName, ", ", typeof(TThird).FullName, ", ", typeof(TFourth).FullName, ") -> [", argumentIndex, "]:", propertyInfo.DeclaringType.FullName, ".", propertyInfo.Name);

            var setter = (Action<T, TSecond, TThird, TFourth>)Setters.GetOrAdd(
                setterKey,
                key =>
                    {
                        var instanceParam = Expression.Parameter(propertyInfo.DeclaringType);
                        var argumentParam = Expression.Parameter(propertyInfo.PropertyType);

                        var parameters = new[]
                                         {
                                             argumentIndex == 0 ? instanceParam : Expression.Parameter(typeof(T)),
                                             argumentIndex == 1 ? instanceParam : Expression.Parameter(typeof(TSecond)),
                                             argumentIndex == 2 ? instanceParam : Expression.Parameter(typeof(TThird)),
                                             argumentParam
                                         };

                        return Expression.Lambda<Action<T, TSecond, TThird, TFourth>>(
                                Expression.Call(
                                    instanceParam,
                                    propertyInfo.GetSetMethod(),
                                    Expression.Convert(argumentParam, propertyInfo.PropertyType)),
                                parameters)
                            .Compile();
                    });

            return setter;
        }

        private static Action<T, TSecond, TThird, TFourth, TFifth> GetOrCreateSetter<TSecond, TThird, TFourth, TFifth>(Expression<Func<T, TSecond, TThird, TFourth, TFifth>> lambdaExpr)
        {
            var memberExpr = (MemberExpression)lambdaExpr.Body;

            if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
            {
                throw new ArgumentException("Member can not applied [NotMapped].");
            }

            var parameterExpr = (ParameterExpression)memberExpr.Expression;

            var argumentIndex = lambdaExpr.Parameters.FindIndex(x => x.Name == parameterExpr.Name);

            var propertyInfo = (PropertyInfo)memberExpr.Member;

            var setterKey = string.Concat("(", typeof(T).FullName, ", ", typeof(TSecond).FullName, ", ", typeof(TThird).FullName, ", ", typeof(TFourth).FullName, ", ", typeof(TFifth).FullName, ") -> [", argumentIndex, "]:", propertyInfo.DeclaringType.FullName, ".", propertyInfo.Name);

            var setter = (Action<T, TSecond, TThird, TFourth, TFifth>)Setters.GetOrAdd(
                setterKey,
                key =>
                    {
                        var instanceParam = Expression.Parameter(propertyInfo.DeclaringType);
                        var argumentParam = Expression.Parameter(propertyInfo.PropertyType);

                        var parameters = new[]
                                         {
                                             argumentIndex == 0 ? instanceParam : Expression.Parameter(typeof(T)),
                                             argumentIndex == 1 ? instanceParam : Expression.Parameter(typeof(TSecond)),
                                             argumentIndex == 2 ? instanceParam : Expression.Parameter(typeof(TThird)),
                                             argumentIndex == 3 ? instanceParam : Expression.Parameter(typeof(TFourth)),
                                             argumentParam
                                         };

                        return Expression.Lambda<Action<T, TSecond, TThird, TFourth, TFifth>>(
                                Expression.Call(
                                    instanceParam,
                                    propertyInfo.GetSetMethod(),
                                    Expression.Convert(argumentParam, propertyInfo.PropertyType)),
                                parameters)
                            .Compile();
                    });

            return setter;
        }

        private static Action<T, TSecond, TThird, TFourth, TFifth, TSixth> GetOrCreateSetter<TSecond, TThird, TFourth, TFifth, TSixth>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>> lambdaExpr)
        {
            var memberExpr = (MemberExpression)lambdaExpr.Body;

            if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
            {
                throw new ArgumentException("Member can not applied [NotMapped].");
            }

            var parameterExpr = (ParameterExpression)memberExpr.Expression;

            var argumentIndex = lambdaExpr.Parameters.FindIndex(x => x.Name == parameterExpr.Name);

            var propertyInfo = (PropertyInfo)memberExpr.Member;

            var setterKey = string.Concat("(", typeof(T).FullName, ", ", typeof(TSecond).FullName, ", ", typeof(TThird).FullName, ", ", typeof(TFourth).FullName, ", ", typeof(TFifth).FullName, ", ", typeof(TSixth).FullName, ") -> [", argumentIndex, "]:", propertyInfo.DeclaringType.FullName, ".", propertyInfo.Name);

            var setter = (Action<T, TSecond, TThird, TFourth, TFifth, TSixth>)Setters.GetOrAdd(
                setterKey,
                key =>
                    {
                        var instanceParam = Expression.Parameter(propertyInfo.DeclaringType);
                        var argumentParam = Expression.Parameter(propertyInfo.PropertyType);

                        var parameters = new[]
                                         {
                                             argumentIndex == 0 ? instanceParam : Expression.Parameter(typeof(T)),
                                             argumentIndex == 1 ? instanceParam : Expression.Parameter(typeof(TSecond)),
                                             argumentIndex == 2 ? instanceParam : Expression.Parameter(typeof(TThird)),
                                             argumentIndex == 3 ? instanceParam : Expression.Parameter(typeof(TFourth)),
                                             argumentIndex == 4 ? instanceParam : Expression.Parameter(typeof(TFifth)),
                                             argumentParam
                                         };

                        return Expression.Lambda<Action<T, TSecond, TThird, TFourth, TFifth, TSixth>>(
                                Expression.Call(
                                    instanceParam,
                                    propertyInfo.GetSetMethod(),
                                    Expression.Convert(argumentParam, propertyInfo.PropertyType)),
                                parameters)
                            .Compile();
                    });

            return setter;
        }

        private static Action<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GetOrCreateSetter<TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>> lambdaExpr)
        {
            var memberExpr = (MemberExpression)lambdaExpr.Body;

            if (Attribute.IsDefined(memberExpr.Member, typeof(NotMappedAttribute)))
            {
                throw new ArgumentException("Member can not applied [NotMapped].");
            }

            var parameterExpr = (ParameterExpression)memberExpr.Expression;

            var argumentIndex = lambdaExpr.Parameters.FindIndex(x => x.Name == parameterExpr.Name);

            var propertyInfo = (PropertyInfo)memberExpr.Member;

            var setterKey = string.Concat("(", typeof(T).FullName, ", ", typeof(TSecond).FullName, ", ", typeof(TThird).FullName, ", ", typeof(TFourth).FullName, ", ", typeof(TFifth).FullName, ", ", typeof(TSixth).FullName, ", ", typeof(TSeventh).FullName, ") -> [", argumentIndex, "]:", propertyInfo.DeclaringType.FullName, ".", propertyInfo.Name);

            var setter = (Action<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>)Setters.GetOrAdd(
                setterKey,
                key =>
                    {
                        var instanceParam = Expression.Parameter(propertyInfo.DeclaringType);
                        var argumentParam = Expression.Parameter(propertyInfo.PropertyType);

                        var parameters = new[]
                                         {
                                             argumentIndex == 0 ? instanceParam : Expression.Parameter(typeof(T)),
                                             argumentIndex == 1 ? instanceParam : Expression.Parameter(typeof(TSecond)),
                                             argumentIndex == 2 ? instanceParam : Expression.Parameter(typeof(TThird)),
                                             argumentIndex == 3 ? instanceParam : Expression.Parameter(typeof(TFourth)),
                                             argumentIndex == 4 ? instanceParam : Expression.Parameter(typeof(TFifth)),
                                             argumentIndex == 5 ? instanceParam : Expression.Parameter(typeof(TSixth)),
                                             argumentParam
                                         };

                        return Expression.Lambda<Action<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>>(
                                Expression.Call(
                                    instanceParam,
                                    propertyInfo.GetSetMethod(),
                                    Expression.Convert(argumentParam, propertyInfo.PropertyType)),
                                parameters)
                            .Compile();
                    });

            return setter;
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

        private (string, IDictionary<string, object>, string, Action<T, TSecond>) GenerateQueryStatement<TSecond>(
            string tableName,
            string alias,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            Expression<Func<T, TSecond, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, object>> selector = null,
            int? top = null)
        {
            var aliases = new[] { alias, GenerateAlias(typeof(TSecond), 2) };

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;

            string splitOn;

            if (selector != null)
            {
                sql += selector.ToSelectList(aliases, out splitOn);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {tableName} [{alias}] WITH (NOLOCK)";

            sql += this.GenerateJoinStatement<TSecond>(secondJoin.Item2, secondJoin.Item3, aliases);

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(aliases, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

            var orderExpressions = orderings.ToOrderExpressions(aliases);

            if (!string.IsNullOrEmpty(orderExpressions))
            {
                sql += @"
ORDER BY ";
                sql += orderExpressions;
            }

            sql += ";";

            var secondSetter = GetOrCreateSetter(secondJoin.Item1);

            return (sql, parameters, splitOn, secondSetter);
        }

        private (string, IDictionary<string, object>, string, Action<T, TSecond>, Action<T, TSecond, TThird>) GenerateQueryStatement<TSecond, TThird>(
            string tableName,
            string alias,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            Expression<Func<T, TSecond, TThird, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, object>> selector = null,
            int? top = null)
        {
            var aliases = new[] { alias, GenerateAlias(typeof(TSecond), 2), GenerateAlias(typeof(TThird), 3) };

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;

            string splitOn;

            if (selector != null)
            {
                sql += selector.ToSelectList(aliases, out splitOn);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {tableName} [{alias}] WITH (NOLOCK)";

            sql += this.GenerateJoinStatement<TSecond>(secondJoin.Item2, secondJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TThird>(thirdJoin.Item2, thirdJoin.Item3, aliases);

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(aliases, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

            var orderExpressions = orderings.ToOrderExpressions(aliases);

            if (!string.IsNullOrEmpty(orderExpressions))
            {
                sql += @"
ORDER BY ";
                sql += orderExpressions;
            }

            sql += ";";

            var secondSetter = GetOrCreateSetter(secondJoin.Item1);
            var thirdSetter = GetOrCreateSetter(thirdJoin.Item1);

            return (sql, parameters, splitOn, secondSetter, thirdSetter);
        }

        private (string, IDictionary<string, object>, string, Action<T, TSecond>, Action<T, TSecond, TThird>, Action<T, TSecond, TThird, TFourth>) GenerateQueryStatement<TSecond, TThird, TFourth>(
            string tableName,
            string alias,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, object>> selector = null,
            int? top = null)
        {
            var aliases = new[] { alias, GenerateAlias(typeof(TSecond), 2), GenerateAlias(typeof(TThird), 3), GenerateAlias(typeof(TFourth), 4) };

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;

            string splitOn;

            if (selector != null)
            {
                sql += selector.ToSelectList(aliases, out splitOn);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {tableName} [{alias}] WITH (NOLOCK)";

            sql += this.GenerateJoinStatement<TSecond>(secondJoin.Item2, secondJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TThird>(thirdJoin.Item2, thirdJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TFourth>(fourthJoin.Item2, fourthJoin.Item3, aliases);

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(aliases, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

            var orderExpressions = orderings.ToOrderExpressions(aliases);

            if (!string.IsNullOrEmpty(orderExpressions))
            {
                sql += @"
ORDER BY ";
                sql += orderExpressions;
            }

            sql += ";";

            var secondSetter = GetOrCreateSetter(secondJoin.Item1);
            var thirdSetter = GetOrCreateSetter(thirdJoin.Item1);
            var fourthSetter = GetOrCreateSetter(fourthJoin.Item1);

            return (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter);
        }

        private (string, IDictionary<string, object>, string, Action<T, TSecond>, Action<T, TSecond, TThird>, Action<T, TSecond, TThird, TFourth>, Action<T, TSecond, TThird, TFourth, TFifth>) GenerateQueryStatement<TSecond, TThird, TFourth, TFifth>(
            string tableName,
            string alias,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> selector = null,
            int? top = null)
        {
            var aliases = new[] { alias, GenerateAlias(typeof(TSecond), 2), GenerateAlias(typeof(TThird), 3), GenerateAlias(typeof(TFourth), 4), GenerateAlias(typeof(TFifth), 5) };

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;

            string splitOn;

            if (selector != null)
            {
                sql += selector.ToSelectList(aliases, out splitOn);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {tableName} [{alias}] WITH (NOLOCK)";

            sql += this.GenerateJoinStatement<TSecond>(secondJoin.Item2, secondJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TThird>(thirdJoin.Item2, thirdJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TFourth>(fourthJoin.Item2, fourthJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TFifth>(fifthJoin.Item2, fifthJoin.Item3, aliases);

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(aliases, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

            var orderExpressions = orderings.ToOrderExpressions(aliases);

            if (!string.IsNullOrEmpty(orderExpressions))
            {
                sql += @"
ORDER BY ";
                sql += orderExpressions;
            }

            sql += ";";

            var secondSetter = GetOrCreateSetter(secondJoin.Item1);
            var thirdSetter = GetOrCreateSetter(thirdJoin.Item1);
            var fourthSetter = GetOrCreateSetter(fourthJoin.Item1);
            var fifthSetter = GetOrCreateSetter(fifthJoin.Item1);

            return (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter);
        }

        private (string, IDictionary<string, object>, string, Action<T, TSecond>, Action<T, TSecond, TThird>, Action<T, TSecond, TThird, TFourth>, Action<T, TSecond, TThird, TFourth, TFifth>, Action<T, TSecond, TThird, TFourth, TFifth, TSixth>) GenerateQueryStatement<TSecond, TThird, TFourth, TFifth, TSixth>(
            string tableName,
            string alias,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> selector = null,
            int? top = null)
        {
            var aliases = new[] { alias, GenerateAlias(typeof(TSecond), 2), GenerateAlias(typeof(TThird), 3), GenerateAlias(typeof(TFourth), 4), GenerateAlias(typeof(TFifth), 5), GenerateAlias(typeof(TSixth), 6) };

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;

            string splitOn;

            if (selector != null)
            {
                sql += selector.ToSelectList(aliases, out splitOn);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {tableName} [{alias}] WITH (NOLOCK)";

            sql += this.GenerateJoinStatement<TSecond>(secondJoin.Item2, secondJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TThird>(thirdJoin.Item2, thirdJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TFourth>(fourthJoin.Item2, fourthJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TFifth>(fifthJoin.Item2, fifthJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TSixth>(sixthJoin.Item2, sixthJoin.Item3, aliases);

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(aliases, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

            var orderExpressions = orderings.ToOrderExpressions(aliases);

            if (!string.IsNullOrEmpty(orderExpressions))
            {
                sql += @"
ORDER BY ";
                sql += orderExpressions;
            }

            sql += ";";

            var secondSetter = GetOrCreateSetter(secondJoin.Item1);
            var thirdSetter = GetOrCreateSetter(thirdJoin.Item1);
            var fourthSetter = GetOrCreateSetter(fourthJoin.Item1);
            var fifthSetter = GetOrCreateSetter(fifthJoin.Item1);
            var sixthSetter = GetOrCreateSetter(sixthJoin.Item1);

            return (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter, sixthSetter);
        }

        private (string, IDictionary<string, object>, string, Action<T, TSecond>, Action<T, TSecond, TThird>, Action<T, TSecond, TThird, TFourth>, Action<T, TSecond, TThird, TFourth, TFifth>, Action<T, TSecond, TThird, TFourth, TFifth, TSixth>, Action<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>) GenerateQueryStatement<TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            string tableName,
            string alias,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>, JoinType) seventhJoin,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> predicate,
            IEnumerable<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>>, Sortord)> orderings = null,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> selector = null,
            int? top = null)
        {
            var aliases = new[] { alias, GenerateAlias(typeof(TSecond), 2), GenerateAlias(typeof(TThird), 3), GenerateAlias(typeof(TFourth), 4), GenerateAlias(typeof(TFifth), 5), GenerateAlias(typeof(TSixth), 6), GenerateAlias(typeof(TSeventh), 7) };

            SqlBuilder sql = @"
SELECT ";
            sql += top.HasValue ? $"TOP ({top})" : string.Empty;

            string splitOn;

            if (selector != null)
            {
                sql += selector.ToSelectList(aliases, out splitOn);
            }
            else
            {
                throw new ArgumentException("Must be at least one column selected.");
            }

            sql += $@"
FROM {tableName} [{alias}] WITH (NOLOCK)";

            sql += this.GenerateJoinStatement<TSecond>(secondJoin.Item2, secondJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TThird>(thirdJoin.Item2, thirdJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TFourth>(fourthJoin.Item2, fourthJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TFifth>(fifthJoin.Item2, fifthJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TSixth>(sixthJoin.Item2, sixthJoin.Item3, aliases);
            sql += this.GenerateJoinStatement<TSeventh>(seventhJoin.Item2, seventhJoin.Item3, aliases);

            var parameters = new Dictionary<string, object>();

            var searchCondition = predicate == null ? string.Empty : predicate.ToSearchCondition(aliases, parameters);

            if (!string.IsNullOrEmpty(searchCondition))
            {
                sql += @"
WHERE ";
                sql += searchCondition;
            }

            var orderExpressions = orderings.ToOrderExpressions(aliases);

            if (!string.IsNullOrEmpty(orderExpressions))
            {
                sql += @"
ORDER BY ";
                sql += orderExpressions;
            }

            sql += ";";

            var secondSetter = GetOrCreateSetter(secondJoin.Item1);
            var thirdSetter = GetOrCreateSetter(thirdJoin.Item1);
            var fourthSetter = GetOrCreateSetter(fourthJoin.Item1);
            var fifthSetter = GetOrCreateSetter(fifthJoin.Item1);
            var sixthSetter = GetOrCreateSetter(sixthJoin.Item1);
            var seventhSetter = GetOrCreateSetter(seventhJoin.Item1);

            return (sql, parameters, splitOn, secondSetter, thirdSetter, fourthSetter, fifthSetter, sixthSetter, seventhSetter);
        }

        private string GenerateJoinStatement<TRight>(LambdaExpression condition, JoinType joinType, string[] aliases)
        {
            if (condition == null)
            {
                throw new ArgumentException("Must have join condition.");
            }

            var server = ServerRegex.Match(this.connectionString).Groups[2].Value.Trim();
            var database = DatabaseRegex.Match(this.connectionString).Groups[2].Value.Trim();

            ConnectionStringAttribute rightConnectionStringAttr = null;
            string rightConnectionString = null;

            foreach (var connectionStringAttr in typeof(TRight).GetCustomAttributes<ConnectionStringAttribute>())
            {
                rightConnectionString = SqlServerDataAccessFactory.Instance.GetConnectionString(connectionStringAttr.ConnectionString);

                if (rightConnectionString.ToLower().Contains(server.ToLower()))
                {
                    rightConnectionStringAttr = connectionStringAttr;

                    break;
                }
            }

            if (rightConnectionStringAttr == null) throw new ArgumentException("Table is not in the same database server.");

            var rightDatabase = DatabaseRegex.Match(rightConnectionString).Groups[2].Value.Trim();

            switch (joinType)
            {
                case JoinType.Inner:
                    return string.Equals(database, rightDatabase, StringComparison.CurrentCultureIgnoreCase)
                               ? string.Concat("\r\n", condition.ToInnerJoin<TRight>(aliases, null, null))
                               : string.Concat("\r\n", condition.ToInnerJoin<TRight>(aliases, rightDatabase, rightConnectionStringAttr.Schema));

                case JoinType.Left: return string.Concat("\r\n", condition.ToLeftJoin<TRight>(aliases, null, null));

                default: throw new ArgumentOutOfRangeException(nameof(joinType), "Unsupported join type.");
            }
        }

        private (string, DataTable) ConvertToTableValuedParameters(IEnumerable<T> values)
        {
            var (tableType, columns) = SqlServerDataAccessFactory.Instance.GetUserDefinedTable<T>();

            if (string.IsNullOrEmpty(tableType) || columns == null)
            {
                throw new ArgumentException("Must add UserDefinedTableType.");
            }

            var dataTable = new DataTable();

            dataTable.Columns.AddRange(columns.ToArray());

            var properties = columns.ToDictionary(
                x => x.ColumnName,
                x =>
                    {
                        var property = typeof(T).GetProperty(x.ColumnName);

                        if (property == null)
                        {
                            property = typeof(T).GetProperties().Single(p => p.GetCustomAttribute<ColumnAttribute>()?.Name == x.ColumnName);
                        }

                        return property;
                    });

            foreach (var value in values)
            {
                var dataRow = dataTable.NewRow();

                foreach (var dataColumn in columns)
                {
                    dataRow[dataColumn.ColumnName] = properties[dataColumn.ColumnName].GetValue(value);
                }

                dataTable.Rows.Add(dataRow);
            }

            return (tableType, dataTable);
        }
    }
}