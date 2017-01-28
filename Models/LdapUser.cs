using System;

namespace PaperCutUtility.Models
{
    public class LdapUser
    {
        #region Properties
        public string Username { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentNumber { get; set; }
        #endregion

        public override string ToString()
        {
            return string.Format("Username: {0}, CN: {1}, DepartmentName: {2}, DepartmentNumber: {3}", Username, FullName, DepartmentName, DepartmentNumber);
        }
    }
}
