using System;
using System.Windows;
using System.Text.RegularExpressions;

using PaperCutUtility.Models;
using PaperCutUtility.Wrappers;
using PaperCutUtility.Helpers;
using PaperCutUtility.Common;

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
            ServerCommandProxy serverProxy = null;
            OperationTypeEnum operation = OperationTypeEnum.Update;

            txtblockOutput.Inlines.Clear();

            try
            {
                ValidationResult result = ValidateInput(operation);

                if (result.IsValid)
                {
                    string ppcHostname = txtboxPpcServer.Text;
                    string ppcAdminPw = pwdboxPpcAdminPw.Password;

                    serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);

                    if (PaperCutProxyWrapper.IsConnectionEstablished(serverProxy))
                    {
                        txtblockOutput.Inlines.Add(Constants.PaperCut.Messages.PaperCutConnectionEstablished);

                        int totalUsers = serverProxy.GetTotalUsers();

                        Console.WriteLine(string.Format(Constants.PaperCut.Messages.NumberOfUsersRetrievedFromPaperCut, totalUsers));
                        Console.WriteLine(Constants.ConsoleSpacing.HashesWithNewLine);

                        PaperCutHelper paperCutHelper = new PaperCutHelper(serverProxy);

                        if (paperCutHelper != null)
                        {
                            int targetIdField = cmboxTargetIDField.SelectedIndex;
                            int numberOfChars = int.Parse(txtboxIDLength.Text.ToString());
                            bool updateOnlyIfBlank = chckboxUpdateIfBlank.IsChecked.Value;

                            bool mustTargetSpecificUsername = chckboxTargetSpecificUser.IsChecked.Value;

                            if (mustTargetSpecificUsername)
                            {
                                string specificUsername = txtboxTargetSpecificUser.Text;

                                paperCutHelper.UpdateSingleCardNumber(updateOnlyIfBlank, targetIdField, specificUsername, numberOfChars);
                            }
                            else
                            {
                                paperCutHelper.UpdateAllCardNumbers(updateOnlyIfBlank, targetIdField, numberOfChars);
                            }
                        }
                    }
                    else
                    {
                        txtblockOutput.Inlines.Add(Constants.PaperCut.Messages.PaperCutConnectionNotEstablished);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnSendEmail_Click(object sender, RoutedEventArgs e)
        {
            ServerCommandProxy serverProxy = null;
            OperationTypeEnum operation = OperationTypeEnum.SendEmail;

            txtblockOutput.Inlines.Clear();

            try
            {
                ValidationResult result = ValidateInput(operation);

                if (result.IsValid)
                {
                    string ppcHostname = txtboxPpcServer.Text;
                    string ppcAdminPw = pwdboxPpcAdminPw.Password;

                    serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);

                    if (PaperCutProxyWrapper.IsConnectionEstablished(serverProxy))
                    {
                        txtblockOutput.Inlines.Add(Constants.PaperCut.Messages.PaperCutConnectionEstablished);

                        int totalUsers = serverProxy.GetTotalUsers();

                        Console.WriteLine(string.Format(Constants.PaperCut.Messages.NumberOfUsersRetrievedFromPaperCut, totalUsers));
                        Console.WriteLine(Constants.ConsoleSpacing.HashesWithNewLine);

                        PaperCutHelper paperCutHelper = new PaperCutHelper(serverProxy);

                        if (paperCutHelper != null)
                        {
                            int targetIdField = cmboxTargetIDField.SelectedIndex;
                            string smtpHostname = txtboxSmtpServer.Text;
                            string senderAddress = txtboxSenderAddress.Text;

                            bool mustTargetSpecificUsername = chckboxTargetSpecificUser.IsChecked.Value;

                            if (mustTargetSpecificUsername)
                            {
                                string specificUsername = txtboxTargetSpecificUser.Text;

                                paperCutHelper.SendSingleEmail(smtpHostname, specificUsername, senderAddress, targetIdField);

                                Console.WriteLine(string.Format(Constants.Smtp.Messages.NotificationEmailSentToSingleUser, specificUsername));
                                Console.WriteLine(Constants.ConsoleSpacing.HashesWithNewLine);
                            }
                            else
                            {
                                paperCutHelper.SendEmailsToAllUsers(smtpHostname, senderAddress, targetIdField);

                                Console.WriteLine(string.Format(Constants.Smtp.Messages.NotificationEmailsSentToMultipleUsers, totalUsers));
                                Console.WriteLine(Constants.ConsoleSpacing.HashesWithNewLine);
                            }
                        }
                    }
                    else
                    {
                        txtblockOutput.Inlines.Add(Constants.PaperCut.Messages.PaperCutConnectionNotEstablished);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(Constants.Smtp.Messages.UnableToSendEmail);
            }
        }

        private void btnSyncDept_Click(object sender, RoutedEventArgs e)
        {
            ServerCommandProxy serverProxy;
            OperationTypeEnum operation = OperationTypeEnum.SyncDepartment;

            txtblockOutput.Inlines.Clear();

            try
            {
                ValidationResult result = ValidateInput(operation);

                if (result.IsValid)
                {
                    string ppcHostname = txtboxPpcServer.Text;
                    string ppcAdminPw = pwdboxPpcAdminPw.Password;

                    serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);

                    if (PaperCutProxyWrapper.IsConnectionEstablished(serverProxy))
                    {
                        txtblockOutput.Inlines.Add(Constants.PaperCut.Messages.PaperCutConnectionEstablished);

                        string domainName = txtboxDomainName.Text;
                        string ldapRoot = txtboxLdapRoot.Text;
                        string domainUser = txtboxDomainUser.Text;
                        string domainUserPwd = pwdboxDomainUserPwd.Password;

                        LdapHelper ldapHelper = new LdapHelper(domainName, ldapRoot, domainUser, domainUserPwd);

                        if (ldapHelper.IsConnectionEstablished(domainUser))
                        {
                            txtblockOutput.Inlines.Add(Constants.Ldap.Messages.LdapConnectionEstablished);

                            PaperCutUser[] ppcUsers = PaperCutProxyWrapper.GetUsersDepartmentInfo(serverProxy);

                            int targetDeptField = cmboxTargetDeptField.SelectedIndex;
                            string departmentNameADField = txtboxDeptNameADField.Text;
                            string departmentNumberADField = txtboxDeptNumberADField.Text;

                            bool areBothAdFieldsEntered = !string.IsNullOrEmpty(departmentNameADField) &&
                                                          !string.IsNullOrEmpty(departmentNumberADField);

                            if (areBothAdFieldsEntered)
                            {
                                LdapUser[] ldapUsers = ldapHelper.RetrieveDepartmentInformation(ppcUsers, departmentNameADField, departmentNumberADField);
                                PaperCutProxyWrapper.SetUsersMultipleDepartmentInfo(serverProxy, ldapUsers, targetDeptField);
                            }
                            else
                            {
                                LdapUser[] ldapUsers = ldapHelper.RetrieveDepartmentInformation(ppcUsers, string.IsNullOrEmpty(departmentNameADField) ?
                                                                                                departmentNumberADField : departmentNameADField);
                                PaperCutProxyWrapper.SetUsersSingleDepartmentInfo(serverProxy, ldapUsers, targetDeptField);
                            }
                        }
                        else
                        {
                            txtblockOutput.Inlines.Add(Constants.Ldap.Messages.LdapConnectionNotEstablished);
                        }
                    }
                    else
                    {
                        txtblockOutput.Inlines.Add(Constants.PaperCut.Messages.PaperCutConnectionNotEstablished);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnClearDept_Click(object sender, RoutedEventArgs e)
        {
            ServerCommandProxy serverProxy;
            OperationTypeEnum operation = OperationTypeEnum.ClearDepartment;

            txtblockOutput.Inlines.Clear();

            try
            {
                ValidationResult result = ValidateInput(operation);

                if (result.IsValid)
                {
                    string ppcHostname = txtboxPpcServer.Text;
                    string ppcAdminPw = pwdboxPpcAdminPw.Password;

                    serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);

                    if (PaperCutProxyWrapper.IsConnectionEstablished(serverProxy))
                    {
                        txtblockOutput.Inlines.Add(Constants.PaperCut.Messages.PaperCutConnectionEstablished);

                        int targetDeptField = cmboxTargetDeptField.SelectedIndex;

                        PaperCutProxyWrapper.ClearUsersDepartmentInfo(serverProxy, targetDeptField);
                    }
                    else
                    {
                        txtblockOutput.Inlines.Add(Constants.PaperCut.Messages.PaperCutConnectionNotEstablished);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private ValidationResult ValidateInput(OperationTypeEnum operationType)
        {
            ValidationResult result = new ValidationResult();
            result.IsValid = true;

            if (string.IsNullOrEmpty(txtboxPpcServer.Text))
            {
                txtblockOutput.Inlines.Add(Constants.Validation.Messages.PaperCutHostnameRequired);
                result.IsValid = false;
            }

            if (string.IsNullOrEmpty(pwdboxPpcAdminPw.Password))
            {
                txtblockOutput.Inlines.Add(Constants.Validation.Messages.PaperCutAdminPasswordRequired);
                result.IsValid = false;
            }

            if (chckboxTargetSpecificUser.IsChecked.HasValue && chckboxTargetSpecificUser.IsChecked.Value &&
                        string.IsNullOrEmpty(txtboxTargetSpecificUser.Text))
            {
                txtblockOutput.Inlines.Add(Constants.Validation.Messages.SpecificUsernameIsRequired);
                result.IsValid = false;
            }

            if (operationType == OperationTypeEnum.Update)
            {
                int numberOfChars = 0;
                bool parsedSuccessfully = int.TryParse(txtboxIDLength.Text.ToString(), out numberOfChars);

                if (!parsedSuccessfully || (numberOfChars < 4 || numberOfChars > 10))
                {
                    txtblockOutput.Inlines.Add(Constants.Validation.Messages.PaperCutIdLengthIsInvalid);
                    result.IsValid = false;
                }
            }

            if (operationType == OperationTypeEnum.SendEmail)
            {
                if (string.IsNullOrEmpty(txtboxSmtpServer.Text))
                {
                    txtblockOutput.Inlines.Add(Constants.Validation.Messages.SmtpServerHostnameRequired);
                    result.IsValid = false;
                }

                if (!IsEmailValid(txtboxSenderAddress.Text))
                {
                    txtblockOutput.Inlines.Add(Constants.Validation.Messages.SenderEmailIsInvalid);
                    result.IsValid = false;
                }
            }

            if (operationType == OperationTypeEnum.SyncDepartment)
            {
                if (string.IsNullOrEmpty(txtboxDomainName.Text))
                {
                    txtblockOutput.Inlines.Add(Constants.Validation.Messages.DomainNameRequired);
                    result.IsValid = false;
                }

                if (string.IsNullOrEmpty(txtboxLdapRoot.Text))
                {
                    txtblockOutput.Inlines.Add(Constants.Validation.Messages.LdapRootPathRequired);
                    result.IsValid = false;
                }

                if (string.IsNullOrEmpty(txtboxDomainUser.Text))
                {
                    txtblockOutput.Inlines.Add(Constants.Validation.Messages.DomainUsernameRequired);
                    result.IsValid = false;
                }

                if (string.IsNullOrEmpty(pwdboxDomainUserPwd.Password))
                {
                    txtblockOutput.Inlines.Add(Constants.Validation.Messages.DomainPasswordRequired);
                    result.IsValid = false;
                }

                if (string.IsNullOrEmpty(txtboxDeptNameADField.Text) && string.IsNullOrEmpty(txtboxDeptNumberADField.Text))
                {
                    txtblockOutput.Inlines.Add(Constants.Validation.Messages.DepartmentAdFieldRequired);
                    result.IsValid = false;
                }
            }

            return result;
        }

        private bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, Constants.RegexExpressions.EmailValidation.Characters)
                && Regex.IsMatch(email, Constants.RegexExpressions.EmailValidation.SpecialCharacters);
        }

        private enum OperationTypeEnum
        {
            Update = 0,
            SendEmail,
            SyncDepartment,
            ClearDepartment
        }

    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
    }
}
