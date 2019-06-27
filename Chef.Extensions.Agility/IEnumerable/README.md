## Chef.Extensions.IEnumerable

### ForEach&lt;T&gt;(Action&lt;T&gt; action)

Execute Action&lt;T&gt; iteratively.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var result = new[] { new Person { Name = "abc", Age = 1 }, new Person { Name = "def", Age = 2 } };
    
    result.ForEach(x => x.Age += 1);
    
    // result is [{"Name":"abc","Age":2},{"Name":"def","Age":3}]

### IsNullOrEmpty&lt;T&gt;()

Check if IEnumerable&lt;T&gt; is null or empty.

Example:

    var result = new int[] { }.IsNullOrEmpty();
    
    // result is true.

### IsNotEmpty&lt;T&gt;()

Check if IEnumerable&lt;T&gt; is not null and not empty.

Example:

    var result = new int[] { }.IsNotEmpty();
    
    // result is false.

### Any&lt;T&gt;(Func&lt;T, bool&gt; predicate, out T first)

Check if any item predicated and output first predicated item.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var persons = new[]
                  {
                      new Person { Name = "abc", Age = 1 },
                      new Person { Name = "def", Age = 2 },
                      new Person { Name = "ghi", Age = 2 }
                  };

    persons.Any(x => x.Age == 2, out var result);
    
    // result is {"Name":"def","Age":2}

### Any&lt;T&gt;(Func&lt;T, bool&gt; predicate, out int index, out T first)

Check if any item predicated and output index and first predicated item.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var persons = new[]
                  {
                      new Person { Name = "abc", Age = 1 },
                      new Person { Name = "def", Age = 2 },
                      new Person { Name = "ghi", Age = 2 }
                  };

    persons.Any(x => x.Age == 2, out var index, out var first);
    
    // index is 1.
    // first is {"Name":"def","Age":2}.

### SelectWhere&lt;T, TResult&gt;(Func&lt;T, bool&gt; predicate, Func&lt;T, TResult&gt; selector)

Select when predicated in one loop.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var persons = new[]
                  {
                      new Person { Name = "abc", Age = 1 },
                      new Person { Name = "def", Age = 2 },
                      new Person { Name = "ghi", Age = 2 }
                  };

    var result = persons.SelectWhere(x => x.Age == 2, x => x);
    
    // result is [{"Name":"def","Age":2},{"Name":"ghi","Age":2}]

### SelectWhere&lt;T, TResult&gt;(Func&lt;T, TResult, bool&gt; predicate)

Select out result when predicated in one loop.

Example:

    var numbers = new string[] { "1", "2", "three" }.SelectWhere((string s, out int result) => int.TryParse(s, out result));
    
    // numbers is [1, 2].

### TakeLast&lt;T&gt;(int count)

Returns a specified number of contiguous elements from the end of a original sequence.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var persons = new[]
                  {
                      new Person { Name = "abc", Age = 1 },
                      new Person { Name = "def", Age = 2 },
                      new Person { Name = "ghi", Age = 2 }
                  };

    var result = persons.TakeLast(1);
    
    // result is [{"Name":"ghi","Age":2}]

### Merge&lt;T&gt;(IEnumerable&lt;T&gt; merged, Func&lt;T, T, bool&gt; compare, Action&lt;T, T&gt; merge)

Merge two collection into one.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var primaryPersons = new[]
                         {
                             new Person { Name = "abc" },
                             new Person { Name = "def" }
                         };

    var mergedPersons = new[]
                        {
                            new Person { Name = "abc", Age = 1 },
                            new Person { Name = "def", Age = 2 },
                            new Person { Name = "ghi", Age = 3 }
                        };

    var result = primaryPersons.Merge(
        mergedPersons,
        (primary, merged) => primary.Name.Equals(merged.Name),
        (primary, merged) => { primary.Age = merged.Age; });
    
    // result is [{"Name": "abc", "Age": 1}, {"Name": "def", "Age": 2}, {"Name": "ghi", "Age": 3}]

### FindIndex&lt;T&gt;(Func&lt;T, bool&gt; predicate)

Find index by predicate.

Example:

    IEnumerable<int> enumerable = new[] { 11, 22, 33, 44, 55 };

    var result = enumerable.FindIndex(x => x.Equals(55));

    // result is 4.