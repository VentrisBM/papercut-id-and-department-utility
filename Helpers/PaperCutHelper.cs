using System;
using System.Net.Mail;
using System.Collections.Generic;

using PaperCutUtility.Wrappers;

namespace PaperCutUtility.Helpers
{
    internal class PaperCutHelper
    {
        #region Members
        private ServerCommandProxy _serverProxy;
        #endregion

        public PaperCutHelper(ServerCommandProxy serverProxy)
        {
            _serverProxy = serverProxy;
        }

        #region Public methods
        /// <summary>
        /// Updates a single users card number.
        /// </summary>
        /// <param name="updateOnlyIfBlank"></param>
        /// <param name="targetIdField"></param>
        /// <param name="targetUsername"></param>
        /// <param name="numberOfChars"></param>
        public void UpdateSingleCardNumber(bool updateOnlyIfBlank, int targetIdField, string targetUsername, int numberOfChars)
        {
            try
            {
                if (!string.IsNullOrEmpty(targetUsername) &&
                    numberOfChars >= Common.Constants.PaperCut.Constants.MinimumNumberOfCharacters &&
                    numberOfChars <= Common.Constants.PaperCut.Constants.MaximumNumberOfCharacters)
                {
                    string newCardNumber = SecurityStringHelper.GenerateIdentifier(numberOfChars);

                    if (!string.IsNullOrEmpty(newCardNumber))
                    {
                        if (updateOnlyIfBlank)
                        {
                            string existingCardNumber = PaperCutProxyWrapper.GetCardNumber(_serverProxy, targetUsername, targetIdField);
                            
                            if (string.IsNullOrEmpty(existingCardNumber))
                            {
                                PaperCutProxyWrapper.SetCardNumber(_serverProxy, targetUsername, newCardNumber, targetIdField);
                                Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedUserWithNewCardNumber,
                                                    targetUsername, newCardNumber));
                                Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);
                            }
                            else
                            {
                                Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.CannotUpdateUserDueToExistingCardNumber,
                                                    targetUsername, existingCardNumber));
                                Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);
                            }
                        }
                        else
                        {
                            PaperCutProxyWrapper.SetCardNumber(_serverProxy, targetUsername, newCardNumber, targetIdField);
                            Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedUserWithNewCardNumber,
                                                    targetUsername, newCardNumber));
                            Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Updates the card numbers for all users.
        /// </summary>
        /// <param name="updateOnlyIfBlank"></param>
        /// <param name="targetIdField"></param>
        /// <param name="numberOfChars"></param>
        public void UpdateAllCardNumbers(bool updateOnlyIfBlank, int targetIdField, int numberOfChars)
        {
            try
            {
                if (numberOfChars >= Common.Constants.PaperCut.Constants.MinimumNumberOfCharacters &&
                    numberOfChars <= Common.Constants.PaperCut.Constants.MaximumNumberOfCharacters)
                {
                    string[] existingUsers = PaperCutProxyWrapper.GetUserAccounts(_serverProxy);
                    string[] newCardNumbers = new string[existingUsers.Length];
                    newCardNumbers = SecurityStringHelper.GenerateIdentifiers(numberOfChars, newCardNumbers.Length).ToArray();

                    if (updateOnlyIfBlank)
                    {
                        int updatedCount = 0;
                        int skippedCount = 0;
                        string[] existingCardNumbers;

                        existingCardNumbers = PaperCutProxyWrapper.GetCardNumbers(_serverProxy, existingUsers, targetIdField);

                        Console.WriteLine(Common.Constants.PaperCut.Messages.UpdatingOnlyUsersWithBlankCardNumbers);
                        Console.WriteLine(Common.Constants.ConsoleSpacing.HashesWithNewLine);

                        for (int i = 0; i < existingUsers.Length; i++)
                        {
                            if (string.IsNullOrEmpty(existingCardNumbers[i]))
                            {
                                PaperCutProxyWrapper.SetCardNumber(_serverProxy, existingUsers[i], newCardNumbers[i], targetIdField);

                                updatedCount++;

                                if (updatedCount > 0 && updatedCount % 100 == 0)
                                {
                                    Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedNUsersSoFar, updatedCount));
                                }
                            }
                            else
                            {
                                skippedCount++;

                                if (skippedCount > 0 && skippedCount % 100 == 0)
                                {
                                    Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.SkippedNUsersSoFar, skippedCount));
                                }
                            }
                        }

                        Console.WriteLine();
                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedAndSkippedNUsers, updatedCount, skippedCount));
                        Console.WriteLine(Common.Constants.ConsoleSpacing.Hashes);
                    }
                    else
                    {
                        PaperCutProxyWrapper.SetCardNumbers(_serverProxy, existingUsers, newCardNumbers, targetIdField);

                        Console.WriteLine();
                        Console.WriteLine(string.Format(Common.Constants.PaperCut.Messages.UpdatedNUsers, existingUsers.Length));
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
        /// Sends an email notification
        /// with the card number to a single user.
        /// </summary>
        /// <param name="smtpHostname"></param>
        /// <param name="targetUsername"></param>
        /// <param name="senderAddress"></param>
        /// <param name="targetIdField"></param>
        public void SendSingleEmail(string smtpHostname, string targetUsername, string senderAddress, int targetIdField)
        {
            string resolvedIdField = PaperCutProxyWrapper.ResolveCardField(targetIdField);
            string targetEmail = _serverProxy.GetUserProperty(targetUsername, Common.Constants.PaperCut.Properties.Email);
            string retrievedId = _serverProxy.GetUserProperty(targetUsername, resolvedIdField);

            try
            {
                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = smtpHostname;
                smtpClient.Port = 25;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;

                SendEmail(smtpClient, senderAddress, targetEmail, retrievedId);
            }
            catch (Exception)
            {
                Console.WriteLine(Common.Constants.Smtp.Email.UnableToSend);
            }
        }

        /// <summary>
        /// Sends an email notification
        /// with the card number to all users.
        /// </summary>
        /// <param name="smtpHostname"></param>
        /// <param name="senderAddress"></param>
        /// <param name="targetIdField"></param>
        public void SendEmailsToAllUsers(string smtpHostname, string senderAddress, int targetIdField)
        {
            try
            {
                if (!string.IsNullOrEmpty(smtpHostname) && !string.IsNullOrEmpty(senderAddress))
                {
                    string resolvedIdField = PaperCutProxyWrapper.ResolveCardField(targetIdField);
                    string[] usernames = PaperCutProxyWrapper.GetUserAccounts(_serverProxy);

                    List<string> emails = null;
                    List<string> idNumbers = null;

                    for (int i = 0; i < usernames.Length; i++)
                    {
                        string[] retrievedProperties = _serverProxy.GetUserProperties(usernames[i],
                            new string[] { Common.Constants.PaperCut.Properties.Email, resolvedIdField });

                        emails.Add(retrievedProperties[0]);
                        idNumbers.Add(retrievedProperties[1]);
                    }

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.Host = smtpHostname;
                    smtpClient.Port = 25;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;

                    for (int i = 0; i < usernames.Length; i++)
                    {
                        SendEmail(smtpClient, senderAddress, emails[i], idNumbers[i]);
                    }
                }
            }
            catch
            {
                Console.WriteLine(Common.Constants.Smtp.Email.UnableToSend);
            }
        }
        #endregion

        #region Private methods
        private void SendEmail(SmtpClient smtpClient, string senderAddress, string recipientAddress, string idNumber)
        {
            if (smtpClient != null)
            {
                MailMessage mail = new MailMessage(senderAddress, recipientAddress);
                mail.Subject = Common.Constants.Smtp.Email.Subject;
                mail.Body = string.Format(Common.Constants.Smtp.Email.Body, idNumber);
                smtpClient.Send(mail);
            }
        }
        #endregion
    }
}
