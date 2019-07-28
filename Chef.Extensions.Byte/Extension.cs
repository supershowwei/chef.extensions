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

        public static bool TryGetRange(this byte[] me, int startIndex, int length, out byte[] bytes)
        {
            bytes = null;

            if (me.Length < (startIndex + length)) return false;

            bytes = new byte[length];

            Array.Copy(me, startIndex, bytes, 0, length);

            return true;
        }

        public static byte[] GetRange(this byte[] me, long startIndex, long length)
        {
            var bytes = new byte[length];

            Array.Copy(me, startIndex, bytes, 0, length);

            return bytes;
        }

        public static bool TryGetRange(this byte[] me, long startIndex, long length, out byte[] bytes)
        {
            bytes = null;

            if (me.Length < (startIndex + length)) return false;

            bytes = new byte[length];

            Array.Copy(me, startIndex, bytes, 0, length);

            return true;
        }

        public static void Set(this byte[] me, int index, byte value)
        {
            me[index] = value;
        }

        public static void Set(this byte[] me, long index, byte value)
        {
            me[index] = value;
        }

        public static void SetLast(this byte[] me, byte value)
        {
            me[me.Length - 1] = value;
        }
    }
}