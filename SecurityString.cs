using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PaperCutUtility
{
    static class SecurityString
    {
        static readonly char[] AvailableCharacters = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        };

        /// <summary>
        /// Generates a random identifier of n length.
        /// </summary>
        ///
        /// <returns>
        /// A string containing a random identifier.
        /// </returns>
        internal static string GenerateIdentifier(int length)
        {
            char[] identifier = new char[length];
            byte[] randomData = new byte[length];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomData);
            }
            for (int i = 0; i < identifier.Length; i++)
            {
                int pos = randomData[i] % AvailableCharacters.Length;
                identifier[i] = AvailableCharacters[pos];
            }
            return new string(identifier);
        }

        /// <summary>
        /// Generates a number of random identifiers of n length.
        /// </summary>
        ///
        /// <returns>
        /// An array of strings containing random identifiers.
        /// </returns>
        internal static string[] GenerateIdentifiers(int lengthOfIdentifier, int numberOfIdentifiers)
        {
            string[] identifiers = new string[numberOfIdentifiers];
            for (int i = 0; i < numberOfIdentifiers; i++)
            {
                identifiers[i] = GenerateIdentifier(lengthOfIdentifier);
            }
            return identifiers;
        }
    }
}
