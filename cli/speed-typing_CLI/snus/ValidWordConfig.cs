using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextHandle
{
    public struct ValidWordConfig
    {
        public int MaxLength { get; }
        public int MinLength { get; }
        public string[] BannedWords { get; }
        public char[] CharsToRemove { get; }

        public ValidWordConfig()
        {
            MaxLength = 20;
            MinLength = 2;
            CharsToRemove = new char[]{ '.', ',', ':', '!', '?' };
            BannedWords = new string[] { };
        }

        public ValidWordConfig(int maxLength, int minLength, string[] bannedWords, char[] charsToRemove)
        {
            MaxLength = maxLength;
            MinLength = minLength;
            BannedWords = bannedWords;
            CharsToRemove = charsToRemove;
        }

        public bool IsWordValid(string? word)
        {
            if (word == null ||
                MaxLength <= word.Length ||
                MinLength >= word.Length ||
                BannedWords.Contains(word))
            {
                return false;
            }

            foreach (char c in word)
            {
                if (!Char.IsAsciiLetter(c)) return false;
            }

            return true;
        }
    }
}
