using System;

namespace PaperCutUtility.Models
{
    public class PaperCutUser
    {
        #region Properties
        public string Username { get; set; }
        public string Department { get; set; }
        public string Office { get; set; }
        #endregion

        public override string ToString()
        {
            return string.Format("Username: {0}, Department: {1}, Office: {2}", Username, Department, Office);
        }
    }
}
