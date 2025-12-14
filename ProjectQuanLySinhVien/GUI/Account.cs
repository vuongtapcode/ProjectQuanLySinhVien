using System;
using System.Data;

namespace ProjectQuanLySinhVien.DTO
{
    public class Account
    {
        // Các thuộc tính tương ứng với cột trong bảng SQL  
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }

        public Account(string userName, string displayName, string password)
        {
            this.UserName = userName;
            this.DisplayName = displayName;
            this.Password = password;
        }

        // Hàm tiện ích: Tự động trích xuất dữ liệu từ 1 dòng kết quả SQL
        public Account(DataRow row)
        {
            this.UserName = row["UserName"].ToString();
            this.DisplayName = row["DisplayName"].ToString(); 
            this.Password = row["PassWord"].ToString();
        }
    }
}