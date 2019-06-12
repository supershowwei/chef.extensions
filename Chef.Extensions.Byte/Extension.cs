using System;

namespace Chef.Extensions.Byte
{
    public static class Extension
    {
        public static byte[] GetRange(this byte[] me, int startIndex, int length)
        {
            var bytes = new byte[length];

            Array.Copy(me, startIndex, bytes, 0, length);

            return bytes;
        }

        public static byte[] GetRange(this byte[] me, long startIndex, long length)
        {
            var bytes = new byte[length];

            Array.Copy(me, startIndex, bytes, 0, length);

            return bytes;
        }
    }
}