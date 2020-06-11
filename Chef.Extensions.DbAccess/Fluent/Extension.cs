using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chef.Extensions.DbAccess.Fluent
{
    public static class Extension
    {
        #region IDataAccess<T>Extension

        public static QueryObject<T> Where<T>(this IDataAccess<T> me, Expression<Func<T, bool>> predicate)
        {
            return new QueryObject<T>(me) { Predicate = predicate };
        }

        public static QueryObject<T> Select<T>(this IDataAccess<T> me, Expression<Func<T, object>> selector)
        {
            return new QueryObject<T>(me) { Selector = selector };
        }

        public static QueryObject<T> Set<T>(this IDataAccess<T> me, Expression<Func<T>> setter)
        {
            return new QueryObject<T>(me) { Setter = setter };
        }

        public static QueryObject<T> OrderBy<T>(this IDataAccess<T> me, Expression<Func<T, object>> ordering)
        {
            return new QueryObject<T>(me)
                   {
                       OrderExpressions = new List<(Expression<Func<T, object>>, Sortord)> { (ordering, Sortord.Ascending) }
                   };
        }

        public static QueryObject<T> OrderByDescending<T>(this IDataAccess<T> me, Expression<Func<T, object>> ordering)
        {
            return new QueryObject<T>(me)
                   {
                       OrderExpressions = new List<(Expression<Func<T, object>>, Sortord)> { (ordering, Sortord.Descending) }
                   };
        }

        public static QueryObject<T> Skip<T>(this IDataAccess<T> me, int n)
        {
            return new QueryObject<T>(me) { Skipped = n };
        }

        public static QueryObject<T> Take<T>(this IDataAccess<T> me, int n)
        {
            return new QueryObject<T>(me) { Taken = n };
        }

        public static QueryObject<T> GroupBy<T>(this IDataAccess<T> me, Expression<Func<T, object>> groupingColumns, Expression<Func<Grouping<T>, T>> groupingSelector)
        {
            return new QueryObject<T>(me) { GroupingColumns = groupingColumns, GroupingSelector = groupingSelector };
        }

        public static QueryObject<T, TSecond> InnerJoin<T, TSecond>(
            this IDataAccess<T> me,
            Expression<Func<T, TSecond>> propertyPath,
            Expression<Func<T, TSecond, bool>> condition)
        {
            return new QueryObject<T, TSecond>(me, (propertyPath, condition, JoinType.Inner));
        }

        public static QueryObject<T, TSecond> LeftJoin<T, TSecond>(
            this IDataAccess<T> me,
            Expression<Func<T, TSecond>> propertyPath,
            Expression<Func<T, TSecond, bool>> condition)
        {
            return new QueryObject<T, TSecond>(me, (propertyPath, condition, JoinType.Left));
        }

        #endregion

        #region QueryObject<T>Extension

        public static QueryObject<T> Where<T>(this QueryObject<T> me, Expression<Func<T, bool>> predicate)
        {
            me.Predicate = predicate;

            return me;
        }

        public static QueryObject<T> And<T>(this QueryObject<T> me, Expression<Func<T, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T> Or<T>(this QueryObject<T> me, Expression<Func<T, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, bool>>(Expression.OrElse(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T> Select<T>(this QueryObject<T> me, Expression<Func<T, object>> selector)
        {
            me.Selector = selector;

            return me;
        }

        public static QueryObject<T> Set<T>(this QueryObject<T> me, Expression<Func<T>> setter)
        {
            me.Setter = setter;

            return me;
        }

        public static QueryObject<T> OrderBy<T>(this QueryObject<T> me, Expression<Func<T, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, object>>, Sortord)> { (ordering, Sortord.Ascending) };

            return me;
        }

        public static QueryObject<T> ThenBy<T>(this QueryObject<T> me, Expression<Func<T, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Ascending));

            return me;
        }

        public static QueryObject<T> OrderByDescending<T>(this QueryObject<T> me, Expression<Func<T, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, object>>, Sortord)> { (ordering, Sortord.Descending) };

            return me;
        }

        public static QueryObject<T> ThenByDescending<T>(this QueryObject<T> me, Expression<Func<T, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Descending));

            return me;
        }

        public static QueryObject<T> Skip<T>(this QueryObject<T> me, int n)
        {
            me.Skipped = n;

            return me;
        }

        public static QueryObject<T> Take<T>(this QueryObject<T> me, int n)
        {
            me.Taken = n;

            return me;
        }

        public static QueryObject<T> GroupBy<T>(this QueryObject<T> me, Expression<Func<T, object>> groupingColumns, Expression<Func<Grouping<T>, T>> groupingSelector)
        {
            me.GroupingColumns = groupingColumns;
            me.GroupingSelector = groupingSelector;

            return me;
        }

        public static T QueryOne<T>(this QueryObject<T> me)
        {
            return me.DataAccess.QueryOne(me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<T> QueryOneAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.QueryOneAsync(me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static List<T> Query<T>(this QueryObject<T> me)
        {
            return me.DataAccess.Query(me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<List<T>> QueryAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.QueryAsync(me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static int Count<T>(this QueryObject<T> me)
        {
            return me.DataAccess.Count(me.Predicate);
        }

        public static Task<int> CountAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.CountAsync(me.Predicate);
        }

        public static bool Exists<T>(this QueryObject<T> me)
        {
            return me.DataAccess.Exists(me.Predicate);
        }

        public static Task<bool> ExistsAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.ExistsAsync(me.Predicate);
        }

        public static int Insert<T>(this QueryObject<T> me)
        {
            return me.DataAccess.Insert(me.Setter);
        }

        public static Task<int> InsertAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.InsertAsync(me.Setter);
        }

        public static int Insert<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.Insert(me.Setter, values);
        }

        public static Task<int> InsertAsync<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.InsertAsync(me.Setter, values);
        }

        public static int BulkInsert<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.BulkInsert(me.Setter, values);
        }

        public static Task<int> BulkInsertAsync<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.BulkInsertAsync(me.Setter, values);
        }

        public static int Update<T>(this QueryObject<T> me)
        {
            return me.DataAccess.Update(me.Predicate, me.Setter);
        }

        public static Task<int> UpdateAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.UpdateAsync(me.Predicate, me.Setter);
        }

        public static int Update<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.Update(me.Predicate, me.Setter, values);
        }

        public static Task<int> UpdateAsync<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.UpdateAsync(me.Predicate, me.Setter, values);
        }

        public static int BulkUpdate<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.Update(me.Predicate, me.Setter, values);
        }

        public static Task<int> BulkUpdateAsync<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.UpdateAsync(me.Predicate, me.Setter, values);
        }

        public static int Upsert<T>(this QueryObject<T> me)
        {
            return me.DataAccess.Upsert(me.Predicate, me.Setter);
        }

        public static Task<int> UpsertAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.UpsertAsync(me.Predicate, me.Setter);
        }

        public static int Upsert<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.Upsert(me.Predicate, me.Setter, values);
        }

        public static Task<int> UpsertAsync<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.UpsertAsync(me.Predicate, me.Setter, values);
        }

        public static int BulkUpsert<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.Upsert(me.Predicate, me.Setter, values);
        }

        public static Task<int> BulkUpsertAsync<T>(this QueryObject<T> me, IEnumerable<T> values)
        {
            return me.DataAccess.UpsertAsync(me.Predicate, me.Setter, values);
        }

        public static int Delete<T>(this QueryObject<T> me)
        {
            return me.DataAccess.Delete(me.Predicate);
        }

        public static Task<int> DeleteAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.DeleteAsync(me.Predicate);
        }

        #endregion

        #region QueryObject<T, TSecond>Extension

        public static QueryObject<T, TSecond, TThird> InnerJoin<T, TSecond, TThird>(
            this QueryObject<T, TSecond> me,
            Expression<Func<T, TSecond, TThird>> propertyPath,
            Expression<Func<T, TSecond, TThird, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird>(me.DataAccess, me.SecondJoin, (propertyPath, condition, JoinType.Inner));
        }

        public static QueryObject<T, TSecond, TThird> LeftJoin<T, TSecond, TThird>(
            this QueryObject<T, TSecond> me,
            Expression<Func<T, TSecond, TThird>> propertyPath,
            Expression<Func<T, TSecond, TThird, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird>(me.DataAccess, me.SecondJoin, (propertyPath, condition, JoinType.Left));
        }

        public static QueryObject<T, TSecond> Where<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, bool>> predicate)
        {
            me.Predicate = predicate;

            return me;
        }

        public static QueryObject<T, TSecond> And<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, bool>>(Expression.AndAlso(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond> Or<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, bool>>(Expression.OrElse(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond> Select<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, object>> selector)
        {
            me.Selector = selector;

            return me;
        }

        public static QueryObject<T, TSecond> OrderBy<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, object>>, Sortord)> { (ordering, Sortord.Ascending) };

            return me;
        }

        public static QueryObject<T, TSecond> ThenBy<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Ascending));

            return me;
        }

        public static QueryObject<T, TSecond> OrderByDescending<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, object>>, Sortord)> { (ordering, Sortord.Descending) };

            return me;
        }

        public static QueryObject<T, TSecond> ThenByDescending<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Descending));

            return me;
        }

        public static QueryObject<T, TSecond> Skip<T, TSecond>(this QueryObject<T, TSecond> me, int n)
        {
            me.Skipped = n;

            return me;
        }

        public static QueryObject<T, TSecond> Take<T, TSecond>(this QueryObject<T, TSecond> me, int n)
        {
            me.Taken = n;

            return me;
        }

        public static QueryObject<T, TSecond> GroupBy<T, TSecond>(this QueryObject<T, TSecond> me, Expression<Func<T, TSecond, object>> groupingColumns, Expression<Func<Grouping<T, TSecond>, T>> groupingSelector)
        {
            me.GroupingColumns = groupingColumns;
            me.GroupingSelector = groupingSelector;

            return me;
        }

        public static T QueryOne<T, TSecond>(this QueryObject<T, TSecond> me)
        {
            return me.DataAccess.QueryOne(me.SecondJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<T> QueryOneAsync<T, TSecond>(this QueryObject<T, TSecond> me)
        {
            return me.DataAccess.QueryOneAsync(me.SecondJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static List<T> Query<T, TSecond>(this QueryObject<T, TSecond> me)
        {
            return me.DataAccess.Query(me.SecondJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<List<T>> QueryAsync<T, TSecond>(this QueryObject<T, TSecond> me)
        {
            return me.DataAccess.QueryAsync(me.SecondJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static int Count<T, TSecond>(this QueryObject<T, TSecond> me)
        {
            return me.DataAccess.Count(me.SecondJoin, me.Predicate);
        }

        public static Task<int> CountAsync<T, TSecond>(this QueryObject<T, TSecond> me)
        {
            return me.DataAccess.CountAsync(me.SecondJoin, me.Predicate);
        }

        #endregion

        #region QueryObject<T, TSecond, TThird>Extension

        public static QueryObject<T, TSecond, TThird, TFourth> InnerJoin<T, TSecond, TThird, TFourth>(
            this QueryObject<T, TSecond, TThird> me,
            Expression<Func<T, TSecond, TThird, TFourth>> propertyPath,
            Expression<Func<T, TSecond, TThird, TFourth, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird, TFourth>(me.DataAccess, me.SecondJoin, me.ThirdJoin, (propertyPath, condition, JoinType.Inner));
        }

        public static QueryObject<T, TSecond, TThird, TFourth> LeftJoin<T, TSecond, TThird, TFourth>(
            this QueryObject<T, TSecond, TThird> me,
            Expression<Func<T, TSecond, TThird, TFourth>> propertyPath,
            Expression<Func<T, TSecond, TThird, TFourth, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird, TFourth>(me.DataAccess, me.SecondJoin, me.ThirdJoin, (propertyPath, condition, JoinType.Left));
        }

        public static QueryObject<T, TSecond, TThird> Where<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, bool>> predicate)
        {
            me.Predicate = predicate;

            return me;
        }

        public static QueryObject<T, TSecond, TThird> And<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, bool>>(Expression.AndAlso(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird> Or<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, bool>>(Expression.OrElse(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird> Select<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, object>> selector)
        {
            me.Selector = selector;

            return me;
        }

        public static QueryObject<T, TSecond, TThird> OrderBy<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, object>>, Sortord)> { (ordering, Sortord.Ascending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird> ThenBy<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Ascending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird> OrderByDescending<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, object>>, Sortord)> { (ordering, Sortord.Descending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird> ThenByDescending<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Descending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird> Skip<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, int n)
        {
            me.Skipped = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird> Take<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, int n)
        {
            me.Taken = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird> GroupBy<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me, Expression<Func<T, TSecond, TThird, object>> groupingColumns, Expression<Func<Grouping<T, TSecond, TThird>, T>> groupingSelector)
        {
            me.GroupingColumns = groupingColumns;
            me.GroupingSelector = groupingSelector;

            return me;
        }

        public static T QueryOne<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me)
        {
            return me.DataAccess.QueryOne(me.SecondJoin, me.ThirdJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<T> QueryOneAsync<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me)
        {
            return me.DataAccess.QueryOneAsync(me.SecondJoin, me.ThirdJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static List<T> Query<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me)
        {
            return me.DataAccess.Query(me.SecondJoin, me.ThirdJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<List<T>> QueryAsync<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me)
        {
            return me.DataAccess.QueryAsync(me.SecondJoin, me.ThirdJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static int Count<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me)
        {
            return me.DataAccess.Count(me.SecondJoin, me.ThirdJoin, me.Predicate);
        }

        public static Task<int> CountAsync<T, TSecond, TThird>(this QueryObject<T, TSecond, TThird> me)
        {
            return me.DataAccess.CountAsync(me.SecondJoin, me.ThirdJoin, me.Predicate);
        }

        #endregion

        #region QueryObject<T, TSecond, TThird, TFourth>Extension

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> InnerJoin<T, TSecond, TThird, TFourth, TFifth>(
            this QueryObject<T, TSecond, TThird, TFourth> me,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth>> propertyPath,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird, TFourth, TFifth>(me.DataAccess, me.SecondJoin, me.ThirdJoin, me.FourthJoin, (propertyPath, condition, JoinType.Inner));
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> LeftJoin<T, TSecond, TThird, TFourth, TFifth>(
            this QueryObject<T, TSecond, TThird, TFourth> me,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth>> propertyPath,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird, TFourth, TFifth>(me.DataAccess, me.SecondJoin, me.ThirdJoin, me.FourthJoin, (propertyPath, condition, JoinType.Left));
        }

        public static QueryObject<T, TSecond, TThird, TFourth> Where<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, bool>> predicate)
        {
            me.Predicate = predicate;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> And<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, TFourth, bool>>(Expression.AndAlso(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> Or<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, TFourth, bool>>(Expression.OrElse(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> Select<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, object>> selector)
        {
            me.Selector = selector;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> OrderBy<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, TFourth, object>>, Sortord)> { (ordering, Sortord.Ascending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> ThenBy<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Ascending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> OrderByDescending<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, TFourth, object>>, Sortord)> { (ordering, Sortord.Descending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> ThenByDescending<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Descending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> Skip<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, int n)
        {
            me.Skipped = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> Take<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, int n)
        {
            me.Taken = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth> GroupBy<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me, Expression<Func<T, TSecond, TThird, TFourth, object>> groupingColumns, Expression<Func<Grouping<T, TSecond, TThird, TFourth>, T>> groupingSelector)
        {
            me.GroupingColumns = groupingColumns;
            me.GroupingSelector = groupingSelector;

            return me;
        }

        public static T QueryOne<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me)
        {
            return me.DataAccess.QueryOne(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<T> QueryOneAsync<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me)
        {
            return me.DataAccess.QueryOneAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static List<T> Query<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me)
        {
            return me.DataAccess.Query(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<List<T>> QueryAsync<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me)
        {
            return me.DataAccess.QueryAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static int Count<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me)
        {
            return me.DataAccess.Count(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.Predicate);
        }

        public static Task<int> CountAsync<T, TSecond, TThird, TFourth>(this QueryObject<T, TSecond, TThird, TFourth> me)
        {
            return me.DataAccess.CountAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.Predicate);
        }

        #endregion

        #region QueryObject<T, TSecond, TThird, TFourth, TFifth>Extension

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> InnerJoin<T, TSecond, TThird, TFourth, TFifth, TSixth>(
            this QueryObject<T, TSecond, TThird, TFourth, TFifth> me,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>> propertyPath,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth>(me.DataAccess, me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, (propertyPath, condition, JoinType.Inner));
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> LeftJoin<T, TSecond, TThird, TFourth, TFifth, TSixth>(
            this QueryObject<T, TSecond, TThird, TFourth, TFifth> me,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>> propertyPath,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth>(me.DataAccess, me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, (propertyPath, condition, JoinType.Left));
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> Where<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> predicate)
        {
            me.Predicate = predicate;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> And<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, TFourth, TFifth, bool>>(Expression.AndAlso(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> Or<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, TFourth, TFifth, bool>>(Expression.OrElse(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> Select<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> selector)
        {
            me.Selector = selector;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> OrderBy<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>>, Sortord)> { (ordering, Sortord.Ascending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> ThenBy<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Ascending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> OrderByDescending<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>>, Sortord)> { (ordering, Sortord.Descending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> ThenByDescending<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Descending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> Skip<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, int n)
        {
            me.Skipped = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> Take<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, int n)
        {
            me.Taken = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth> GroupBy<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> groupingColumns, Expression<Func<Grouping<T, TSecond, TThird, TFourth, TFifth>, T>> groupingSelector)
        {
            me.GroupingColumns = groupingColumns;
            me.GroupingSelector = groupingSelector;

            return me;
        }

        public static T QueryOne<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me)
        {
            return me.DataAccess.QueryOne(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<T> QueryOneAsync<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me)
        {
            return me.DataAccess.QueryOneAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static List<T> Query<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me)
        {
            return me.DataAccess.Query(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<List<T>> QueryAsync<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me)
        {
            return me.DataAccess.QueryAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static int Count<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me)
        {
            return me.DataAccess.Count(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.Predicate);
        }

        public static Task<int> CountAsync<T, TSecond, TThird, TFourth, TFifth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth> me)
        {
            return me.DataAccess.CountAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.Predicate);
        }

        #endregion

        #region QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth>Extension

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> InnerJoin<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>> propertyPath,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(me.DataAccess, me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, (propertyPath, condition, JoinType.Inner));
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> LeftJoin<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(
            this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>> propertyPath,
            Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> condition)
        {
            return new QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(me.DataAccess, me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, (propertyPath, condition, JoinType.Left));
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> Where<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> predicate)
        {
            me.Predicate = predicate;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> And<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>(Expression.AndAlso(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> Or<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>(Expression.OrElse(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> Select<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> selector)
        {
            me.Selector = selector;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> OrderBy<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>>, Sortord)> { (ordering, Sortord.Ascending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> ThenBy<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Ascending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> OrderByDescending<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>>, Sortord)> { (ordering, Sortord.Descending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> ThenByDescending<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Descending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> Skip<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, int n)
        {
            me.Skipped = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> Take<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, int n)
        {
            me.Taken = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> GroupBy<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> groupingColumns, Expression<Func<Grouping<T, TSecond, TThird, TFourth, TFifth, TSixth>, T>> groupingSelector)
        {
            me.GroupingColumns = groupingColumns;
            me.GroupingSelector = groupingSelector;

            return me;
        }

        public static T QueryOne<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me)
        {
            return me.DataAccess.QueryOne(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<T> QueryOneAsync<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me)
        {
            return me.DataAccess.QueryOneAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static List<T> Query<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me)
        {
            return me.DataAccess.Query(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<List<T>> QueryAsync<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me)
        {
            return me.DataAccess.QueryAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static int Count<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me)
        {
            return me.DataAccess.Count(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.Predicate);
        }

        public static Task<int> CountAsync<T, TSecond, TThird, TFourth, TFifth, TSixth>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth> me)
        {
            return me.DataAccess.CountAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.Predicate);
        }

        #endregion

        #region QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>Extension

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Where<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> predicate)
        {
            me.Predicate = predicate;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> And<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>(Expression.AndAlso(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Or<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> predicate)
        {
            var replacer = new ExpressionReplacer();

            if (!SameParameterNames(predicate.Parameters, me.Predicate.Parameters))
            {
                predicate = replacer.Replace(predicate, me.Predicate);
            }

            me.Predicate = Expression.Lambda<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>(Expression.OrElse(me.Predicate.Body, predicate.Body), me.Predicate.Parameters);

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Select<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> selector)
        {
            me.Selector = selector;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderBy<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>>, Sortord)> { (ordering, Sortord.Ascending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> ThenBy<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Ascending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> OrderByDescending<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> ordering)
        {
            me.OrderExpressions = new List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>>, Sortord)> { (ordering, Sortord.Descending) };

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> ThenByDescending<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> ordering)
        {
            me.OrderExpressions.Add((ordering, Sortord.Descending));

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Skip<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, int n)
        {
            me.Skipped = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> Take<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, int n)
        {
            me.Taken = n;

            return me;
        }

        public static QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> GroupBy<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> groupingColumns, Expression<Func<Grouping<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>, T>> groupingSelector)
        {
            me.GroupingColumns = groupingColumns;
            me.GroupingSelector = groupingSelector;

            return me;
        }

        public static T QueryOne<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me)
        {
            return me.DataAccess.QueryOne(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.SeventhJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<T> QueryOneAsync<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me)
        {
            return me.DataAccess.QueryOneAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.SeventhJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static List<T> Query<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me)
        {
            return me.DataAccess.Query(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.SeventhJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static Task<List<T>> QueryAsync<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me)
        {
            return me.DataAccess.QueryAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.SeventhJoin, me.Predicate, me.OrderExpressions, me.Selector, me.GroupingColumns, me.GroupingSelector, me.Skipped, me.Taken);
        }

        public static int Count<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me)
        {
            return me.DataAccess.Count(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.SeventhJoin, me.Predicate);
        }

        public static Task<int> CountAsync<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>(this QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> me)
        {
            return me.DataAccess.CountAsync(me.SecondJoin, me.ThirdJoin, me.FourthJoin, me.FifthJoin, me.SixthJoin, me.SeventhJoin, me.Predicate);
        }

        #endregion

        private static bool SameParameterNames(IList<ParameterExpression> parameters1, IList<ParameterExpression> parameters2)
        {
            if (parameters1.Count != parameters2.Count) return false;

            for (var i = 0; i < parameters1.Count; i++)
            {
                if (parameters1[i].Name != parameters2[i].Name) return false;
            }

            return true;
        }
    }

    internal class ExpressionReplacer : ExpressionVisitor
    {
        private readonly IDictionary<string, ParameterExpression> parameterMap = new Dictionary<string, ParameterExpression>();

        public T Replace<T>(T oldExpr, T newExpr) where T : LambdaExpression
        {
            for (var i = 0; i < oldExpr.Parameters.Count; i++)
            {
                this.parameterMap.Add(oldExpr.Parameters[i].Name, newExpr.Parameters[i]);
            }

            return this.Visit(oldExpr) as T;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return this.parameterMap[node.Name];
        }
    }
}