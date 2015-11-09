using System;

namespace PaperCutUtility.Models
{
    class PpcUser
    {
        public string Username { get; set; }
        public string Department { get; set; }
        public string Office { get; set; }

        public PpcUser()
        {
            this.Username = "";
            this.Department = "";
            this.Office = "";
        }

        public override string ToString()
        {
            return String.Format("Username: {0}, Department: {1}, Office: {2}",
                        this.Username, this.Department, this.Office);
        }
    }   // end class PpcUser
}
