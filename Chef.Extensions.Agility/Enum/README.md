## Chef.Extensions.Enum

### GetDescription()

Return description if Enum field has DescriptionAttribute, otherwise return enum name.

Example:

    public enum SomeKind
    {
        [Description("A new one.")]
        New,
        Old
    }

    var result1 = SomeKind.New.GetDescription();
    var result2 = SomeKind.Old.GetDescription();
    
    // result1 is "A new one.".
    // result2 is "Old".