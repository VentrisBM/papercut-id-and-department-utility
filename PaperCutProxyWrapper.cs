using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaperCutUtility
{
    static class PaperCutProxyWrapper
    {
        /// <summary>
        /// Checks if the provided ServerCommandProxy instance is valid.
        /// </summary>
        ///
        /// <returns>
        /// True if a connection has been successfully established.
        /// </returns>
        internal static bool ConnectionEstablished(ServerCommandProxy serverProxy)
        {
            if (serverProxy.GetTotalUsers() != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }   // end ConnectionEstablished

        /// <summary>
        /// Returns a users card number.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the users card number, 
        /// or a blank string if the operation was unsuccessful.
        /// </returns>
        internal static string GetCardNumber(ServerCommandProxy serverProxy, string username, int cardFieldToRetrieve)
        {
            string cardNumber = "";
            string fieldToRetrieve = PaperCutProxyWrapper.ResolveCardField(cardFieldToRetrieve);

            try
            {
                cardNumber = serverProxy.GetUserProperty(username, fieldToRetrieve);
                return cardNumber;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return cardNumber;
            }
        }   // end GetCardNumber

        /// <summary>
        /// Returns all of the existing card numbers from PaperCut.
        /// </summary>
        ///
        /// <returns>
        /// An array of strings that contains all of the existing card 
        /// numbers for all of the users in PaperCut.
        /// </returns>
        internal static string[] GetCardNumbers(ServerCommandProxy serverProxy, string[] usernames, int cardFieldToRetrieve)
        {
            string[] cardNumbers = new string[usernames.Length];
            string fieldToRetrieve = PaperCutProxyWrapper.ResolveCardField(cardFieldToRetrieve);

            try
            {
                Console.WriteLine("Retrieving existing card numbers...");
                Console.WriteLine("########################################");
                for (int i = 0; i < usernames.Length; i++)
                {
                    string currentCard = serverProxy.GetUserProperty(usernames[i], fieldToRetrieve);
                    cardNumbers[i] = currentCard.ToString();
                    if (i != 0 && i % 100 == 0)
                    {
                        Console.WriteLine("Retrieved {0} card numbers so far...", i);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("########################################\r\n");
            return cardNumbers;
        }

        /// <summary>
        /// Returns a users email address.
        /// </summary>
        ///
        /// <returns>
        /// A string containing the users email address, 
        /// or a blank string if the operation was unsuccessful.
        /// </returns>
        internal static string GetEmailAddress(ServerCommandProxy serverProxy, string username)
        {
            string emailAddress = "";

            try
            {
                emailAddress = serverProxy.GetUserProperty(username, "email");
                return emailAddress;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return emailAddress;
            }
        }

        /// <summary>
        /// Returns all of the existing email addresses from PaperCut.
        /// </summary>
        ///
        /// <returns>
        /// An array of strings that contains all of the existing email 
        /// addresses for all of the users in PaperCut.
        /// </returns>
        internal static string[] GetEmailAddresses(ServerCommandProxy serverProxy, string[] usernames)
        {
            string[] emailAddresses = new string[usernames.Length];

            try
            {
                Console.WriteLine("Retrieving email addresses...");
                Console.WriteLine("########################################");
                for (int i = 0; i < usernames.Length; i++)
                {
                    string currentEmail = serverProxy.GetUserProperty(usernames[i], "email");
                    emailAddresses[i] = currentEmail.ToString();
                    if (i != 0 && i % 100 == 0)
                    {
                        Console.WriteLine("Retrieved {0} email addresses so far...", i);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("########################################\r\n");
            return emailAddresses;
        }

        /// <summary>
        /// Clears the card number for a specific user.
        /// </summary>
        internal static void ClearCardNumber(ServerCommandProxy serverProxy, string username, int cardFieldToUpdate)
        {
            string fieldToUpdate = PaperCutProxyWrapper.ResolveCardField(cardFieldToUpdate);
            try
            {
                Console.WriteLine("Clearing {0} for username {1}\r\n.)", fieldToUpdate, username);
                serverProxy.SetUserProperty(username, fieldToUpdate, "");
                Console.WriteLine("Clear successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }   // endClearCardNumber

        /// <summary>
        /// Clears the card numbers all of users.
        /// </summary>
        internal static void ClearCardNumbers(ServerCommandProxy serverProxy, string[] usernames, int cardFieldToUpdate)
        {
            string fieldToUpdate = PaperCutProxyWrapper.ResolveCardField(cardFieldToUpdate);
            int clearCount = 0;

            try
            {
                Console.WriteLine("Clearing {0} for {0} usernames\r\n.)", fieldToUpdate, usernames.Length);
                for (int i = 0; i < usernames.Length; i++)
                {
                    if (i != 0 && i % 100 == 0)
                    {
                        Console.WriteLine("Cleared {0} card numbers so far...", i);
                        clearCount++;
                    }
                    serverProxy.SetUserProperty(usernames[i], fieldToUpdate, "");
                }
                Console.WriteLine("Cleared {0} card numbers.", clearCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }   // end ClearCardNumbers

        /// <summary>
        /// Sets the ID number for a specific user.
        /// </summary>
        internal static void SetCardNumber(ServerCommandProxy serverProxy, string username, string cardNumber, int cardFieldToUpdate)
        {
            string fieldToUpdate = PaperCutProxyWrapper.ResolveCardField(cardFieldToUpdate);
            try
            {
                serverProxy.SetUserProperty(username, fieldToUpdate, cardNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }   // endSetCardNumber

        /// <summary>
        /// Sets the ID numbers for all users.
        /// </summary>
        internal static void SetCardNumbers(ServerCommandProxy serverProxy, string[] usernames, string[] cardNumbers, int cardFieldToUpdate)
        {
            string fieldToUpdate = PaperCutProxyWrapper.ResolveCardField(cardFieldToUpdate);

            try
            {
                for (int i = 0; i < usernames.Length; i++)
                {
                    serverProxy.SetUserProperty(usernames[i], fieldToUpdate, cardNumbers[i]);
                    Console.WriteLine("#{0}/{1} updated username: {2}, new card number: {3}", i + 1, usernames.Length, usernames[i], cardNumbers[i]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }   // end SetCardNumbers

        /// <summary>
        /// Returns all of the users in PaperCut.
        /// </summary>
        ///
        /// <returns>
        /// An array of strings that contains all of the usernames for all of the users in PaperCut.
        /// </returns>
        internal static string[] GetUserAccounts(ServerCommandProxy serverProxy)
        {
            string[] usersInBatch;
            string[] allUsers;
            int offset;
            int totalUsers;
            int remainingUsers;
            int maxUsersInBatch;
            int j;

            offset = 0;
            totalUsers = serverProxy.GetTotalUsers();
            maxUsersInBatch = 1000;
            j = 0;
            allUsers = new string[totalUsers];
            remainingUsers = totalUsers;

            if (totalUsers <= 1000)
            {
                allUsers = serverProxy.ListUserAccounts(offset, totalUsers);
                return allUsers;
            }
            else
            {
                do
                {
                    usersInBatch = serverProxy.ListUserAccounts(offset, maxUsersInBatch);

                    foreach (string user in usersInBatch)
                    {
                        allUsers[j] = user.ToString();
                        j++;
                    }

                    remainingUsers = remainingUsers - usersInBatch.Length;
                    offset += maxUsersInBatch;

                    if (remainingUsers >= 1000)
                    {
                        maxUsersInBatch = 1000;
                    }
                    else
                    {
                        maxUsersInBatch = remainingUsers;
                    }
                } while (remainingUsers > 0);
            }
            return allUsers;
        }   // end GetUserAccounts


        /// <summary>
        /// Resolves which cardField must be updated.
        /// </summary>
        ///
        /// <returns>
        /// A string that contains the correct card property name.
        /// </returns>
        internal static string ResolveCardField(int cardFieldToUpdate)
        {
            string fieldToUpdate = "";
            switch (cardFieldToUpdate)
            {
                case 0:
                    fieldToUpdate = "primary-card-number";
                    break;
                case 1:
                    fieldToUpdate = "secondary-card-number";
                    break;
            }
            return fieldToUpdate;
        }

    }   // end class
}
