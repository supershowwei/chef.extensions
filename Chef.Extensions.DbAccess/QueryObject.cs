using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Chef.Extensions.DbAccess
{
    public class QueryObject<T>
    {
        public QueryObject(IDataAccess<T> dataAccess)
        {
            this.DataAccess = dataAccess;
        }

        public IDataAccess<T> DataAccess { get; }

        public Expression<Func<T, bool>> Predicate { get; set; }

        public List<(Expression<Func<T, object>>, Sortord)> OrderExpressions { get; set; }

        public Expression<Func<T, object>> Selector { get; set; }

        public Expression<Func<T>> Setter { get; set; }

        public int? Top { get; set; }
    }

    public class QueryObject<T, TSecond>
    {
        public QueryObject(
            IDataAccess<T> dataAccess,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin)
        {
            this.DataAccess = dataAccess;
            this.SecondJoin = secondJoin;
        }

        public IDataAccess<T> DataAccess { get; }

        public Expression<Func<T, TSecond, bool>> Predicate { get; set; }

        public List<(Expression<Func<T, TSecond, object>>, Sortord)> OrderExpressions { get; set; }

        public Expression<Func<T, TSecond, object>> Selector { get; set; }

        public int? Top { get; set; }

        public (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) SecondJoin { get; }
    }

    public class QueryObject<T, TSecond, TThird>
    {
        public QueryObject(
            IDataAccess<T> dataAccess,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin)
        {
            this.DataAccess = dataAccess;
            this.SecondJoin = secondJoin;
            this.ThirdJoin = thirdJoin;
        }

        public IDataAccess<T> DataAccess { get; }

        public Expression<Func<T, TSecond, TThird, bool>> Predicate { get; set; }

        public List<(Expression<Func<T, TSecond, TThird, object>>, Sortord)> OrderExpressions { get; set; }

        public Expression<Func<T, TSecond, TThird, object>> Selector { get; set; }

        public int? Top { get; set; }

        public (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) SecondJoin { get; }

        public (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) ThirdJoin { get; }
    }
}