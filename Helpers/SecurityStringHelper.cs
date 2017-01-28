using System.Collections.Generic;
using System.Security.Cryptography;

namespace PaperCutUtility.Helpers
{
    public static class SecurityStringHelper
    {
        /// <summary>
        /// Generates a random identifier of n length.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateIdentifier(int length)
        {
            string returnString = null;
            char[] availableCharacters = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            };

            if (length > 0)
            {
                char[] identifier = new char[length];
                byte[] randomData = new byte[length];

                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(randomData);

                    for (int i = 0; i < identifier.Length; i++)
                    {
                        int pos = randomData[i] % availableCharacters.Length;
                        identifier[i] = availableCharacters[pos];
                    }
                }

                returnString = new string(identifier);
            }

            return returnString;
        }

        /// <summary>
        /// Generates an array of random identifiers of n length.
        /// </summary>
        /// <param name="lengthOfIdentifier"></param>
        /// <param name="numberOfIdentifiers"></param>
        /// <returns></returns>
        public static List<string> GenerateIdentifiers(int lengthOfIdentifier, int numberOfIdentifiers)
        {
            List<string> identifiers = null;

            if (lengthOfIdentifier > 0 && numberOfIdentifiers > 0)
            {
                identifiers = new List<string>();

                for (int i = 0; i < numberOfIdentifiers; i++)
                {
                    identifiers.Add(GenerateIdentifier(lengthOfIdentifier));
                }
            }

            return identifiers;
        }
    }
}
