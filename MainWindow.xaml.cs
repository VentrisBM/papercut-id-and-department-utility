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
using PaperCutUtility;

namespace PaperCutUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string ppcHostname;
        string ppcAdminPw;
        int targetIdField;
        bool updateOnlyIfBlank;
        int numberOfChars;
        bool mustTargetSpecificUsername;
        string specificUsername;

        ServerCommandProxy serverProxy;
        bool inputIsValid;
        string[] existingUsers;
        string[] newCardNumbers;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            txtblockOutput.Inlines.Clear();
            try
            {
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

                targetIdField = cmboxFieldToUpdate.SelectedIndex;

                try
                {
                    numberOfChars = Int16.Parse(txtboxNumberOfChars.Text.ToString());
                    if (numberOfChars < 4 || numberOfChars > 10)
                        throw new Exception();
                }
                catch (Exception ex)
                {
                    txtblockOutput.Inlines.Add("You must enter a number between 4 and 10.\r\n");
                }

                updateOnlyIfBlank = chckboxUpdateIfBlank.IsChecked.Value;
                mustTargetSpecificUsername = chckboxTargetSpecificUser.IsChecked.Value;
                specificUsername = txtboxTargetSpecificUser.Text;

                inputIsValid = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                inputIsValid = false;
            }

            try
            {
                if (!inputIsValid)
                    throw new Exception();

                serverProxy = new ServerCommandProxy(ppcHostname, 9191, ppcAdminPw);
                if (PaperCutProxyWrapper.ConnectionEstablished(serverProxy))
                {
                    txtblockOutput.Inlines.Add("Connection to PaperCut established.\r\n");
                    
                    existingUsers = PaperCutProxyWrapper.GetUserAccounts(serverProxy);
                    txtblockOutput.Inlines.Add(String.Format("Retrieved {0} users from PaperCut.\r\n", existingUsers.Length));

                    if (mustTargetSpecificUsername)
                    {
                        string newCardNumber = SecurityString.GenerateIdentifier(numberOfChars);
                        if (updateOnlyIfBlank)
                        {
                            string existingCardNumber = PaperCutProxyWrapper.GetCardNumber(serverProxy, specificUsername, targetIdField);
                            if (String.IsNullOrEmpty(existingCardNumber))
                            {
                                PaperCutProxyWrapper.SetCardNumber(serverProxy, specificUsername, newCardNumber, targetIdField);
                                txtblockOutput.Inlines.Add(String.Format("Updated user: {0},\r\n"
                                                            + "new cardNumber: {1}.\r\n", specificUsername, newCardNumber));
                            }
                            else
                            {
                                txtblockOutput.Inlines.Add(String.Format("Cannot update user: {0},\r\n"
                                                            + "existing cardNumber: {1}\r\n", specificUsername, existingCardNumber));
                            }
                        }
                        else
                        {
                            PaperCutProxyWrapper.SetCardNumber(serverProxy, specificUsername, newCardNumber, targetIdField);
                            txtblockOutput.Inlines.Add(String.Format("Updated user: {0},\r\n"
                                                            + "new cardNumber: {1}.\r\n", specificUsername, newCardNumber));
                        }
                    }
                    else
                    {
                        newCardNumbers = new string[existingUsers.Length];
                        newCardNumbers = SecurityString.GenerateIdentifiers(numberOfChars, newCardNumbers.Length);

                        if (updateOnlyIfBlank)
                        {
                            int userCount = 0;
                            int skippedCount = 0;
                            string[] existingCardNumbers;
                            existingCardNumbers = PaperCutProxyWrapper.GetCardNumbers(serverProxy, existingUsers, targetIdField);

                            for (int i = 0; i < existingUsers.Length; i++)
                            {
                                if (String.IsNullOrEmpty(existingCardNumbers[i]))
                                {
                                    PaperCutProxyWrapper.SetCardNumber(serverProxy, existingUsers[i], newCardNumbers[i], targetIdField);
                                    //Console.WriteLine("#{0}/{1} updated username: {2}, existing cardNumber: {3}", i, existingUsers.Length - 1, existingUsers[i], existingCardNumbers[i]);
                                    userCount++;
                                }
                                else
                                {
                                    skippedCount++;
                                    //Console.WriteLine("#{0}/{1} skipped username: {2}, existing cardNumber: {3}", i, existingUsers.Length - 1, existingUsers[i], existingCardNumbers[i]);
                                }
                            }
                            txtblockOutput.Inlines.Add(String.Format("Updated {0} users with new card numbers,\r\n"
                                                        + "skipped {1} users with existing card numbers.\r\n", userCount, skippedCount));
                        }
                        else
                        {
                            PaperCutProxyWrapper.SetCardNumbers(serverProxy, existingUsers, newCardNumbers, targetIdField);
                            txtblockOutput.Inlines.Add(String.Format("Updated {0} users with new card numbers.\r\n", existingUsers.Length));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                txtblockOutput.Inlines.Add("Connection to PaperCut cannot be established.\r\n");
            }

        }   // end btnUpdate_Click

    }   // end MainWindow
}   // end PaperCutUtility
