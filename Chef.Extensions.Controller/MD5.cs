using System;
using System.Text;

namespace Chef.Extensions.Controller
{
    internal class MD5
    {
        public static string Hash(string value, string salt)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(string.Concat(value, salt)));

                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }
    }
}