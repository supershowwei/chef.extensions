# Chef.Extensions.Dapper


## List of Methods

- **PolymorphicQuery&lt;T&gt;(sql [, param] [, discriminator])**: Do polymorphic query and return a collection of base type.

> **sql**: string
>
> **param**: object (Default is null)
>
> **discriminator**: string (Default is "Discriminator")

- **PolymorphicQuery&lt;T&gt;(sql [, param] [, discriminator])**: Do polymorphic query and return only one of base type.

> **sql**: string
>
> **param**: object (Default is null)
>
> **discriminator**: string (Default is "Discriminator")

## Custom RowParser

We can implement *`IRowParserProvider`* and assign to `Chef.Extensions.Dapper.Extension.RowParserProvider` change row parser instance.