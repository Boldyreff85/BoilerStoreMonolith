using System.Globalization;

namespace BoilerStoreMonolith.Extensions
{
    public static class StringExtensions
    {
        public static float ToFloat(this string input)
        {
            return float.Parse(input, CultureInfo.InvariantCulture);
        }
    }
}