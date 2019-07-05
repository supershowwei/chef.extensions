## Chef.Extensions.Type

### IsUserDefined()

Check if Type is user defined.

Example:

    public class Person
    {
        public string Name { get; set; }
    
        public int Age { get; set; }
    }

    var result1 = typeof(Person).IsUserDefined();
    var result2 = typeof(string).IsUserDefined();
    
    // result1 is true, result2 is false.

### GetPropertyNames()

Return property names.

Example:

    var result = new { Name = "1", Age = 2 }.GetType().GetPropertyNames();
    
    // result is ["Name","Age"]

### GetPropertyNames(string prefix)

Return property names concatenated prefix.

Example:

    var result = new { Name = "1", Age = 2 }.GetType().GetPropertyNames("kkk.");
    
    // result is ["kkk.Name","kkk.Age"]

### GetActivator()

Return activator of type.

Example:

    var activator = typeof(List<int>).GetActivator();
    
    var list = (List<int>)activator();
    
    // list is a new instance of List<int>.

### GetActivator(int index)

Return activator of type by constructor's index.

Example:

    var activator = typeof(List<int>).GetActivator(0);
    
    var list = (List<int>)activator();
    
    // list is a new instance of List<int>.

### GetActivator(Type attributeType)

Return activator of type by constructor's custom attribute.

Example:

    var activator = typeof(MyType).GetActivator(typeof(MyDefaultCtorAttribute));
    
    var obj = (MyType)activator();
    
    // obj is a new instance of MyType.

### GetActivator(params Type[] parameterTypes)

Return activator of type by constructor's parameter types.

Example:

    var activator = typeof(MyType).GetActivator(typeof(int));
    
    var args = new object[] { 1 };
    var obj = (MyType)activator(args);
    
    // obj is a new instance of MyType.

