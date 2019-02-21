using System.IO;

namespace Chef.Extensions.Assembly
{
    public static class Extension
    {
        public static string GetCurrentDirectory(this System.Reflection.Assembly me)
        {
            return Path.GetDirectoryName(me.Location);
        }
    }
}