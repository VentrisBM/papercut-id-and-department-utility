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
using System.DirectoryServices;
using PaperCutUtility.Models;

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
            bool inputIsValid = true;
            ServerCommandProxy serverProxy;

            txtblockOutput.Inlines.Clear();

            ppcHostname = txtboxPpcServer.Text;
            if (String.IsNullOrEmpty(ppcHostname))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut server hostname.\r\n");
                inputIsValid = false;
            }

            ppcAdminPw = pwdboxPpcAdminPw.Password;
            if (String.IsNullOrEmpty(ppcAdminPw))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut admin password.\r\n");
                inputIsValid = false;
            }

            targetIdField = cmboxTargetIDField.SelectedIndex;
            updateOnlyIfBlank = chckboxUpdateIfBlank.IsChecked.Value;
            mustTargetSpecificUsername = chckboxTargetSpecificUser.IsChecked.Value;
            specificUsername = txtboxTargetSpecificUser.Text;

            try
            {
                numberOfChars = Int16.Parse(txtboxIDLength.Text.ToString());
                if (numberOfChars < 4 || numberOfChars > 10)
                {
                    txtblockOutput.Inlines.Add("You must enter a length between 4 and 10.\r\n");
                    inputIsValid = false;
                }
                else
                {
                    inputIsValid = true;
                }
            }
            catch (Exception)
            {
                txtblockOutput.Inlines.Add("You must enter a length between 4 and 10.\r\n");
                return;
            }

            if (inputIsValid)
            {
                try
                {
                    serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);
                    if (PaperCutProxyWrapper.ConnectionEstablished(serverProxy))
                    {
                        PaperCutHelper ppcUtility = new PaperCutHelper(serverProxy);
                        txtblockOutput.Inlines.Add("Connection to PaperCut established.\r\n");

                        int totalUsers = serverProxy.GetTotalUsers();
                        Console.WriteLine(String.Format("Retrieved {0} users from PaperCut.", totalUsers));
                        Console.WriteLine("########################################\r\n");

                        if (mustTargetSpecificUsername)
                        {
                            ppcUtility.UpdateSingleCardNumber(updateOnlyIfBlank, targetIdField, specificUsername, numberOfChars);
                        }
                        else
                        {
                            ppcUtility.UpdateAllCardNumbers(updateOnlyIfBlank, targetIdField, numberOfChars);
                        }
                    }
                }
                catch (Exception)
                {
                    txtblockOutput.Inlines.Add("Connection to PaperCut cannot be established.\r\n");
                }
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
            bool inputIsValid = true;
            ServerCommandProxy serverProxy;

            txtblockOutput.Inlines.Clear();

            ppcHostname = txtboxPpcServer.Text;
            if (String.IsNullOrEmpty(ppcHostname))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut server hostname.\r\n");
                inputIsValid = false;
            }

            ppcAdminPw = pwdboxPpcAdminPw.Password;
            if (String.IsNullOrEmpty(ppcAdminPw))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut admin password.\r\n");
                inputIsValid = false;
            }

            smtpHostname = txtboxSmtpServer.Text;
            if (String.IsNullOrEmpty(smtpHostname))
            {
                txtblockOutput.Inlines.Add("You must enter the SMTP server hostname.\r\n");
                inputIsValid = false;
            }

            senderAddress = txtboxSenderAddress.Text;
            if (!IsValidEmail(senderAddress))
            {
                txtblockOutput.Inlines.Add("You must enter a valid sender address.\r\n");
                inputIsValid = false;
                return;
            }

            targetIdField = cmboxTargetIDField.SelectedIndex;
            mustTargetSpecificUsername = chckboxTargetSpecificUser.IsChecked.Value;
            specificUsername = txtboxTargetSpecificUser.Text;

            if (inputIsValid)
            {
                try
                {
                    serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);
                    if (PaperCutProxyWrapper.ConnectionEstablished(serverProxy))
                    {
                        PaperCutHelper ppcUtility = new PaperCutHelper(serverProxy);
                        txtblockOutput.Inlines.Add("Connection to PaperCut established.\r\n");

                        int totalUsers = serverProxy.GetTotalUsers();
                        Console.WriteLine(String.Format("Retrieved {0} users from PaperCut.", totalUsers));
                        Console.WriteLine("########################################\r\n");

                        if (mustTargetSpecificUsername)
                        {
                            ppcUtility.SendSingleEmail(targetIdField, specificUsername, smtpHostname, senderAddress);
                            Console.WriteLine(String.Format("Notification email sent to {0}.", specificUsername));
                            Console.WriteLine("\n########################################\r\n");
                        }
                        else
                        {
                            ppcUtility.SendMultipleEmails(targetIdField, smtpHostname, senderAddress);
                            Console.WriteLine(String.Format("Notification emails sent to {0} users.", totalUsers));
                            Console.WriteLine("\n########################################\r\n");
                        }
                    }
                }
                catch (Exception)
                {
                    txtblockOutput.Inlines.Add("Connection to PaperCut cannot be established,\r\n"
                                                + "or cannot send email.");
                }
            }
        }   // btnSendEmail_Click

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
                && Regex.IsMatch(email, @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");
        }

        private void btnSyncDept_Click(object sender, RoutedEventArgs e)
        {
            string ppcHostname;
            string ppcAdminPw;
            string domainName;
            string ldapRoot;
            string domainUser;
            string domainUserPwd;
            int targetDeptField;
            string deptNumberADField;
            string deptNameADField;
            bool inputIsValid = true;
            ServerCommandProxy serverProxy;

            txtblockOutput.Inlines.Clear();

            ppcHostname = txtboxPpcServer.Text;
            if (String.IsNullOrEmpty(ppcHostname))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut server hostname.\r\n");
                inputIsValid = false;
            }

            ppcAdminPw = pwdboxPpcAdminPw.Password;
            if (String.IsNullOrEmpty(ppcAdminPw))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut admin password.\r\n");
                inputIsValid = false;
            }

            domainName = txtboxDomainName.Text;
            if (String.IsNullOrEmpty(domainName))
            {
                txtblockOutput.Inlines.Add("You must enter the domain name.\r\n");
                inputIsValid = false;
            }

            ldapRoot = txtboxLdapRoot.Text;
            if (String.IsNullOrEmpty(ldapRoot))
            {
                txtblockOutput.Inlines.Add("You must enter the LDAP root path.\r\n");
                inputIsValid = false;
            }

            domainUser = txtboxDomainUser.Text;
            if (String.IsNullOrEmpty(domainUser))
            {
                txtblockOutput.Inlines.Add("You must enter the domain username.\r\n");
                inputIsValid = false;
            }

            domainUserPwd = pwdboxDomainUserPwd.Password;
            if (String.IsNullOrEmpty(domainUserPwd))
            {
                txtblockOutput.Inlines.Add("You must enter the domain user password.\r\n");
                inputIsValid = false;
            }

            deptNameADField = txtboxDeptNameADField.Text;
            deptNumberADField = txtboxDeptNumberADField.Text;
            if (String.IsNullOrEmpty(deptNameADField) && String.IsNullOrEmpty(deptNumberADField))
            {
                txtblockOutput.Inlines.Add("At least one department AD field is required.\r\n");
                inputIsValid = false;
            }

            targetDeptField = cmboxTargetDeptField.SelectedIndex;

            if (inputIsValid)
            {
                try
                {
                    serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);
                    bool ppcConnectionEstablished = PaperCutProxyWrapper.ConnectionEstablished(serverProxy);

                    LdapHelper ldapHelper = new LdapHelper(domainName, ldapRoot, domainUser, domainUserPwd);
                    bool ldapConnectionEstablished = ldapHelper.ldapConnectionEstablished(domainUser);

                    if (ppcConnectionEstablished)
                    {
                        txtblockOutput.Inlines.Add("Connection to PaperCut established.\r\n");

                        if (ldapConnectionEstablished)
                        {
                            txtblockOutput.Inlines.Add("Connection to LDAP established.\r\n");
                        }
                        else
                        {
                            txtblockOutput.Inlines.Add("Connection to LDAP cannot be established.\r\n");
                            return;
                        }

                        PpcUser[] ppcUsers = PaperCutProxyWrapper.GetUsersDepartmentInfo(serverProxy);
                        if (String.IsNullOrEmpty(deptNumberADField) || String.IsNullOrEmpty(deptNameADField))
                        {
                            if (String.IsNullOrEmpty(deptNumberADField))
                            {
                                LdapUser[] ldapUsers = ldapHelper.retrieveUserDepartment(ppcUsers, deptNameADField);
                                PaperCutProxyWrapper.SetUsersSingleDepartmentInfo(serverProxy, ldapUsers, targetDeptField);
                            }
                            else
                            {
                                LdapUser[] ldapUsers = ldapHelper.retrieveUserDepartment(ppcUsers, deptNumberADField);
                                PaperCutProxyWrapper.SetUsersSingleDepartmentInfo(serverProxy, ldapUsers, targetDeptField);
                            }
                        }
                        else
                        {
                            LdapUser[] ldapUsers = ldapHelper.retrieveUserDepartments(ppcUsers, deptNumberADField, deptNameADField);
                            PaperCutProxyWrapper.SetUsersMultipleDepartmentInfo(serverProxy, ldapUsers, targetDeptField);
                        }

                        

                    }   // end if
                }
                catch (Exception)
                {
                    txtblockOutput.Inlines.Add("Connection to PaperCut cannot be established.\r\n");
                    return;
                }
            }
        }   // end btnSyncDept_Click

        private void btnClearDept_Click(object sender, RoutedEventArgs e)
        {
            string ppcHostname;
            string ppcAdminPw;
            int targetDeptField;
            bool inputIsValid = true;
            ServerCommandProxy serverProxy;

            txtblockOutput.Inlines.Clear();

            ppcHostname = txtboxPpcServer.Text;
            if (String.IsNullOrEmpty(ppcHostname))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut server hostname.\r\n");
                inputIsValid = false;
            }

            ppcAdminPw = pwdboxPpcAdminPw.Password;
            if (String.IsNullOrEmpty(ppcAdminPw))
            {
                txtblockOutput.Inlines.Add("You must enter the PaperCut admin password.\r\n");
                inputIsValid = false;
            }

            targetDeptField = cmboxTargetDeptField.SelectedIndex;

            if (inputIsValid)
            {
                try
                {
                    serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);
                    bool ppcConnectionEstablished = PaperCutProxyWrapper.ConnectionEstablished(serverProxy);

                    if(ppcConnectionEstablished)
                    {
                        PaperCutProxyWrapper.ClearUsersDepartmentInfo(serverProxy, targetDeptField);
                    }
                }
                catch (Exception)
                {
                    txtblockOutput.Inlines.Add("Connection to PaperCut cannot be established.\r\n");
                    return;
                }
            }
        }   // end btnClearDept_Click

    }   // end MainWindow
}   // end PaperCutUtility
