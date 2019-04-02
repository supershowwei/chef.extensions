using System;
using System.Text;

namespace Chef.Extensions.Mvc.Helpers
{
    internal class MD5
    {
        public static string Hash(string value)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(value));

                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
    }
}