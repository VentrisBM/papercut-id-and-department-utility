using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperCutUtility.Common
{
    public static class Constants
    {
        public static class Validation
        {
            public static class Messages
            {
                public const string PaperCutHostnameRequired = "You must enter the PaperCut server hostname.\r\n";
                public const string PaperCutAdminPasswordRequired = "You must enter the PaperCut admin password.\r\n";
                public const string PaperCutIdLengthIsInvalid = "You must enter a length between 4 and 10.\r\n";

                public const string SmtpServerHostnameRequired = "You must enter the SMTP server hostname.\r\n";
                public const string SenderEmailIsInvalid = "You must enter a valid sender address.\r\n";

                public const string DomainNameRequired = "You must enter the domain name.\r\n";
                public const string LdapRootPathRequired = "You must enter the LDAP root path.\r\n";
                public const string DomainUsernameRequired = "You must enter the domain username.\r\n";
                public const string DomainPasswordRequired = "You must enter the domain user password.\r\n";

                public const string DepartmentAdFieldRequired = "At least one department AD field is required.\r\n";

                public const string SpecificUsernameIsRequired = "You must enter a specific username.\r\n";
            }
        }

        public static class PaperCut
        {
            public static class Messages
            {
                public const string PaperCutConnectionEstablished = "Connection to PaperCut server established.\r\n";
                public const string PaperCutConnectionNotEstablished = "Connection to PaperCut cannot be established.\r\n";
                public const string NumberOfUsersRetrievedFromPaperCut = "Retrieved {0} users from PaperCut.\r\n";

                public const string RetrievingExistingCardNumbers = "Retrieving existing card numbers...";
                public const string RetrievedNCardNumbersSoFar = "Retrieved {0} card numbers so far...";

                public const string RetrievingEmailAddresses = "Retrieving email addresses...";
                public const string RetrievedNEmailAddressesSoFar = "Retrieved {0} email addresses so far...";

                public const string ClearingCardNumberForUsername = "Clearing {0} for username {1}.";
                public const string ClearSuccessful = "Clear successful.";

                public const string ClearingPropertyForNUsers = "Clearing {0} for {1} users...";

                public const string ClearedNCardNumbersSoFar = "Cleared {0} card numbers so far...";
                public const string ClearedCardNumbersForNUsers = "Cleared card numbers for {0} users.";

                public const string ClearedNPropertiesSoFar = "Cleared {0} {1} properties so far...";
                public const string ClearedPropertiesForNUsers = "Cleared {0}s for {1} users.";

                public const string UpdatedCardNumberProgress = "#{0}/{1} updated username: {2}, new card number: {3}";
                public const string UpdatedDeparmentProgress = "#{0}/{1} updated username: {2}, new department: {3}";

                public const string RetrievingDepartmentInformationForNUsers = "Retrieving department information \nfor {0} PaperCut users...";

                public const string RetrievedNUsersSoFar = "Retrieved {0} users so far...";
                public const string RetrievedNUsers = "Retrieved {0} users.";

                public const string UpdatedNUsersWithNewCardDepartments = "Updated {0} users with new card departments.";

                public const string UpdatedUserWithNewCardNumber = "Updated user: {0}, new card number: {1}\r\n";

                public const string CannotUpdateUserDueToExistingCardNumber = "Cannot update user: {0} due to existing card number: {1}\r\n";

                public const string UpdatingOnlyUsersWithBlankCardNumbers = "Checking users and updating only\r\n if no existing card number exists...";

                public const string UpdatedNUsersSoFar = "Updated {0} users so far...";
                public const string SkippedNUsersSoFar = "Skipped {0} users so far...";
                public const string UpdatedAndSkippedNUsers = "Updated {0} users with new card numbers,\r\n skipped {1} users with existing card numbers.\r\n";
                public const string UpdatedNUsers = "Updated {0} users with new card numbers.";
            }

            public static class Properties
            {
                public const string Email = "email";
                public const string Department = "department";
                public const string Office = "office";
                public const string PrimaryCardNumber = "primary-card-number";
                public const string SecondaryCardNumber = "secondary-card-number";
            }

            public static class Constants
            {
                public const int MinimumNumberOfCharacters = 4;
                public const int MaximumNumberOfCharacters = 10;
            }
        }

        public static class Smtp
        {
            public static class Messages
            {
                public const string NotificationEmailSentToSingleUser = "Notification email sent to {0}.\r\n";
                public const string NotificationEmailsSentToMultipleUsers = "Notification emails sent to {0} users.\r\n";
                public const string UnableToSendEmail = "Unable to send email. Please check your SMTP settings.\r\n";
            }

            public static class Email
            {
                public const string Subject = "Papercut ID";
                public const string Body = "Your PaperCut ID is {0}. Please keep this confidential.";
                public const string UnableToSend = "Unable to send email.";
            }
        }

        public static class Ldap
        {
            public static class Messages
            {
                public const string LdapConnectionEstablished = "Connection to LDAP established.\r\n";
                public const string LdapConnectionNotEstablished = "Connection to LDAP cannot be established.\r\n";

                public const string IncompleteAdInformationForUser = "Incomplete AD information for user: {0}";
                public const string NothingRetrieved = "Nothing retrieved.";
            }

            public static class Search
            {
                public const string Path = "LDAP://{0}";
                public const string CN = "cn";
                public const string AccountName = "(sAMAccountName={0})";
            }
        }

        public static class ConsoleSpacing
        {
            public const string Hashes = "########################################";
            public const string HashesWithNewLine = "########################################\r\n";
            public const string Dash = " - ";
        }

        public static class RegexExpressions
        {
            public static class EmailValidation
            {
                public const string Characters = @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z";
                public const string SpecialCharacters = @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*";
            }
            
        }
    }
}
