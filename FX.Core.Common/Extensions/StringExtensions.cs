using FX.Core.Common.Settings;
using System.Text;
using System.Text.RegularExpressions;

namespace FX.Core.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns true if the calling string contains inside the compareTo string, case insensitive
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static bool ContainsLowered(this string obj, string compareTo)
        {
            if (compareTo == null)
                return false;

            return obj.ToLower().Trim().Contains(compareTo.ToLower().Trim());
        }

        /// <summary>
        /// Returns true if the two strings are equal (case insensitive)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static bool EqualsLowered(this string obj, string compareTo)
        {
            if (compareTo == null)
                return false;

            return obj.ToLower().Trim() == compareTo.ToLower().Trim();
        }

        /// <summary>
        /// Replaces everything that is not letter, number, space or line in the string with the given character/string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ChrTran(this string s, string c = "_")
        {
            Regex pattern = new Regex("[^a-zA-Z0-9 -]");
            return pattern.Replace(s, c);
        }

        /// <summary>
        /// Replaces everything that does not match the pattern (regex) defined by p, with the given character/string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="p"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static string ChrTran(this string s, string p, string r = "_")
        {
            Regex pattern = new Regex($"[^{p}]");
            return pattern.Replace(s, r);
        }

        /// <summary>
        /// Replaces, in the calling string, every character with the 'replaceWith' character.
        /// If the forbidden chars array is not specified, the default forbidden chars are used.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="forbiddenChars"></param>
        /// <returns></returns>
        public static string ReplaceForbiddenChars(this string input, string replaceWith = "_", string[] forbiddenChars = null)
        {
            var fc = forbiddenChars ?? Constants.DefaultForbiddenChars;
            foreach (var chr in fc)
            {
                input = input.Replace(chr, "_");
            }

            return input;
        }

        /// <summary>
        /// Adds spaces after each word of a sentence
        /// </summary>
        /// <param name="text"></param>
        /// <param name="preserveAcronyms"></param>
        /// <returns></returns>
        public static string AddSpacesToSentence(this string text, bool preserveAcronyms)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);

            for (int i = 1; i < text.Length; i++)
            {
                var thisChar = text[i];
                var previousChar = text[i - 1];

                if (!char.IsUpper(thisChar) && (!char.IsDigit(thisChar) || !(char.IsDigit(thisChar) && !char.IsDigit(previousChar))))
                {
                    newText.Append(text[i]);
                    continue;
                }

                if ((previousChar != ' ' && !char.IsUpper(previousChar)) || (!preserveAcronyms && char.IsUpper(previousChar)))
                    newText.Append(" - ");

                newText.Append(text[i]);
            }
            return newText.ToString();
        }
    }
}
