using System.Collections.Generic;
using System.Data;
using System.Linq;
using Chef.Extensions.Dapper.Extensions;
using Dapper;

namespace Chef.Extensions.Dapper
{
    public static class Extension
    {
        private static readonly IRowParserProvider DefaultRowParserProvider = new DefaultRowParserProvider();
        private static IRowParserProvider userDefinedRowParserProvider;

        public static IRowParserProvider RowParserProvider
        {
            private get { return userDefinedRowParserProvider ?? DefaultRowParserProvider; }
            set { userDefinedRowParserProvider = value; }
        }

        public static IEnumerable<T> PolymorphicQuery<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = new List<T>();

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader);

                    result.Add(parser(reader));
                }
            }

            return result;
        }

        public static T PolymorphicQuerySingle<T>(
            this IDbConnection cnn,
            string sql,
            object param = null,
            string discriminator = "Discriminator")
        {
            var result = new List<T>();

            using (var reader = cnn.ExecuteReader(sql, param))
            {
                while (reader.Read())
                {
                    var parser = RowParserProvider.GetRowParser<T>(discriminator, reader);

                    result.Add(parser(reader));
                }
            }

            return result.Single();
        }

        public static Dictionary<string, string> GenerateParam(
            this object value,
            DynamicParameters param,
            string prefix = "",
            string suffix = "")
        {
            var columns = new Dictionary<string, string>();

            if (value == null) return columns;

            foreach (var property in value.GetType().GetProperties())
            {
                var propertyType = property.PropertyType;
                var propertyValue = property.GetValue(value);

                if (propertyValue == null) continue;

                if (propertyType.IsUserDefined())
                {
                    columns.AddRange(GenerateParam(propertyValue, param, $"{prefix}{property.Name}_", suffix));
                }
                else
                {
                    var columnName = $"{prefix}{property.Name}";

                    columns.Add(columnName, $"@{columnName}{suffix}");
                    param.Add($"{columnName}{suffix}", propertyValue);
                }
            }

            return columns;
        }
    }
}