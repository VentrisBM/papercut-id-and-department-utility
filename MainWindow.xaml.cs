using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using PaperCutUtility;

namespace PaperCutUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string ppcHostname;
            string ppcAdminPw;
            int targetIdField;
            bool updateOnlyIfBlank;
            int numberOfChars;
            bool mustTargetSpecificUsername;
            string specificUsername;
            bool inputIsValid = false;
            ServerCommandProxy serverProxy;

            txtblockOutput.Inlines.Clear();

            ppcHostname = txtboxPpcServer.Text;
            if (String.IsNullOrEmpty(ppcHostname))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut server hostname.\r\n");
            }

            ppcAdminPw = pwdboxPpcAdminPw.Password;
            if (String.IsNullOrEmpty(ppcAdminPw))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut admin password.\r\n");
            }

            targetIdField = cmboxTargetIDField.SelectedIndex;
            updateOnlyIfBlank = chckboxUpdateIfBlank.IsChecked.Value;
            mustTargetSpecificUsername = chckboxTargetSpecificUser.IsChecked.Value;
            specificUsername = txtboxTargetSpecificUser.Text;

            do
            {
                try
                {
                    numberOfChars = Int16.Parse(txtboxIDLength.Text.ToString());
                    if (numberOfChars < 4 || numberOfChars > 10)
                    {
                        txtblockOutput.Inlines.Add("You must enter a length between 4 and 10.\r\n");
                        return;
                    }
                    else
                    {
                        inputIsValid = true;
                        continue;
                    }
                }
                catch (Exception)
                {
                    txtblockOutput.Inlines.Add("You must enter a length between 4 and 10.\r\n");
                    return;
                }
            } while (!inputIsValid);

            try
            {
                serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);
                if (PaperCutProxyWrapper.ConnectionEstablished(serverProxy))
                {
                    PaperCutUtilityHelper ppcUtility = new PaperCutUtilityHelper(serverProxy);
                    txtblockOutput.Inlines.Add("Connection to PaperCut established.\r\n");

                    int totalUsers = serverProxy.GetTotalUsers();
                    txtblockOutput.Inlines.Add(String.Format("Retrieved {0} users from PaperCut.\r\n", totalUsers));

                    if (mustTargetSpecificUsername)
                    {
                        ppcUtility.UpdateSingleCardNumber(updateOnlyIfBlank, targetIdField, specificUsername, numberOfChars);
                        txtblockOutput.Inlines.Add(String.Format("Card number updated for user: {0}.\r\n", specificUsername));
                    }
                    else
                    {
                        ppcUtility.UpdateAllCardNumbers(updateOnlyIfBlank, targetIdField, numberOfChars);
                        txtblockOutput.Inlines.Add(String.Format("Card numbers updated for {0} users.\r\n", totalUsers));
                    }
                }
            }
            catch (Exception)
            {
                txtblockOutput.Inlines.Add("Connection to PaperCut cannot be established.\r\n");
            }
        }   // end btnUpdate_Click

        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            string ppcHostname;
            string ppcAdminPw;
            int targetIdField;
            bool mustTargetSpecificUsername;
            string specificUsername;
            string smtpHostname;
            string senderAddress;
            ServerCommandProxy serverProxy;

            txtblockOutput.Inlines.Clear();

            ppcHostname = txtboxPpcServer.Text;
            if (String.IsNullOrEmpty(ppcHostname))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut server hostname.\r\n");
            }

            ppcAdminPw = pwdboxPpcAdminPw.Password;
            if (String.IsNullOrEmpty(ppcAdminPw))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut admin password.\r\n");
            }

            smtpHostname = txtboxSmtpServer.Text;
            if (String.IsNullOrEmpty(smtpHostname))
            {
                txtblockOutput.Inlines.Add("You must enter the SMTP server hostname.\r\n");
            }

            senderAddress = txtboxSenderAddress.Text;
            if (!IsValidEmail(senderAddress))
            {
                txtblockOutput.Inlines.Add("You must enter a valid sender address.\r\n");
                return;
            }

            targetIdField = cmboxTargetIDField.SelectedIndex;
            mustTargetSpecificUsername = chckboxTargetSpecificUser.IsChecked.Value;
            specificUsername = txtboxTargetSpecificUser.Text;

            try
            {
                serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);
                if (PaperCutProxyWrapper.ConnectionEstablished(serverProxy))
                {
                    PaperCutUtilityHelper ppcUtility = new PaperCutUtilityHelper(serverProxy);
                    txtblockOutput.Inlines.Add("Connection to PaperCut established.\r\n");

                    int totalUsers = serverProxy.GetTotalUsers();
                    txtblockOutput.Inlines.Add(String.Format("Retrieved {0} users from PaperCut.\r\n", totalUsers));

                    if (mustTargetSpecificUsername)
                    {
                        ppcUtility.SendSingleEmail(targetIdField, specificUsername, smtpHostname, senderAddress);
                        txtblockOutput.Inlines.Add(String.Format("Notification email sent to {0}.", specificUsername));
                    }
                    else
                    {
                        ppcUtility.SendMultipleEmails(targetIdField, smtpHostname, senderAddress);
                        txtblockOutput.Inlines.Add(String.Format("Notification emails sent to {0} users.", totalUsers));
                    }
                }
            }
            catch (Exception ex)
            {
                txtblockOutput.Inlines.Add("Connection to PaperCut cannot be established,\r\n"
                                            + "or cannot send email.");
            }
        }   // btnSendEmail_Click

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                && Regex.IsMatch(email, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
        }

    }   // end MainWindow
}   // end PaperCutUtility
