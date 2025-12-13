using System;
using System.Data;
using System.Data.SqlClient; // Thư viện kết nối SQL
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fDangNhap : Form
    {
        // 1. Chuỗi kết nối (Nhớ sửa tên máy của bạn nếu cần)
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True";
        SqlConnection conn = null;

        public fDangNhap()
        {
            InitializeComponent();
        }

        // --- SỰ KIỆN 1: KHI FORM VỪA MỞ LÊN (Tự điền pass cũ nếu có) ---
        private void fDangNhap_Load(object sender, EventArgs e)
        {
            try
            {
                // Nếu trước đó đã lưu thì tự điền
                if (!string.IsNullOrEmpty(Properties.Settings.Default.SavedUsername))
                {
                    txbTaiKhoan.Text = Properties.Settings.Default.SavedUsername;
                    txbMatKhau.Text = Properties.Settings.Default.SavedPassword;
                    chkGhiNho.Checked = true;
                }
            }
            catch
            {
                // Ignore if settings missing
            }
        }

        // --- SỰ KIỆN 2: BẤM NÚT ĐĂNG NHẬP ---
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string taiKhoan = txbTaiKhoan.Text;
            string matKhau = txbMatKhau.Text;

            // 1. Kiểm tra rỗng
            if (string.IsNullOrEmpty(taiKhoan) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tài khoản và mật khẩu!");
                return;
            }

            // 2. Kiểm tra trong Database (SQL)
            if (KiemTraDangNhap(taiKhoan, matKhau))
            {
                // Lưu thông tin đăng nhập để lần sau tự điền (nếu user chọn)
                try
                {
                    if (chkGhiNho.Checked)
                    {
                        Properties.Settings.Default.SavedUsername = taiKhoan;
                        Properties.Settings.Default.SavedPassword = matKhau;
                    }
                    else
                    {
                        Properties.Settings.Default.SavedUsername = string.Empty;
                        Properties.Settings.Default.SavedPassword = string.Empty;
                    }
                    Properties.Settings.Default.Save();
                }
                catch
                {
                    // ignore save errors
                }

                // -- Mở Form Chính --
                MessageBox.Show("Đăng nhập thành công!");
                fSinhVien f = new fSinhVien(); // Mở form chính
                this.Hide();
                f.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!");
            }
        }

        // --- HÀM KIỂM TRA SQL ---
        private bool KiemTraDangNhap(string tk, string mk)
        {
            try
            {
                conn = new SqlConnection(strKetNoi);
                conn.Open();

                // Đếm xem có tài khoản nào khớp cả Tên và Pass không
                string sql = "SELECT COUNT(*) FROM ACCOUNT WHERE UserName = @user AND PassWord = @pass";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@user", tk);
                cmd.Parameters.AddWithValue("@pass", mk);

                int kq = (int)cmd.ExecuteScalar(); // Trả về số lượng tìm thấy

                return kq > 0; // Nếu > 0 là có tài khoản -> Đúng
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
                return false;
            }
            finally
            {
                if (conn != null && conn.State != System.Data.ConnectionState.Closed)
                    conn.Close();
            }
        }
    }
}