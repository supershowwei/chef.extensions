using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Chef.Extensions.DbAccess
{
    public enum Sortord
    {
        Ascending,
        Descending
    }

    public interface IDataAccess<T>
    {
        T QueryOne(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null);

        Task<T> QueryOneAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null);

        List<T> Query(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null);

        Task<List<T>> QueryAsync(
            Expression<Func<T, bool>> predicate,
            IEnumerable<(Expression<Func<T, object>>, Sortord)> orderings = null,
            Expression<Func<T, object>> selector = null,
            int? top = null);

        int Insert(T value);

        Task<int> InsertAsync(T value);

        int Insert(Expression<Func<T>> setter);

        Task<int> InsertAsync(Expression<Func<T>> setter);

        int Insert(IEnumerable<T> values);

        Task<int> InsertAsync(IEnumerable<T> values);

        int Insert(Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task<int> InsertAsync(Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        int BulkInsert(IEnumerable<T> values);

        Task<int> BulkInsertAsync(IEnumerable<T> values);

        int BulkInsert(Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task<int> BulkInsertAsync(Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        int Update(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        int Update(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task<int> UpdateAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        int BulkUpdate(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task<int> BulkUpdateAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        int Upsert(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        Task<int> UpsertAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        int Upsert(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task<int> UpsertAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        int BulkUpsert(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task<int> BulkUpsertAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        int Delete(Expression<Func<T, bool>> predicate);

        Task<int> DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}