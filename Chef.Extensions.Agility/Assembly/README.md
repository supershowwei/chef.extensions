## Chef.Extensions.Assembly

### GetCurrentDirectory()

Return directory of assembly executing location.

Example:

    var result = Assembly.GetExecutingAssembly().GetCurrentDirectory();
    
    // result is current directory of current assembly.