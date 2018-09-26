## List of Methods

### PolymorphicQuery&lt;T&gt;(sql [, param] [, discriminator])

Do polymorphic query and return a collection of base type.

> **sql**: string<br />
> **param**: object (Default is null)<br />
> **discriminator**: string (Default is "Discriminator")

### PolymorphicQuerySingle&lt;T&gt;(sql [, param] [, discriminator])

Do polymorphic query and return only one of base type.

> **sql**: string<br />
> **param**: object (Default is null)<br />
> **discriminator**: string (Default is "Discriminator")

### PolymorphicQuerySingleOrDefault&lt;T&gt;(sql [, param] [, discriminator])

Do polymorphic query and return only one or default value of base type.

> **sql**: string<br />
> **param**: object (Default is null)<br />
> **discriminator**: string (Default is "Discriminator")

### GenerateParam(param [, prefix] [, suffix])

Generate `DynamicParameters` and output a `Dictionary<string, string>` for building sql statement.

> **param**: DynamicParameters<br />
> **prefix**: string (Default is "")<br />
> **suffix**: string (Default is "")

## Custom RowParser

Default is getting row parser by finding derive type with matching discriminator value precisely. We can implement *`IRowParserProvider`* and assign to `Chef.Extensions.Dapper.Extension.RowParserProvider` to change default row parser.
