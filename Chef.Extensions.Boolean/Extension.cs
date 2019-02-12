using System;

namespace Chef.Extensions.Boolean
{
    public static class Extension
    {
        public static T IIF<T>(this bool me, T trueValue, T falseValue)
        {
            return me ? trueValue : falseValue;
        }

        public static T IIF<T>(this bool me, Func<T> trueValue, Func<T> falseValue)
        {
            return me ? trueValue() : falseValue();
        }
    }
}