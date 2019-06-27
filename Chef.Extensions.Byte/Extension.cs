using System;

namespace Chef.Extensions.Byte
{
    public static class Extension
    {
        public static byte[] GetRange(this byte[] me, int startIndex, int length)
        {
            if (me.Length < (startIndex + length)) return null;

            var bytes = new byte[length];

            Array.Copy(me, startIndex, bytes, 0, length);

            return bytes;
        }

        public static byte[] GetRange(this byte[] me, long startIndex, long length)
        {
            if (me.Length < (startIndex + length)) return null;

            var bytes = new byte[length];

            Array.Copy(me, startIndex, bytes, 0, length);

            return bytes;
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