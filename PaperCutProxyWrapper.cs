using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaperCutUtility.Models;

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
                Console.WriteLine("########################################\n");
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

            Console.WriteLine("\n########################################\r\n");
            return cardNumbers;
        }   // end GetCardNumbers

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
        }   // end GetEmailAddress

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
        }   // end GetEmailAddresses

        /// <summary>
        /// Clears the card number for a specific user.
        /// </summary>
        internal static void ClearCardNumber(ServerCommandProxy serverProxy, string username, int cardFieldToUpdate)
        {
            string fieldToUpdate = PaperCutProxyWrapper.ResolveCardField(cardFieldToUpdate);
            try
            {
                Console.WriteLine("Clearing {0} for username {1}.)", fieldToUpdate, username);
                Console.WriteLine("########################################\r\n");
                serverProxy.SetUserProperty(username, fieldToUpdate, "");
                Console.WriteLine("Clear successful.");
                Console.WriteLine("########################################\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }   // end ClearCardNumber

        /// <summary>
        /// Clears the card numbers all of users.
        /// </summary>
        internal static void ClearCardNumbers(ServerCommandProxy serverProxy, int cardFieldToUpdate)
        {
            string[] usernames = PaperCutProxyWrapper.GetUserAccounts(serverProxy);
            string fieldToUpdate = PaperCutProxyWrapper.ResolveCardField(cardFieldToUpdate);
            int clearCount = 0;

            try
            {
                Console.WriteLine("Clearing {0} for {0} users.)", fieldToUpdate, usernames.Length);
                Console.WriteLine("########################################\r\n");
                for (int i = 0; i < usernames.Length; i++)
                {
                    if (i != 0 && i % 100 == 0)
                    {
                        Console.WriteLine("Cleared {0} card numbers so far...", i);
                    }
                    serverProxy.SetUserProperty(usernames[i], fieldToUpdate, "");
                    clearCount++;
                }
                Console.WriteLine("\nCleared card numbers for {0} users.", clearCount);
                Console.WriteLine("########################################\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }   // end ClearCardNumbers

        /// <summary>
        /// Clears the selected department information for all users.
        /// </summary>
        internal static void ClearUsersDepartmentInfo(ServerCommandProxy serverProxy, int targetDepartmentField)
        {
            string[] users = PaperCutProxyWrapper.GetUserAccounts(serverProxy);
            string fieldToUpdate = PaperCutProxyWrapper.ResolveDepartmentField(targetDepartmentField);
            int clearCount = 0;

            try
            {
                Console.WriteLine("Clearing {0} for {1} users...", fieldToUpdate, users.Length);
                Console.WriteLine("########################################\r\n");
                for (int i = 0; i < users.Length; i++)
                {
                    if (i != 0 && i % 100 == 0)
                    {
                        Console.WriteLine("Cleared {0} {1} properties so far...", i, fieldToUpdate);
                    }
                    serverProxy.SetUserProperty(users[i], fieldToUpdate, "");
                    clearCount++;
                }
                Console.WriteLine("\nCleared {0}s for {1} users.", fieldToUpdate, clearCount);
                Console.WriteLine("########################################\r\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }   // end ClearUsersDepartmentInfo

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
        }   // end SetCardNumber

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
        /// Returns all of the users in PaperCut, 
        /// as well as their departmental information.
        /// </summary>
        ///
        /// <returns>
        /// An array of PpcUser that contains all the users
        ///  in PaperCut and their department information.
        /// </returns>
        internal static PpcUser[] GetUsersDepartmentInfo(ServerCommandProxy serverProxy)
        {
            string[] allUsers = GetUserAccounts(serverProxy);
            PpcUser[] allUsersWithDeptInfo = new PpcUser[allUsers.Length];
            string[] propertiesToFetch = { "department", "office" };
            string[] retrievedProperties = new string[2];

            Console.WriteLine("Retrieving department information \nfor {0} PaperCut users...", allUsers.Length);
            Console.WriteLine("########################################\r\n");
            for (int i = 0; i < allUsersWithDeptInfo.Length; i++)
            {
                try
                {
                    allUsersWithDeptInfo[i] = new PpcUser();
                    allUsersWithDeptInfo[i].Username = allUsers[i];
                    retrievedProperties = serverProxy.GetUserProperties(allUsersWithDeptInfo[i].Username, propertiesToFetch);

                    allUsersWithDeptInfo[i].Department = retrievedProperties[0];
                    allUsersWithDeptInfo[i].Department = retrievedProperties[1];

                    if (i != 0 && i % 100 == 0)
                    {
                        Console.WriteLine("Retrieved {0} users so far...", i);
                    }
                    
                    if (i == allUsersWithDeptInfo.Length - 1)
                    {
                        Console.WriteLine("\nRetrieved {0} users.", allUsersWithDeptInfo.Length);
                        Console.WriteLine("########################################\r\n");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return allUsersWithDeptInfo;
        }   // end GetUsersDepartmentInfo

        /// <summary>
        /// Combines and sets multiple department information for all users in papercut.
        /// </summary>
        internal static void SetUsersMultipleDepartmentInfo(ServerCommandProxy serverProxy, LdapUser[] ldapUsers, int targetDepartmentField)
        {
            string fieldToUpdate = PaperCutProxyWrapper.ResolveDepartmentField(targetDepartmentField);

            try
            {
                for (int i = 0; i < ldapUsers.Length; i++)
                {
                    string combinedDepartment = ldapUsers[i].DepartmentNumber + " - " + ldapUsers[i].DepartmentName;
                    serverProxy.SetUserProperty(ldapUsers[i].Username, fieldToUpdate, combinedDepartment);
                    Console.WriteLine("#{0}/{1} updated username: {2}, new department: {3}",
                        i + 1, ldapUsers.Length, ldapUsers[i].Username, combinedDepartment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.WriteLine("\nUpdated {0} users with new card departments.", ldapUsers.Length);
            Console.WriteLine("########################################\r\n");
        }   // end SetUsersMultipleDepartmentInfo

        /// <summary>
        /// Sets a single department information for all users in papercut.
        /// </summary>
        internal static void SetUsersSingleDepartmentInfo(ServerCommandProxy serverProxy, LdapUser[] ldapUsers, int targetDepartmentField)
        {
            string fieldToUpdate = PaperCutProxyWrapper.ResolveDepartmentField(targetDepartmentField);

            try
            {
                for (int i = 0; i < ldapUsers.Length; i++)
                {
                    serverProxy.SetUserProperty(ldapUsers[i].Username, fieldToUpdate, ldapUsers[i].DepartmentName);
                    Console.WriteLine("#{0}/{1} updated username: {2}, new department: {3}",
                        i + 1, ldapUsers.Length, ldapUsers[i].Username, ldapUsers[i].DepartmentName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            Console.WriteLine("\n########################################\r\n");
        }   // end SetUsersSingleDepartmentInfo

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
        }   // end ResolveCardField

        /// <summary>
        /// Resolves which department field must be updated.
        /// </summary>
        ///
        /// <returns>
        /// A string that contains the correct department property name.
        /// </returns>
        internal static string ResolveDepartmentField(int cardFieldToUpdate)
        {
            string fieldToUpdate = "";
            switch (cardFieldToUpdate)
            {
                case 0:
                    fieldToUpdate = "department";
                    break;
                case 1:
                    fieldToUpdate = "office";
                    break;
            }
            return fieldToUpdate;
        }   // end ResolveDepartmentField
    }   // end class PapercutProxyWrapper
}
