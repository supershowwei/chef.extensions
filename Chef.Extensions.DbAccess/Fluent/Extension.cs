using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chef.Extensions.DbAccess.Fluent
{
    public static class Extension
    {
        #region IDataAccessExtension

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

        public static QueryObject<T> Top<T>(this IDataAccess<T> me, int n)
        {
            return new QueryObject<T>(me) { Top = n };
        }

        #endregion

        #region QueryObjectExtension

        public static QueryObject<T> Where<T>(this QueryObject<T> me, Expression<Func<T, bool>> predicate)
        {
            me.Predicate = predicate;

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

        public static QueryObject<T> Top<T>(this QueryObject<T> me, int n)
        {
            me.Top = n;

            return me;
        }

        public static T QueryOne<T>(this QueryObject<T> me)
        {
            return me.DataAccess.QueryOne(me.Predicate, me.OrderExpressions, me.Selector, me.Top);
        }

        public static Task<T> QueryOneAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.QueryOneAsync(me.Predicate, me.OrderExpressions, me.Selector, me.Top);
        }

        public static List<T> Query<T>(this QueryObject<T> me)
        {
            return me.DataAccess.Query(me.Predicate, me.OrderExpressions, me.Selector, me.Top);
        }

        public static Task<List<T>> QueryAsync<T>(this QueryObject<T> me)
        {
            return me.DataAccess.QueryAsync(me.Predicate, me.OrderExpressions, me.Selector, me.Top);
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
    }
}