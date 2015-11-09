using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using PaperCutUtility.Models;

namespace PaperCutUtility
{
    class LdapHelper
    {
        string domainName;
        string ldapRoot;
        string domainUser;
        string domainUserPwd;

        private DirectoryEntry createDirectoryEntry()
        {
            DirectoryEntry ldapConnection = new DirectoryEntry("LDAP://" + domainName, domainUser, domainUserPwd);
            ldapConnection.Path = "LDAP://" + ldapRoot;
            ldapConnection.AuthenticationType = AuthenticationTypes.Secure;
            return ldapConnection;
        }

        public LdapHelper(string domainName, string ldapRoot, string domainUser, 
                    string domainUserPwd)
        {
            this.domainName = domainName;
            this.ldapRoot = ldapRoot;
            this.domainUser = domainUser;
            this.domainUserPwd = domainUserPwd;
        }

        /// <summary>
        /// Checks whether or not the LDAP connection has been established.
        /// </summary>
        ///
        /// <returns>
        /// True or false, whether or not the LDAP connection has been established.
        /// </returns>
        public bool ldapConnectionEstablished(string ldapUsername)
        {
            DirectoryEntry ldapConnection = createDirectoryEntry();
            DirectorySearcher ldapSearch = new DirectorySearcher(ldapConnection);
            string[] requiredProperties = new string[] { "cn" };
            bool connectionEstablished = false;

            foreach (String property in requiredProperties)
            {
                ldapSearch.PropertiesToLoad.Add(property);
            }

            try
            {
                ldapSearch.Filter = "(sAMAccountName=" + ldapUsername + ")";
                SearchResult result = ldapSearch.FindOne();

                if (result != null)
                {
                    connectionEstablished = true;
                }
                else
                {
                    connectionEstablished = false;
                }
            }
            catch (Exception)
            {
                connectionEstablished = false;
            }

            return connectionEstablished;
        }

        /// <summary>
        /// Retrieves multiple department information for users from Active Directory.
        /// </summary>
        ///
        /// <returns>
        /// A list of LdapUsers containing the usernames, full names,
        /// department names and department numbers.
        /// </returns>
        public LdapUser[] retrieveUserDepartments(PpcUser[] ppcUsers, string deptNumber, string deptName)
        {
            LdapUser[] ldapUsers = new LdapUser[ppcUsers.Length];

            DirectoryEntry ldapConnection = createDirectoryEntry();
            DirectorySearcher ldapSearch = new DirectorySearcher(ldapConnection);
            string[] requiredProperties = new string[] { "cn", deptNumber, deptName };
            
            foreach (String property in requiredProperties)
            {
                ldapSearch.PropertiesToLoad.Add(property);
            }

            for (int i = 0; i < ldapUsers.Length; i++)
            {
                ldapUsers[i] = new LdapUser();
                ldapUsers[i].Username = ppcUsers[i].Username;
                try
                {
                    ldapSearch.Filter = "(sAMAccountName=" + ldapUsers[i].Username + ")";
                    SearchResult result = ldapSearch.FindOne();

                    if (result != null)
                    {
                        try
                        {
                            ldapUsers[i].FullName = result.Properties[requiredProperties[0]][0].ToString();
                            ldapUsers[i].DepartmentNumber = result.Properties[requiredProperties[1]][0].ToString();
                            ldapUsers[i].DepartmentName = result.Properties[requiredProperties[2]][0].ToString();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Incomplete AD information for user: {0}", ldapUsers[i].Username);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nothing retrieved.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return ldapUsers;
        }   // end retrieveUserDepartments

        /// <summary>
        /// Retrieves the department information for users from Active Directory.
        /// </summary>
        ///
        /// <returns>
        /// A list of LdapUsers containing the usernames, full names,
        /// department names or department numbers.
        /// </returns>
        public LdapUser[] retrieveUserDepartment(PpcUser[] ppcUsers, string department)
        {
            LdapUser[] ldapUsers = new LdapUser[ppcUsers.Length];

            DirectoryEntry ldapConnection = createDirectoryEntry();
            DirectorySearcher ldapSearch = new DirectorySearcher(ldapConnection);
            string[] requiredProperties = new string[] { "cn", department};

            foreach (String property in requiredProperties)
            {
                ldapSearch.PropertiesToLoad.Add(property);
            }

            for (int i = 0; i < ldapUsers.Length; i++)
            {
                ldapUsers[i] = new LdapUser();
                ldapUsers[i].Username = ppcUsers[i].Username;
                try
                {
                    ldapSearch.Filter = "(sAMAccountName=" + ldapUsers[i].Username + ")";
                    SearchResult result = ldapSearch.FindOne();

                    if (result != null)
                    {
                        try
                        {
                            ldapUsers[i].FullName = result.Properties[requiredProperties[0]][0].ToString();
                            ldapUsers[i].DepartmentName = result.Properties[requiredProperties[1]][0].ToString();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Incomplete AD information for user: {0}", ldapUsers[i].Username);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nothing retrieved.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return ldapUsers;
        }   // end retrieveUserDepartments

    }   // end class LdapHelper
}
