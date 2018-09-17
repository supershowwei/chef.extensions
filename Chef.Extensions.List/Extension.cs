using System.Collections.Generic;

namespace Chef.Extensions.List
{
    public static class Extension
    {
        public static bool IsNullOrEmpty<T>(this List<T> me)
        {
            return me == null || me.Count == 0;
        }
    }
}