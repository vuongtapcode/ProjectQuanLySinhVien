using System;
using System.Data;

namespace ProjectQuanLySinhVien.DTO
{
    public class Account
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }

        // LƯU QUYỀN (0 HOẶC 1)
        public int Type { get; set; }

        public Account(string userName, string displayName, int type, string password = null)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.Type = type;
            this.Password = password;
        }

        public Account(DataRow row)
        {
            this.UserName = row["UserName"].ToString();
            this.DisplayName = row["DisplayName"].ToString();
            this.Password = row["PassWord"].ToString();

            // LẤY CỘT TYPE TỪ DATABASE GÁN VÀO
            if (row["Type"] != DBNull.Value)
            {
                this.Type = (int)row["Type"];
            }
        }
    }
}