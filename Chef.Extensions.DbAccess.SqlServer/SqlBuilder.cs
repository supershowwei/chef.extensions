using System.Text;

namespace Chef.Extensions.DbAccess.SqlServer
{
    internal class SqlBuilder
    {
        private readonly StringBuilder builder;

        public SqlBuilder()
        {
            this.builder = new StringBuilder();
        }

        private string Sql => this.builder.ToString();

        public static SqlBuilder operator +(SqlBuilder sqlBuilder, string sql)
        {
            sqlBuilder.Append(sql);

            return sqlBuilder;
        }

        public static implicit operator SqlBuilder(string sql)
        {
            var sqlBuilder = new SqlBuilder();

            sqlBuilder.Append(sql);

            return sqlBuilder;
        }

        public static implicit operator string(SqlBuilder sqlBuilder)
        {
            return sqlBuilder.ToString();
        }

        public void Append(string sql)
        {
            this.builder.Append(sql);
        }

        public override string ToString()
        {
            return this.builder.ToString();
        }
    }
}