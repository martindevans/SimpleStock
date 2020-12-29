using System.Text;

namespace SimpleStock.Extensions
{
    public static class StringExtensions
    {
        public static string SliceExact(this string input, int length, char padding = ' ')
        {
            var b = new StringBuilder(input);

            // Pad to correct length
            while (b.Length < length)
                b.Append(padding);

            // Trim over length
            if (b.Length > length)
            {
                if (length <= 10)
                {
                    b.Remove(length, b.Length - length);
                }
                else
                {
                    b.Remove(length - 3, b.Length - length + 3);
                    b.Append("...");
                }
            }
            
            return b.ToString();
        }
    }
}
