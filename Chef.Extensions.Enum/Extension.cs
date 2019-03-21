using System.ComponentModel;

namespace Chef.Extensions.Enum
{
    public static class Extension
    {
        public static string GetDescription(this System.Enum me)
        {
            var fieldInfo = me.GetType().GetField(me.ToString());

            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : me.ToString();
        }
    }
}