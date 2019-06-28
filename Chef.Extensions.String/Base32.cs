using System.Text;

namespace Chef.Extensions.String
{
    internal class Base32
    {
        private static readonly string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

        public static string Encode(string value, Encoding encoding)
        {
            if (string.IsNullOrEmpty(value)) return null;

            var valueBytes = encoding.GetBytes(value);
            var encodedBuilder = new StringBuilder();
            var position = 0;
            var left = 0;

            for (var i = 0; i < valueBytes.Length * 8 / 5 + (valueBytes.Length * 8 % 5 == 0 ? 0 : 1); i++)
            {
                var encodedByte = default(byte);

                if (left > 0)
                {
                    encodedByte |= (byte)(valueBytes[position] << (8 - left));

                    if (left <= 5 && position < valueBytes.Length - 1)
                    {
                        position++;

                        if (left < 5) encodedByte |= (byte)(valueBytes[position] >> left);
                    }
                }
                else
                {
                    encodedByte |= valueBytes[position];
                }

                encodedBuilder.Append(Alphabet[(byte)(encodedByte >> 3)]);

                left = 8 * (position + 1) - 5 * (i + 1);
            }

            encodedBuilder.Append(new string('=', encodedBuilder.Length % 8));

            return encodedBuilder.ToString();
        }

        public static string Decode(string value, Encoding encoding)
        {
            if (string.IsNullOrEmpty(value)) return null;

            value = value.ToUpper().TrimEnd('=');

            var decodedBytes = new byte[value.Length * 5 / 8];
            var position = 0;
            var available = 0;

            for (var i = 0; i < value.Length; i++)
            {
                var symbol = (byte)(Alphabet.IndexOf(value[i]) << 3);

                if (available > 0)
                {
                    decodedBytes[position] |= (byte)(symbol >> (8 - available));

                    if (available <= 5 && position < decodedBytes.Length - 1)
                    {
                        decodedBytes[++position] |= (byte)(symbol << available);
                    }
                }
                else
                {
                    decodedBytes[position] |= symbol;
                }

                available = 8 * (position + 1) - 5 * (i + 1);
            }

            return encoding.GetString(decodedBytes);
        }
    }
}