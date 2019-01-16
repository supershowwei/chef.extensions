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

### PolymorphicQueryFirst&lt;T&gt;(sql [, param] [, discriminator])

Do polymorphic query and return first of base type.

> **sql**: string<br />
> **param**: object (Default is null)<br />
> **discriminator**: string (Default is "Discriminator")

### PolymorphicQueryFirstOrDefault&lt;T&gt;(sql [, param] [, discriminator])

Do polymorphic query and return first or default value of base type.

> **sql**: string<br />
> **param**: object (Default is null)<br />
> **discriminator**: string (Default is "Discriminator")

### PolymorphicInsert(sql, param)

Do polymorphic inserting.

> **sql**: string<br />
> **param**: object

### HierarchyInsert(sql, param)

Do hierarchy inserting.

> **sql**: string<br />
> **param**: object

## Examples

Suppose the `Food` table is:

![](https://i.imgur.com/Mw6EErT.png)

Base class `Food` and derived classes:

```cs
public abstract class Food
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Discriminator => this.GetType().Name;
}

public class Dessert : Food
{
    public int Calorie { get; set; }
}

public class DryGoods : Food
{
    public string CountryOfOrigin { get; set; }
}

public class Delicatessen : Food
{
    public string Chef { get; set; }
}
```

### Query foods that Id is 1, 2, 4 and return polymorphic results.

```cs
using (var db = new SqlConnection(connectionString))
{
    var sql = @"
SELECT
    f.Id
   ,f.[Name]
   ,f.Discriminator
   ,f.Calorie
   ,f.CountryOfOrigin
   ,f.Chef
FROM Food f
WHERE f.Id IN (1, 2, 4)";

    var results = db.PolymorphicQuery<Food>(sql);
}
```

The executed results:

```json
[
  {
    "$type": "LabForm462.Model.Data.Dessert, LabForm462",
    "Calorie": 300,
    "Id": 1,
    "Name": "蛋糕",
    "Discriminator": "Dessert"
  },
  {
    "$type": "LabForm462.Model.Data.DryGoods, LabForm462",
    "CountryOfOrigin": "台灣",
    "Id": 2,
    "Name": "乾香菇",
    "Discriminator": "DryGoods"
  },
  {
    "$type": "LabForm462.Model.Data.Delicatessen, LabForm462",
    "Chef": "Johnny",
    "Id": 4,
    "Name": "涼拌毛豆",
    "Discriminator": "Delicatessen"
  }
]
```

### Insert polymorphic collection to database.

```cs
var foods = new List<Food>
            {
                new Dessert
                {
                    Name = "Cake111",
                    Calorie = 100,
                    ShelfLife = new ShelfLife { Months = 0, Days = 3 }
                },
                new DryGoods
                {
                    Name = "Shiitake222",
                    CountryOfOrigin = "Taiwan",
                    ShelfLife = new ShelfLife { Months = 12, Days = 0 }
                },
                new Delicatessen
                {
                    Name = "Bun333",
                    Chef = "Mary",
                    ShelfLife = new ShelfLife { Months = 0, Days = 3 }
                }
            };

using (var db = new SqlConnection(connectionString))
{
    var sql = @"
INSERT INTO Food([Name]
                ,ShelfLife_Months
                ,ShelfLife_Days
                ,Discriminator
                ,Calorie
                ,CountryOfOrigin
                ,Chef)
    VALUES (@Name
           ,@ShelfLife_Months
           ,@ShelfLife_Days
           ,@Discriminator
           ,@Calorie
           ,@CountryOfOrigin
           ,@Chef);";

    db.PolymorphicInsert(sql, foods);
}
```

Split hierarchical properties by underscore as `ShelfLife` in example. The executed result is:

![](https://i.imgur.com/g5tCZJ3.png)

## Custom RowParser

Default is getting row parser by finding derived type with matching discriminator value precisely. We can implement *`IRowParserProvider`* and assign to `Chef.Extensions.Dapper.Extension.RowParserProvider` to change default row parser.
