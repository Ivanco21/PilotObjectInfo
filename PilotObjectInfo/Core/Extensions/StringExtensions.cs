
using System.Globalization;
using System.Text;
using System;

namespace PilotObjectInfo.Core.Extensions
{
    static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (input.Length == 1)
                return input.ToUpper();

            return char.ToUpper(input[0]) + input.Substring(1);
        }

        public static string SnakeToPascalCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            input = input.ToLower();
            var parts = input.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();

            foreach (var part in parts)
            {
                if (part.Length == 0)
                    continue;

                result.Append(char.ToUpper(part[0], CultureInfo.InvariantCulture));
                if (part.Length > 1)
                {
                    result.Append(part.Substring(1).ToLower(CultureInfo.InvariantCulture));
                }
            }

            return result.ToString();
        }
    }
}
