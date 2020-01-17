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

        void Insert(T value);

        Task InsertAsync(T value);

        void Insert(Expression<Func<T>> setter);

        Task InsertAsync(Expression<Func<T>> setter);

        void Insert(IEnumerable<T> values);

        Task InsertAsync(IEnumerable<T> values);

        void Insert(Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task InsertAsync(Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        void BulkInsert(Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task BulkInsertAsync(Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        void Update(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        void Update(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task UpdateAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        void BulkUpdate(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task BulkUpdateAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        void Upsert(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        Task UpsertAsync(Expression<Func<T, bool>> predicate, Expression<Func<T>> setter);

        void Upsert(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task UpsertAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        void BulkUpsert(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        Task BulkUpsertAsync(Expression<Func<T, bool>> predicateTemplate, Expression<Func<T>> setterTemplate, IEnumerable<T> values);

        void Delete(Expression<Func<T, bool>> predicate);

        Task DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}