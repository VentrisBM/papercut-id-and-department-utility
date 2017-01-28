using System;
using System.DirectoryServices;
using System.Collections.Generic;

using PaperCutUtility.Models;

namespace PaperCutUtility.Helpers
{
    internal class LdapHelper
    {
        #region Members
        private DirectoryEntry _ldapConnection;
        #endregion

        public LdapHelper(string domainName, string ldapRoot, string domainUser, string domainUserPwd)
        {
            _ldapConnection = new DirectoryEntry(string.Format(Common.Constants.Ldap.Search.Path, domainName),
                                                domainUser, domainUserPwd);
            _ldapConnection.Path = string.Format(Common.Constants.Ldap.Search.Path, ldapRoot);
            _ldapConnection.AuthenticationType = AuthenticationTypes.Secure;
        }

        #region Public methods
        /// <summary>
        /// Returns a boolean indicating whether or not the LDAP connection has been established.
        /// </summary>
        /// <param name="ldapUsername"></param>
        /// <returns></returns>
        public bool IsConnectionEstablished(string ldapUsername)
        {
            bool isConnectionEstablished = false;

            try
            {
                if (!string.IsNullOrEmpty(ldapUsername))
                {
                    DirectorySearcher ldapSearch = new DirectorySearcher(_ldapConnection);
                    
                    ldapSearch.PropertiesToLoad.Add(Common.Constants.Ldap.Search.CN);
                    ldapSearch.Filter = string.Format(Common.Constants.Ldap.Search.AccountName, ldapUsername);

                    SearchResult result = ldapSearch.FindOne();

                    isConnectionEstablished = result != null;
                }
            }
            catch (Exception)
            {
                isConnectionEstablished = false;
            }

            return isConnectionEstablished;
        }

        /// <summary>
        /// Retrieves an array of LdapUsers containing the usernames, full names,
        /// department names and department numbers.
        /// </summary>
        /// <param name="ppcUsers"></param>
        /// <param name="departmentName"></param>
        /// <param name="departmentNumber"></param>
        /// <returns></returns>
        public LdapUser[] RetrieveDepartmentInformation(PaperCutUser[] ppcUsers, string departmentName, string departmentNumber = null)
        {
            List<LdapUser> ldapUsers = null;

            try
            {
                if (ppcUsers.Length > 0 && !string.IsNullOrEmpty(departmentName))
                {
                    DirectorySearcher ldapSearch = new DirectorySearcher(_ldapConnection);
                    List<string> requiredProperties = new List<string>();
                    requiredProperties.Add(Common.Constants.Ldap.Search.CN);
                    requiredProperties.Add(departmentName);

                    bool isDepartmentNumberRequired = !string.IsNullOrEmpty(departmentNumber);

                    if (isDepartmentNumberRequired)
                    {
                        requiredProperties.Add(departmentNumber);
                    }

                    foreach (string property in requiredProperties)
                    {
                        ldapSearch.PropertiesToLoad.Add(property);
                    }

                    for (int i = 0; i < ppcUsers.Length; i++)
                    {
                        LdapUser ldapUser = new LdapUser();
                        ldapUser.Username = ppcUsers[i].Username;

                        ldapSearch.Filter = string.Format(Common.Constants.Ldap.Search.AccountName, ldapUser.Username);
                        SearchResult result = ldapSearch.FindOne();

                        if (result != null)
                        {
                            if (ResultContainsRequiredProperties(result, requiredProperties, isDepartmentNumberRequired))
                            {
                                FillLdapUserPropertiesFromSearchResult(ldapUser, result, requiredProperties, isDepartmentNumberRequired);

                                ldapUsers.Add(ldapUser);
                            }
                            else
                            {
                                Console.WriteLine(string.Format(Common.Constants.Ldap.Messages.IncompleteAdInformationForUser,
                                                    ldapUsers[i].Username));
                            }
                        }
                        else
                        {
                            Console.WriteLine(Common.Constants.Ldap.Messages.NothingRetrieved);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return ldapUsers != null ? ldapUsers.ToArray() : null;
        }
        #endregion

        #region Private methods
        private bool ResultContainsRequiredProperties(SearchResult result, List<string> requiredProperties, bool isDepartmentNumberRequired)
        {
            bool isResultComplete = false;

            if (result != null && requiredProperties.Count > 0)
            {
                isResultComplete = !string.IsNullOrEmpty(result.Properties[requiredProperties[0]][0].ToString()) &&
                                 !string.IsNullOrEmpty(result.Properties[requiredProperties[1]][0].ToString());

                if (isDepartmentNumberRequired)
                {
                    isResultComplete = isResultComplete && !string.IsNullOrEmpty(result.Properties[requiredProperties[2]][0].ToString());
                }
            }

            return isResultComplete;
        }

        private void FillLdapUserPropertiesFromSearchResult(LdapUser ldapUser, SearchResult result, List<string> requiredProperties, bool isDepartmentNumberRequired)
        {
            if (ldapUser != null && result != null && requiredProperties.Count > 0)
            {
                ldapUser.FullName = result.Properties[requiredProperties[0]][0].ToString();
                ldapUser.DepartmentName = result.Properties[requiredProperties[1]][0].ToString();

                if (isDepartmentNumberRequired)
                {
                    ldapUser.DepartmentNumber = result.Properties[requiredProperties[2]][0].ToString();
                }
            }
        }
        #endregion
    }
}
