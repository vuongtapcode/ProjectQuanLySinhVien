using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ProjectQuanLySinhVien.DTO; 

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fThongTinTaiKhoan : Form
    {
        private Account loginAccount; 
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";

        public fThongTinTaiKhoan()
        {
            InitializeComponent();
        }

        public fThongTinTaiKhoan(Account acc)
        {
            InitializeComponent();
            this.loginAccount = acc; 
            LoadAccountInfo();
        }

        void LoadAccountInfo()
        {
            txbTenDangNhap.Text = loginAccount.UserName;
            txbTenHienThi.Text = loginAccount.DisplayName;
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            string tenHienThi = txbTenHienThi.Text;
            string matKhauCu = txbMatKhauCu.Text; 
            string matKhauMoi = txbMatKhauMoi.Text;
            string nhapLai = txbNhapLai.Text;

            if (string.IsNullOrEmpty(matKhauCu))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu hiện tại để xác nhận!");
                return;
            }
            if (!matKhauMoi.Equals(nhapLai))
            {
                MessageBox.Show("Mật khẩu mới và nhập lại không khớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(strKetNoi))
                {
                    connection.Open();
                    string sqlCheck = "SELECT COUNT(*) FROM ACCOUNT WHERE UserName=@user AND PassWord=@pass";
                    SqlCommand cmdCheck = new SqlCommand(sqlCheck, connection);
                    cmdCheck.Parameters.AddWithValue("@user", loginAccount.UserName);
                    cmdCheck.Parameters.AddWithValue("@pass", matKhauCu);

                    int kq = (int)cmdCheck.ExecuteScalar();
                    if (kq == 0)
                    {
                        MessageBox.Show("Mật khẩu cũ không đúng!");
                        return;
                    }
                    SqlCommand cmdUpdate = new SqlCommand();
                    cmdUpdate.Connection = connection;

                    string sqlUpdate = "";
                    if (!string.IsNullOrEmpty(matKhauMoi))
                    {
   

                        sqlUpdate = "UPDATE ACCOUNT SET DisplayName=@name, PassWord=@newPass WHERE UserName=@user";
                        cmdUpdate.Parameters.AddWithValue("@newPass", matKhauMoi);
                    }
                    else
                    {
                        sqlUpdate = "UPDATE ACCOUNT SET DisplayName=@name WHERE UserName=@user";
                    }

                    cmdUpdate.CommandText = sqlUpdate;
                    cmdUpdate.Parameters.AddWithValue("@name", tenHienThi);
                    cmdUpdate.Parameters.AddWithValue("@user", loginAccount.UserName); 

                    int rowsAffected = cmdUpdate.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cập nhật thành công! Vui lòng đăng nhập lại để áp dụng thay đổi.");
                        loginAccount.DisplayName = tenHienThi;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Cập nhật thất bại!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}