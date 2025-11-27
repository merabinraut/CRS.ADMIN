using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CRS.ADMIN.APPLICATION.Helper
{
    public static class Helper
    {
        public static string GenerateRandomPassword(int length)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string numberChars = "0123456789";
            const string symbolChars = "!@#$%^&*";
            const string allChars = upperChars + lowerChars + numberChars + symbolChars;

            Random random = new Random();

            // Ensure at least one character of each type
            char upper = upperChars[random.Next(upperChars.Length)];
            char lower = lowerChars[random.Next(lowerChars.Length)];
            char number = numberChars[random.Next(numberChars.Length)];
            char symbol = symbolChars[random.Next(symbolChars.Length)];

            // Generate remaining random characters
            string remainingChars = new string(Enumerable.Repeat(allChars, length - 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // Combine all characters and shuffle
            string password = upper.ToString() + lower.ToString() + number.ToString() + symbol.ToString() + remainingChars;
            return new string(password.ToCharArray().OrderBy(x => random.Next()).ToArray());
        }
    }
}