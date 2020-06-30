using System;
using System.Linq.Expressions;

namespace Chef.Extensions.DbAccess
{
    public abstract class Grouping
    {
        protected Grouping()
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }
    }

    public class Grouping<T> : Grouping
    {
        public TColumn Select<TColumn>(Expression<Func<T, TColumn>> columnSelector)
        {
            throw new NotImplementedException();
        }

        public int Count<TColumn>(Expression<Func<T, TColumn>> countSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Max<TColumn>(Expression<Func<T, TColumn>> maxSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Min<TColumn>(Expression<Func<T, TColumn>> minSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Sum<TColumn>(Expression<Func<T, TColumn>> sumSelector)
        {
            throw new NotImplementedException();
        }

        public decimal Avg<TColumn>(Expression<Func<T, TColumn>> avgSelector)
        {
            throw new NotImplementedException();
        }
    }

    public class Grouping<T, TSecond> : Grouping
    {
        public TColumn Select<TColumn>(Expression<Func<T, TSecond, TColumn>> columnSelector)
        {
            throw new NotImplementedException();
        }

        public int Count<TColumn>(Expression<Func<T, TSecond, TColumn>> countSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Max<TColumn>(Expression<Func<T, TSecond, TColumn>> maxSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Min<TColumn>(Expression<Func<T, TSecond, TColumn>> minSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Sum<TColumn>(Expression<Func<T, TSecond, TColumn>> sumSelector)
        {
            throw new NotImplementedException();
        }

        public decimal Avg<TColumn>(Expression<Func<T, TSecond, TColumn>> avgSelector)
        {
            throw new NotImplementedException();
        }
    }

    public class Grouping<T, TSecond, TThird> : Grouping
    {
        public TColumn Select<TColumn>(Expression<Func<T, TSecond, TThird, TColumn>> columnSelector)
        {
            throw new NotImplementedException();
        }

        public int Count<TColumn>(Expression<Func<T, TSecond, TThird, TColumn>> countSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Max<TColumn>(Expression<Func<T, TSecond, TThird, TColumn>> maxSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Min<TColumn>(Expression<Func<T, TSecond, TThird, TColumn>> minSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Sum<TColumn>(Expression<Func<T, TSecond, TThird, TColumn>> sumSelector)
        {
            throw new NotImplementedException();
        }

        public decimal Avg<TColumn>(Expression<Func<T, TSecond, TThird, TColumn>> avgSelector)
        {
            throw new NotImplementedException();
        }
    }

    public class Grouping<T, TSecond, TThird, TFourth> : Grouping
    {
        public TColumn Select<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TColumn>> columnSelector)
        {
            throw new NotImplementedException();
        }

        public int Count<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TColumn>> countSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Max<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TColumn>> maxSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Min<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TColumn>> minSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Sum<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TColumn>> sumSelector)
        {
            throw new NotImplementedException();
        }

        public decimal Avg<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TColumn>> avgSelector)
        {
            throw new NotImplementedException();
        }
    }

    public class Grouping<T, TSecond, TThird, TFourth, TFifth> : Grouping
    {
        public TColumn Select<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TColumn>> columnSelector)
        {
            throw new NotImplementedException();
        }

        public int Count<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TColumn>> countSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Max<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TColumn>> maxSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Min<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TColumn>> minSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Sum<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TColumn>> sumSelector)
        {
            throw new NotImplementedException();
        }

        public decimal Avg<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TColumn>> avgSelector)
        {
            throw new NotImplementedException();
        }
    }

    public class Grouping<T, TSecond, TThird, TFourth, TFifth, TSixth> : Grouping
    {
        public TColumn Select<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TColumn>> columnSelector)
        {
            throw new NotImplementedException();
        }

        public int Count<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TColumn>> countSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Max<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TColumn>> maxSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Min<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TColumn>> minSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Sum<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TColumn>> sumSelector)
        {
            throw new NotImplementedException();
        }

        public decimal Avg<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TColumn>> avgSelector)
        {
            throw new NotImplementedException();
        }
    }

    public class Grouping<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh> : Grouping
    {
        public TColumn Select<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TColumn>> columnSelector)
        {
            throw new NotImplementedException();
        }

        public int Count<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TColumn>> countSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Max<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TColumn>> maxSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Min<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TColumn>> minSelector)
        {
            throw new NotImplementedException();
        }

        public TColumn Sum<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TColumn>> sumSelector)
        {
            throw new NotImplementedException();
        }

        public decimal Avg<TColumn>(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TColumn>> avgSelector)
        {
            throw new NotImplementedException();
        }
    }
}