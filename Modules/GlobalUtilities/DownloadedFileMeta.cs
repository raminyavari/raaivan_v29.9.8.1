using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaaiVan.Modules.GlobalUtilities
{
    public class DownloadedFileMeta
    {
        public string IP;
        public string UserName;
        public string FirstName;
        public string LastName;
        public string ConfidentialityLevel;

        public DownloadedFileMeta(string ip, string username, string firstName, string lastName, string confidentialityLevel) {
            this.IP = ip;
            this.UserName = username;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.ConfidentialityLevel = confidentialityLevel;
        }

        private string FullName {
            get {
                return ((string.IsNullOrEmpty(FirstName) ? string.Empty : FirstName) + " " +
                   (string.IsNullOrEmpty(LastName) ? string.Empty : LastName)).Trim();
            }
        }

        public void write2table(DataTable table, int index)
        {
            if (table.Rows.Count < 20) for (int i = 0; i < 20; ++i) table.Rows.Add(table.NewRow());

            int rowIndex = 2;
            
            table.Rows[rowIndex++][index] = "'طبقه بندی گزارش'";
            table.Rows[rowIndex++][index] = string.IsNullOrEmpty(ConfidentialityLevel) ? "فاقد طبقه بندی" : ConfidentialityLevel;

            rowIndex++;

            table.Rows[rowIndex++][index] = "'نام کاربری'";
            table.Rows[rowIndex++][index] = UserName;

            rowIndex++;

            table.Rows[rowIndex++][index] = "'نام'";
            table.Rows[rowIndex++][index] = FirstName + " " + LastName;

            rowIndex++;

            table.Rows[rowIndex++][index] = "'آدرس'";
            table.Rows[rowIndex++][index] = IP;

            rowIndex++;

            DateTime now = DateTime.Now;

            table.Rows[rowIndex++][index] = "'تاریخ'";
            table.Rows[rowIndex++][index] = GenericDate.get_local_date(now);

            rowIndex++;

            table.Rows[rowIndex++][index] = "'ساعت'";
            table.Rows[rowIndex++][index] = (now.Hour < 10 ? "0" : "") + now.Hour + ":" + now.Minute;
        }

        public string toString()
        {
            DateTime now = DateTime.Now;
            
            string strTime = GenericDate.get_local_date(now, detail: false, reverse: true) + " " +
                (now.Hour < 10 ? "0" : "") + now.Hour + ":" + now.Minute;

            List<string> lst = new List<string>();

            if (!string.IsNullOrEmpty(UserName)) lst.Add("User: " + UserName);
            if (!string.IsNullOrEmpty(FullName)) lst.Add("FullName: " + FullName);
            if (!string.IsNullOrEmpty(IP)) lst.Add("Address: " + IP);
            lst.Add("Time: " + strTime);

            return string.Join(",  ", lst);
        }
    }
}
