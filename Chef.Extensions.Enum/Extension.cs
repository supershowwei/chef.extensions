using System.Collections.Concurrent;
using System.ComponentModel;

namespace Chef.Extensions.Enum
{
    public static class Extension
    {
        private static readonly ConcurrentDictionary<string, string> FriendlyStrings = new ConcurrentDictionary<string, string>();
        
        public static string GetDescription(this System.Enum me)
        {
            var fieldInfo = me.GetType().GetField(me.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : me.ToString();
        }

        public static string ToFriendlyString(this System.Enum me)
        {
            var enumType = me.GetType();
            var enumString = me.ToString();

            return FriendlyStrings.GetOrAdd($"{enumType.FullName}.{enumString}", key => GetDescription(me));
        }
    }
}