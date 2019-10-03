using System;

namespace Chef.Extensions.Long
{
    public static class Extension
    {
        private static readonly System.DateTime InitialTime = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///     Parse Unix time milliseconds to DateTime.
        /// </summary>
        /// <param name="me">Milliseconds of Unix time.</param>
        /// <returns>Returns DateTime.</returns>
        public static System.DateTime ToDateTime(this long me)
        {
            return InitialTime.AddMilliseconds(me).ToLocalTime();
        }
    }
}