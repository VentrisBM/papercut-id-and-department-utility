using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperCutUtility.Models
{
    class LdapUser
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentNumber { get; set; }

        public LdapUser()
        {
            this.Username = "";
            this.FullName = "";
            this.DepartmentName = "";
            this.DepartmentNumber = "";
        }

        public override string ToString()
        {
            return String.Format("Username: {0}, CN: {1}, DepartmentName: {2}, DepartmentNumber: {3}",
                            this.Username, this.FullName, this.DepartmentName, this.DepartmentNumber);
        }
    }
}
