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

        public int? Skipped { get; set; }

        public int? Taken { get; set; }
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

        public int? Skipped { get; set; }

        public int? Taken { get; set; }

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

        public int? Skipped { get; set; }

        public int? Taken { get; set; }

        public (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) SecondJoin { get; }

        public (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) ThirdJoin { get; }
    }

    public class QueryObject<T, TSecond, TThird, TFourth>
    {
        public QueryObject(
            IDataAccess<T> dataAccess,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin)
        {
            this.DataAccess = dataAccess;
            this.SecondJoin = secondJoin;
            this.ThirdJoin = thirdJoin;
            this.FourthJoin = fourthJoin;
        }

        public IDataAccess<T> DataAccess { get; }

        public Expression<Func<T, TSecond, TThird, TFourth, bool>> Predicate { get; set; }

        public List<(Expression<Func<T, TSecond, TThird, TFourth, object>>, Sortord)> OrderExpressions { get; set; }

        public Expression<Func<T, TSecond, TThird, TFourth, object>> Selector { get; set; }

        public int? Skipped { get; set; }

        public int? Taken { get; set; }

        public (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) SecondJoin { get; }

        public (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) ThirdJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) FourthJoin { get; }
    }

    public class QueryObject<T, TSecond, TThird, TFourth, TFifth>
    {
        public QueryObject(
            IDataAccess<T> dataAccess,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin)
        {
            this.DataAccess = dataAccess;
            this.SecondJoin = secondJoin;
            this.ThirdJoin = thirdJoin;
            this.FourthJoin = fourthJoin;
            this.FifthJoin = fifthJoin;
        }

        public IDataAccess<T> DataAccess { get; }

        public Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>> Predicate { get; set; }

        public List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>>, Sortord)> OrderExpressions { get; set; }

        public Expression<Func<T, TSecond, TThird, TFourth, TFifth, object>> Selector { get; set; }

        public int? Skipped { get; set; }

        public int? Taken { get; set; }

        public (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) SecondJoin { get; }

        public (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) ThirdJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) FourthJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) FifthJoin { get; }
    }

    public class QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth>
    {
        public QueryObject(
            IDataAccess<T> dataAccess,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin)
        {
            this.DataAccess = dataAccess;
            this.SecondJoin = secondJoin;
            this.ThirdJoin = thirdJoin;
            this.FourthJoin = fourthJoin;
            this.FifthJoin = fifthJoin;
            this.SixthJoin = sixthJoin;
        }

        public IDataAccess<T> DataAccess { get; }

        public Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>> Predicate { get; set; }

        public List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>>, Sortord)> OrderExpressions { get; set; }

        public Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, object>> Selector { get; set; }

        public int? Skipped { get; set; }

        public int? Taken { get; set; }

        public (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) SecondJoin { get; }

        public (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) ThirdJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) FourthJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) FifthJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) SixthJoin { get; }
    }

    public class QueryObject<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>
    {
        public QueryObject(
            IDataAccess<T> dataAccess,
            (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) secondJoin,
            (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) thirdJoin,
            (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) fourthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) fifthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) sixthJoin,
            (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>, JoinType) seventhJoin)
        {
            this.DataAccess = dataAccess;
            this.SecondJoin = secondJoin;
            this.ThirdJoin = thirdJoin;
            this.FourthJoin = fourthJoin;
            this.FifthJoin = fifthJoin;
            this.SixthJoin = sixthJoin;
            this.SeventhJoin = seventhJoin;
        }

        public IDataAccess<T> DataAccess { get; }

        public Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>> Predicate { get; set; }

        public List<(Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>>, Sortord)> OrderExpressions { get; set; }

        public Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, object>> Selector { get; set; }

        public int? Skipped { get; set; }

        public int? Taken { get; set; }

        public (Expression<Func<T, TSecond>>, Expression<Func<T, TSecond, bool>>, JoinType) SecondJoin { get; }

        public (Expression<Func<T, TSecond, TThird>>, Expression<Func<T, TSecond, TThird, bool>>, JoinType) ThirdJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth>>, Expression<Func<T, TSecond, TThird, TFourth, bool>>, JoinType) FourthJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth, TFifth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, bool>>, JoinType) FifthJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, bool>>, JoinType) SixthJoin { get; }

        public (Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh>>, Expression<Func<T, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, bool>>, JoinType) SeventhJoin { get; }
    }
}