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
}