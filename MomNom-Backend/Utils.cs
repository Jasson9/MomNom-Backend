using System.Text;
using System.Text.RegularExpressions;

namespace MomNom_Backend
{
    public static class Utils
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public static class StringExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string emailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
            return Regex.IsMatch(email, emailPattern);
        }

        public static bool IsContainingSymbol(this string text)
        {
            bool hasSymbol = false;
            foreach (char c in text)
            {
                if (Char.IsSymbol(c))
                {
                    hasSymbol = true;
                    break;
                }
            }

            return hasSymbol;
        }
    }
}
