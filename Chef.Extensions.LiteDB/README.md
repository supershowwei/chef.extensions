## Find as Immutability

LiteDB do not support deserializing immutable object, like this:

    public sealed class Member
    {
        public Member(int id, string name, string email, IReadOnlyCollection<int> identifiers, IReadOnlyDictionary<int, string> goods)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.Identifiers = identifiers;
            this.Goods = goods;
        }

        public Member()
        {
        }

        public int Id { get; }

        public string Name { get; }

        public string Email { get; }

        public IReadOnlyCollection<int> Identifiers { get; }

        public IReadOnlyDictionary<int, string> Goods { get; }
    }

When I use `Find<Member>()` to query objects, the content of objects will be empty or null. I extend `LiteCollection<T>` to solve this problem, the following are extension methods:

    public static IEnumerable<T> FindAll<T>(this LiteCollection<T> me);
    public static IEnumerable<T> FindAsImmutability<T>(this LiteCollection<T> me, Query query, int skip = 0, int limit = int.MaxValue);
    public static IEnumerable<T> FindAsImmutability<T>(this LiteCollection<T> me, Expression<Func<T, bool>> predicate, int skip = 0, int limit = int.MaxValue);
    public static T FindAsImmutabilityById<T>(this LiteCollection<T> me, BsonValue id);
    public static T FindOneAsImmutability<T>(this LiteCollection<T> me, Query query);
    public static T FindOneAsImmutability<T>(this LiteCollection<T> me, Expression<Func<T, bool>> predicate);

When use these extension methods must declare a parameterless constructor.