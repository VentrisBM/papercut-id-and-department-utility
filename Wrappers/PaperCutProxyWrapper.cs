using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PaperCutUtility.Models;

namespace PaperCutUtility.Wrappers
{
    public static class PaperCutProxyWrapper
    {
        /// <summary>
        /// Returns a string containing the users card number, 
        /// or a blank string if nothing was retrieved.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="username"></param>
        /// <param name="cardFieldToRetrieve"></param>
        /// <returns></returns>
        public static string GetCardNumber(ServerCommandProxy serverProxy, string username, int cardFieldToRetrieve)
        {
            string cardNumber = null;
            string fieldToRetrieve = ResolveCardField(cardFieldToRetrieve);

            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    cardNumber = serverProxy.GetUserProperty(username, fieldToRetrieve);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return cardNumber;
        }

        /// <summary>
        /// Returns an array of strings that contains all of the existing card 
        /// numbers for all of the users in PaperCut.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="usernames"></param>
        /// <param name="cardFieldToRetrieve"></param>
        /// <returns></returns>
        public static string[] GetCardNumbers(ServerCommandProxy serverProxy, string[] usernames, int cardFieldToRetrieve)
        {
            List<string> cardNumbers = null;
            string fieldToRetrieve = ResolveCardField(cardFieldToRetrieve);

            try
            {
                if (serverProxy != null && usernames.Any())
                {
                    Console.WriteLine(Common.Constants.PaperCut.Messages.RetrievingExistingCardNumbers);
                    Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);

                    for (int i = 0; i < usernames.Length; i++)
                    {
                        string currentCard = serverProxy.GetUserProperty(usernames[i], fieldToRetrieve);
                        cardNumbers.Add(currentCard);

                        if (i != 0 && i % 100 == 0)
                        {
                            Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.RetrievedNCardNumbersSoFar, i));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);

            return cardNumbers != null ? cardNumbers.ToArray() : null;
        }

        /// <summary>
        /// Returns a string containing the users email address, 
        /// or a blank string if nothing was retrieved.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GetEmailAddress(ServerCommandProxy serverProxy, string username)
        {
            string emailAddress = null;

            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    emailAddress = serverProxy.GetUserProperty(username, Common.Constants.PaperCut.Properties.Email);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return emailAddress;
        }

        /// <summary>
        /// Returns an array of strings that contains all of the existing email 
        /// addresses for all of the users in PaperCut.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="usernames"></param>
        /// <returns></returns>
        public static List<string> GetEmailAddresses(ServerCommandProxy serverProxy, List<string> usernames)
        {
            List<string> emailAddresses = null;

            try
            {
                if (serverProxy != null && usernames.Any())
                {
                    Console.WriteLine(Common.Constants.PaperCut.Messages.RetrievingEmailAddresses);
                    Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);

                    for (int i = 0; i < usernames.Count; i++)
                    {
                        string currentEmail = serverProxy.GetUserProperty(usernames[i], Common.Constants.PaperCut.Properties.Email);
                        emailAddresses.Add(currentEmail);

                        if (i != 0 && i % 100 == 0)
                        {
                            Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.RetrievedNEmailAddressesSoFar, i));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);

            return emailAddresses;
        }

        /// <summary>
        /// Clears the card number for a specific user.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="username"></param>
        /// <param name="cardFieldToUpdate"></param>
        public static void ClearCardNumber(ServerCommandProxy serverProxy, string username, int cardFieldToUpdate)
        {
            try
            {
                if (serverProxy != null && !string.IsNullOrEmpty(username))
                {
                    string fieldToUpdate = ResolveCardField(cardFieldToUpdate);

                    Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.ClearingCardNumberForUsername, fieldToUpdate, username));
                    Console.WriteLine(Common.Constants.ConsoleSpacing.Hashes);

                    serverProxy.SetUserProperty(username, fieldToUpdate, string.Empty);

                    Console.WriteLine(Common.Constants.PaperCut.Messages.ClearSuccessful);
                    Console.WriteLine(Common.Constants.ConsoleSpacing.Hashes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Clears the card numbers all of users.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="cardFieldToUpdate"></param>
        public static void ClearCardNumbers(ServerCommandProxy serverProxy, int cardFieldToUpdate)
        {
            try
            {
                if (serverProxy != null)
                {
                    string[] usernames = GetUserAccounts(serverProxy);
                    string fieldToUpdate = ResolveCardField(cardFieldToUpdate);
                    int clearCount = 0;

                    if (usernames.Any())
                    {
                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.ClearingPropertyForNUsers, fieldToUpdate, usernames.Length));
                        Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);

                        for (int i = 0; i < usernames.Length; i++)
                        {
                            if (i != 0 && i % 100 == 0)
                            {
                                Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.ClearedNCardNumbersSoFar, i));
                            }

                            serverProxy.SetUserProperty(usernames[i], fieldToUpdate, string.Empty);
                            clearCount++;
                        }

                        Console.WriteLine();
                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.ClearedCardNumbersForNUsers, clearCount));
                        Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Clears the selected department information for all users.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="targetDepartmentField"></param>
        public static void ClearUsersDepartmentInfo(ServerCommandProxy serverProxy, int targetDepartmentField)
        {
            try
            {
                if (serverProxy != null)
                {
                    string[] users = GetUserAccounts(serverProxy);
                    string fieldToUpdate = ResolveDepartmentField(targetDepartmentField);
                    int clearCount = 0;

                    if (users.Any())
                    {
                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.ClearingPropertyForNUsers, fieldToUpdate, users.Length));
                        Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);

                        for (int i = 0; i < users.Length; i++)
                        {
                            if (i != 0 && i % 100 == 0)
                            {
                                Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.ClearedNPropertiesSoFar, i, fieldToUpdate));
                            }

                            serverProxy.SetUserProperty(users[i], fieldToUpdate, string.Empty);
                            clearCount++;
                        }

                        Console.WriteLine();
                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.ClearedPropertiesForNUsers, fieldToUpdate, clearCount));
                        Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Sets the ID number for a specific user.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="username"></param>
        /// <param name="cardNumber"></param>
        /// <param name="cardFieldToUpdate"></param>
        public static void SetCardNumber(ServerCommandProxy serverProxy, string username, string cardNumber, int cardFieldToUpdate)
        {
            try
            {
                if (serverProxy != null && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(cardNumber))
                {
                    string fieldToUpdate = ResolveCardField(cardFieldToUpdate);
                    serverProxy.SetUserProperty(username, fieldToUpdate, cardNumber);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Sets the ID numbers for all users.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="usernames"></param>
        /// <param name="cardNumbers"></param>
        /// <param name="cardFieldToUpdate"></param>
        public static void SetCardNumbers(ServerCommandProxy serverProxy, string[] usernames, string[] cardNumbers, int cardFieldToUpdate)
        {
            try
            {
                if (serverProxy != null && usernames.Any() && cardNumbers.Any() && (usernames.Length == cardNumbers.Length))
                {
                    string fieldToUpdate = ResolveCardField(cardFieldToUpdate);

                    for (int i = 0; i < usernames.Length; i++)
                    {
                        serverProxy.SetUserProperty(usernames[i], fieldToUpdate, cardNumbers[i]);

                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedCardNumberProgress,
                                            i + 1, usernames.Length, usernames[i], cardNumbers[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Returns an array of strings that contains all
        /// of the usernames for all of the users in PaperCut.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <returns></returns>
        public static string[] GetUserAccounts(ServerCommandProxy serverProxy)
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
        }

        /// <summary>
        /// Returns an array of PaperCutUser that contains all the users
        /// in PaperCut and their department information.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <returns></returns>
        public static PaperCutUser[] GetUsersDepartmentInfo(ServerCommandProxy serverProxy)
        {
            string[] allUsers = GetUserAccounts(serverProxy);
            PaperCutUser[] allUsersWithDeptInfo = new PaperCutUser[allUsers.Length];

            string[] retrievedProperties = new string[2];
            string[] propertiesToRetrieve =
            {
                Common.Constants.PaperCut.Properties.Department,
                Common.Constants.PaperCut.Properties.Office
            };

            Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.RetrievingDepartmentInformationForNUsers, allUsers.Length));
            Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);

            for (int i = 0; i < allUsersWithDeptInfo.Length; i++)
            {
                try
                {
                    allUsersWithDeptInfo[i] = new PaperCutUser();
                    allUsersWithDeptInfo[i].Username = allUsers[i];
                    retrievedProperties = serverProxy.GetUserProperties(allUsersWithDeptInfo[i].Username, propertiesToRetrieve);

                    allUsersWithDeptInfo[i].Department = retrievedProperties[0];
                    allUsersWithDeptInfo[i].Department = retrievedProperties[1];

                    if (i != 0 && i % 100 == 0)
                    {
                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.RetrievedNUsersSoFar, i));
                    }

                    if (i == allUsersWithDeptInfo.Length - 1)
                    {
                        Console.WriteLine();
                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.RetrievedNUsers, allUsersWithDeptInfo.Length));
                        Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return allUsersWithDeptInfo;
        }

        /// <summary>
        /// Combines and sets multiple department information for all users in papercut.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="ldapUsers"></param>
        /// <param name="targetDepartmentField"></param>
        public static void SetUsersMultipleDepartmentInfo(ServerCommandProxy serverProxy, LdapUser[] ldapUsers, int targetDepartmentField)
        {
            try
            {
                if (serverProxy != null && ldapUsers.Any())
                {
                    string fieldToUpdate = ResolveDepartmentField(targetDepartmentField);

                    for (int i = 0; i < ldapUsers.Length; i++)
                    {
                        string combinedDepartment = string.Format("{0} - {1}", ldapUsers[i].DepartmentNumber, ldapUsers[i].DepartmentName);
                        serverProxy.SetUserProperty(ldapUsers[i].Username, fieldToUpdate, combinedDepartment);

                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedDeparmentProgress,
                                            i + 1, ldapUsers.Length, ldapUsers[i].Username, combinedDepartment));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine();
            Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedNUsersWithNewCardDepartments, ldapUsers.Length));
            Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);
        }

        /// <summary>
        /// Sets a single department information for all users in PaperCut.
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <param name="ldapUsers"></param>
        /// <param name="targetDepartmentField"></param>
        public static void SetUsersSingleDepartmentInfo(ServerCommandProxy serverProxy, LdapUser[] ldapUsers, int targetDepartmentField)
        {
            try
            {
                if (serverProxy != null && ldapUsers.Any())
                {
                    string fieldToUpdate = ResolveDepartmentField(targetDepartmentField);

                    for (int i = 0; i < ldapUsers.Length; i++)
                    {
                        serverProxy.SetUserProperty(ldapUsers[i].Username, fieldToUpdate, ldapUsers[i].DepartmentName);

                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedDeparmentProgress,
                                            i + 1, ldapUsers.Length, ldapUsers[i].Username, ldapUsers[i].DepartmentName));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine();
            Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);
        }

        /// <summary>
        /// Resolves which cardField must be updated.
        /// </summary>
        /// <param name="cardFieldToUpdate"></param>
        /// <returns></returns>
        public static string ResolveCardField(int cardFieldToUpdate)
        {
            string targetField = null;

            switch (cardFieldToUpdate)
            {
                case 1:
                    targetField = Common.Constants.PaperCut.Properties.SecondaryCardNumber;
                    break;
                case 0:
                default:
                    targetField = Common.Constants.PaperCut.Properties.PrimaryCardNumber;
                    break;
            }

            return targetField;
        }

        /// <summary>
        /// Resolves which department field must be updated.
        /// </summary>
        /// <param name="cardFieldToUpdate"></param>
        /// <returns></returns>
        public static string ResolveDepartmentField(int departmentFieldToUpdate)
        {
            string targetField = null;

            switch (departmentFieldToUpdate)
            {
                case 1:
                    targetField = Common.Constants.PaperCut.Properties.Office;
                    break;
                case 0:
                default:
                    targetField = Common.Constants.PaperCut.Properties.Department;
                    break;
            }

            return targetField;
        }

        /// <summary>
        /// Returns a boolean indicating if the connection has been established
        /// </summary>
        /// <param name="serverProxy"></param>
        /// <returns></returns>
        public static bool IsConnectionEstablished(ServerCommandProxy serverProxy)
        {
            bool isConnectionEstablished = false;

            if (serverProxy.GetTotalUsers() > 0)
            {
                isConnectionEstablished = true;
            }

            return isConnectionEstablished;
        }
    }
}
