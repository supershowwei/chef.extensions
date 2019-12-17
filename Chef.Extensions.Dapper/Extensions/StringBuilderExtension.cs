using System.Text;

namespace Chef.Extensions.Dapper.Extensions
{
    internal static class StringBuilderExtension
    {
        public static void AppendAlias(this StringBuilder me, string value, string alias)
        {
            if (!string.IsNullOrEmpty(alias)) me.Append($"{alias}.");

            me.Append(value);
        }
    }
}