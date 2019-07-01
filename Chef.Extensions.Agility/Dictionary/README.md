## Chef.Extensions.Dictionary

### AddRange&lt;TKey, TValue&gt;(IDictionary&lt;TKey, TValue&gt; collection)

Add `IDictionary<TKey, TValue>` collection to Dictionary&lt;TKey, TValue&gt;.

Example:

    var dict = new Dictionary<string, int> { { "1", 1 }, { "2", 2 } };
    var anothers = (IDictionary<string, int>)new Dictionary<string, int> { { "3", 3 }, { "4", 4 } };
    
    dict.AddRange(anothers);
    
    // dict is { "1": 1, "2": 2, "3": 3, "4": 4 }.

### AddRange&lt;TKey, TValue&gt;(ICollection&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `ICollection<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

Example:

    var dict = new Dictionary<string, int> { { "1", 1 }, { "2", 2 } };
    var anothers = (ICollection<KeyValuePair<string, int>>)new Dictionary<string, int> { { "3", 3 }, { "4", 4 } };
    
    dict.AddRange(anothers);
    
    // dict is { "1": 1, "2": 2, "3": 3, "4": 4 }.

### AddRange&lt;TKey, TValue&gt;(IEnumerable&lt;KeyValuePair&lt;TKey, TValue&gt;&gt; collection)

Add `IEnumerable<KeyValuePair<TKey, TValue>>` collection to Dictionary&lt;TKey, TValue&gt;.

Example:

    var dict = new Dictionary<string, int> { { "1", 1 }, { "2", 2 } };
    var anothers = (IEnumerable<KeyValuePair<string, int>>)new Dictionary<string, int> { { "3", 3 }, { "4", 4 } };
    
    dict.AddRange(anothers);
    
    // dict is { "1": 1, "2": 2, "3": 3, "4": 4 }.

### GetOrAdd&lt;TKey, TValue&gt;(TKey key, Func&lt;TValue&gt; factory)

Get value by key, but create object if not exists.

Example:

    var dict = new Dictionary<string, int>();
    
    var value = dict.GetOrAdd("1", () => 1);
    
    // value is 1.

### SafeGetOrAdd&lt;TKey, TValue&gt;(TKey key, Func&lt;TValue&gt; factory)

Get value by key, but create object if not exists. [Thread-Safe]

Example:

    var dict = new Dictionary<string, int>();
    
    var value = dict.SafeGetOrAdd("1", () => 1);
    
    // value is 1.

### AddOrSet&lt;TKey, TValue&gt;(TKey key, TValue value)

Set value by key and return value.

Example:

    var dict = new Dictionary<string, int>();
    
    dict.Add("1", 1);
    
    var value = dict.AddOrSet("1", 2);
    
    // value is 2.

### SafeAddOrSet&lt;TKey, TValue&gt;(TKey key, TValue value)

Set value by key and return value. [Thread-Safe]

Example:

    var dict = new Dictionary<string, int>();
    
    dict.Add("1", 1);
    
    var value = dict.AddOrSet("1", 2);
    
    // value is 2.

### GetValueOrDefault&lt;TKey, TValue&gt;(TKey key, TValue defaultValue = default(TValue))

Return default value if get value failed.

Example:

    var dict = new Dictionary<int, string>();
    
    dict.Add(1, "a");
    
    var value1 = dict.GetValueOrDefault(2, string.Empty);
    var value2 = dict.GetValueOrDefault(2);

    // value1 is "".
    // value2 is null.

### RollingRemove&lt;TKey, TValue&gt;(int maxCount)

Rolling remove last item.

Example:

    var dict = new Dictionary<int, string>();
    
    dict.Add(1, "1");
    dict.Add(2, "2");
    dict.Add(3, "3");
    
    dict.RollingRemove(1);
    
    // dict is { 1: "1" }.