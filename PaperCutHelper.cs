using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PaperCutUtility
{
    class PaperCutHelper
    {
        ServerCommandProxy _serverProxy;

        public PaperCutHelper(ServerCommandProxy serverProxy)
        {
            this._serverProxy = serverProxy;
        }

        /// <summary>
        /// Updates a single users card number.
        /// </summary>
        public void UpdateSingleCardNumber(bool updateOnlyIfBlank, int targetIdField, string targetUsername, int numberOfChars)
        {
            string newCardNumber = SecurityString.GenerateIdentifier(numberOfChars);

            if (updateOnlyIfBlank)
            {
                string existingCardNumber = PaperCutProxyWrapper.GetCardNumber(_serverProxy, targetUsername, targetIdField);
                if (String.IsNullOrEmpty(existingCardNumber))
                {
                    PaperCutProxyWrapper.SetCardNumber(_serverProxy, targetUsername, newCardNumber, targetIdField);
                    Console.WriteLine("Updated user: {0}, new card number: {1}\r\n", 
                        targetUsername, newCardNumber);
                    Console.WriteLine("########################################\r\n");
                }
                else
                {
                    Console.WriteLine("Cannot update user: {0} due to existing card number: {1}\r\n",
                        targetUsername, existingCardNumber);
                    Console.WriteLine("########################################\r\n");
                }
            }
            else
            {
                PaperCutProxyWrapper.SetCardNumber(_serverProxy, targetUsername, newCardNumber, targetIdField);
                Console.WriteLine("Updated user: {0}, new card number: {1}\r\n", targetUsername, newCardNumber);
                Console.WriteLine("\n########################################\r\n");
            }
        }   // end UpdateSingleCardNumber

        /// <summary>
        /// Updates the card numbers for all users.
        /// </summary>
        public void UpdateAllCardNumbers(bool updateOnlyIfBlank, int targetIdField, int numberOfChars)
        {
            string[] existingUsers = PaperCutProxyWrapper.GetUserAccounts(_serverProxy);
            string[] newCardNumbers = new string[existingUsers.Length];
            newCardNumbers = SecurityString.GenerateIdentifiers(numberOfChars, newCardNumbers.Length);

            if (updateOnlyIfBlank)
            {
                int updatedCount = 0;
                int skippedCount = 0;
                string[] existingCardNumbers;
                existingCardNumbers = PaperCutProxyWrapper.GetCardNumbers(_serverProxy, existingUsers, targetIdField);

                Console.WriteLine("Checking users and updating only\r\n"
                                + "if no existing card number exists...");
                Console.WriteLine("########################################\r\n");

                for (int i = 0; i < existingUsers.Length; i++)
                {
                    if (String.IsNullOrEmpty(existingCardNumbers[i]))
                    {
                        PaperCutProxyWrapper.SetCardNumber(_serverProxy, existingUsers[i], newCardNumbers[i], targetIdField);
                        updatedCount++;
                        if (updatedCount != 0 && updatedCount % 100 == 0)
                        {
                            Console.WriteLine("Updated {0} users so far...", updatedCount);
                        }
                    }
                    else
                    {
                        skippedCount++;
                        if (skippedCount != 0 && skippedCount % 100 == 0)
                        {
                            Console.WriteLine("Skipped {0} users so far...", skippedCount);
                        }
                    }
                }
                Console.WriteLine("\r\nUpdated {0} users with new card numbers,\r\n"
                        + "skipped {1} users with existing card numbers.\r\n", updatedCount, skippedCount);
                Console.WriteLine("########################################");
            }
            else
            {
                PaperCutProxyWrapper.SetCardNumbers(_serverProxy, existingUsers, newCardNumbers, targetIdField);
                Console.WriteLine("\nUpdated {0} users with new card numbers.", existingUsers.Length);
                Console.WriteLine("\n########################################\r\n");
            }
        }   // end UpdateAllCardNumbers

        /// <summary>
        /// Sends an email notification 
        /// with the card number to a single user.
        /// </summary>
        public void SendSingleEmail(int targetIdField, string targetUsername, string smtpHostname, string senderAddress)
        {
            string resolvedIdField = PaperCutProxyWrapper.ResolveCardField(targetIdField);
            string targetEmail = _serverProxy.GetUserProperty(targetUsername, "email");
            string retrievedId = _serverProxy.GetUserProperty(targetUsername, resolvedIdField);

            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = smtpHostname;

                MailMessage mail = new MailMessage(senderAddress, targetEmail);
                mail.Subject = "PaperCut ID";
                mail.Body = String.Format("Your PaperCut ID is {0}. Please keep this confidential.", retrievedId);
                client.Send(mail);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to send email.");
            }
        }   // end SendSingleEmail

        /// <summary>
        /// Sends an email notification 
        /// with the users card number to all users.
        /// </summary>
        public void SendMultipleEmails(int targetIdField, string smtpHostname, string senderAddress)
        {
            string resolvedIdField = PaperCutProxyWrapper.ResolveCardField(targetIdField);
            string[] usernames = PaperCutProxyWrapper.GetUserAccounts(_serverProxy);
            string[] retrievedProperties;
            string[] emails = new string[usernames.Length];
            string[] idNumbers = new string[usernames.Length];

            for (int i = 0; i < usernames.Length; i++)
            {
                retrievedProperties = _serverProxy.GetUserProperties(usernames[i], new string[] {"email", resolvedIdField});
                emails[i] = retrievedProperties[0];
                idNumbers[i] = retrievedProperties[1];
            }

            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = smtpHostname;

                for (int i = 0; i < usernames.Length; i++)
                {
                    MailMessage mail = new MailMessage(senderAddress, emails[i]);
                    mail.Subject = "PaperCut ID";
                    mail.Body = String.Format("Your PaperCut ID is {0}. Please keep this confidential.", idNumbers[i]);
                    client.Send(mail);
                }
            }
            catch
            {
                Console.WriteLine("Unable to send email.");
            }
        }   // end SendMultipleEmails
    }   // end class PaperCutHelper
}
