using System;
using System.Collections.Generic;
using System.Linq;

namespace PilotObjectInfo.Core.Services
{
    /// <summary>
    /// Сервис извлечения слов с латиницей из произвольного текста.
    ///</summary>
    internal class LatinWordExtractorService
    {
        private static readonly char[] _separators = " \t\r\n\"'`(){}[]<>,;:=+-*/\\|!?@#$%^&~.".ToCharArray();
        public string[] ExtractWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<string>();
            
            string[] words = text.Split(_separators, StringSplitOptions.RemoveEmptyEntries);

            List<string> result = new();

            foreach (string rawWord in words)
            {
                if (IsContainsCyrillic(rawWord))
                    continue;
                
                string trimmed = TrimNonLetterOrDigit(rawWord);

                if (string.IsNullOrEmpty(trimmed))
                    continue;
                
                if (!IsContainsLatin(trimmed))
                    continue;

                result.Add(trimmed);
            }

            return result.Distinct().ToArray();
        }


        private static bool IsContainsCyrillic(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];
                if (c is >= '\u0400' and <= '\u04FF' || // Основная кириллица
                    c is >= '\u0500' and <= '\u052F' || // Дополнение к кириллице
                    c is >= '\u2DE0' and <= '\u2DFF' || // Расширенная кириллица — A
                    c is >= '\uA640' and <= '\uA69F' || // Расширенная кириллица — B
                    c is >= '\u1C80' and <= '\u1C8F')   // Расширенная кириллица — C
                {
                    return true;
                }
            }

            return false;
        }
        
        private static bool IsContainsLatin(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            foreach (char c in word)
            {
                if (c is >= 'a' and <= 'z' ||
                    c is >= 'A' and <= 'Z' ||
                    c is >= '\u00C0' and <= '\u024F') // Latin-1 Supplement & Extended
                {
                    return true;
                }
            }

            return false;
        }
        
        private static string TrimNonLetterOrDigit(string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            
            int start = 0;
            while (start < word.Length && !char.IsLetterOrDigit(word[start]))
            {
                start++;
            }

            if (start >= word.Length)
                return string.Empty;

            int end = word.Length - 1;
            while (end >= start && !char.IsLetterOrDigit(word[end]))
            {
                end--;
            }

            return word.Substring(start, end - start + 1);
        }
    }
}
