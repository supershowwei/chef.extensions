using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Chef.Extensions.Data
{
    public static class Extension
    {
        public static DataTable ToDataTable<T, TColumns>(this List<T> me, Func<T, TColumns> columns)
        {
            var table = new DataTable();

            var propertiesOfCorrespondingColumn = GeneratePropertiesOfCorrespondingColumn(columns);

            table.Columns.AddRange(propertiesOfCorrespondingColumn.Keys.ToArray());

            me.ForEach(
                item =>
                    {
                        var row = table.NewRow();

                        foreach (var property in propertiesOfCorrespondingColumn)
                        {
                            row[property.Key] = property.Value == null ? DBNull.Value : property.Value.GetValue(item);
                        }

                        table.Rows.Add(row);
                    });

            return table;
        }

        private static Dictionary<DataColumn, PropertyInfo> GeneratePropertiesOfCorrespondingColumn<T, TColumns>(
            Func<T, TColumns> columns)
        {
            var properties = typeof(T).GetProperties()
                .ToDictionary(
                    p => p.Name,
                    p =>
                        {
                            var columnAttr =
                                p.CustomAttributes.SingleOrDefault(a => a.AttributeType == typeof(ColumnAttribute));

                            return new
                                       {
                                           Value = p,
                                           ColumnName =
                                               columnAttr == null
                                                   ? p.Name
                                                   : (string)columnAttr.ConstructorArguments[0].Value
                                       };
                        });

            return columns.Method.ReturnType.GetProperties()
                .ToDictionary(
                    column => new DataColumn(
                        properties[column.Name].ColumnName,
                        Nullable.GetUnderlyingType(properties[column.Name].Value.PropertyType)
                        ?? properties[column.Name].Value.PropertyType),
                    column => properties[column.Name].Value);
        }
    }
}