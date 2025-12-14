using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ProjectQuanLySinhVien.DTO; // Nhớ dòng này

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fDangNhap : Form
    {
        // Chuỗi kết nối
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True";

        public fDangNhap()
        {
            InitializeComponent();
        }

        // --- SỰ KIỆN: KHI FORM VỪA MỞ LÊN ---
        private void fDangNhap_Load(object sender, EventArgs e)
        {
            try
            {
                // Tự động điền nếu đã lưu trước đó
                if (!string.IsNullOrEmpty(Properties.Settings.Default.SavedUsername))
                {
                    txbTaiKhoan.Text = Properties.Settings.Default.SavedUsername;
                    txbMatKhau.Text = Properties.Settings.Default.SavedPassword;
                    chkGhiNho.Checked = true; // Tích sẵn vào ô ghi nhớ
                }
            }
            catch
            {
                // Bỏ qua lỗi nếu chưa có Setting
            }
        }

        // --- SỰ KIỆN: BẤM NÚT ĐĂNG NHẬP ---
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string taiKhoan = txbTaiKhoan.Text;
            string matKhau = txbMatKhau.Text;

            // 1. Kiểm tra rỗng
            if (string.IsNullOrEmpty(taiKhoan) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đủ thông tin!");
                return;
            }

            // 2. Gọi hàm kiểm tra Database
            Account loginAccount = LayTaiKhoan(taiKhoan, matKhau);

            if (loginAccount != null) // Đăng nhập thành công
            {
                // Xử lý Ghi nhớ mật khẩu (Đã sửa logic chuẩn)
                if (chkGhiNho.Checked)
                {
                    Properties.Settings.Default.SavedUsername = taiKhoan;
                    Properties.Settings.Default.SavedPassword = matKhau;
                }
                else
                {
                    // Nếu bỏ tích thì phải XÓA mật khẩu đã lưu đi
                    Properties.Settings.Default.SavedUsername = "";
                    Properties.Settings.Default.SavedPassword = "";
                }
                Properties.Settings.Default.Save(); // Lưu thay đổi

                MessageBox.Show("Đăng nhập thành công! Xin chào " + loginAccount.DisplayName);

                // --- QUAN TRỌNG: Truyền Account sang Form Chính ---
                fSinhVien f = new fSinhVien(loginAccount);

                this.Hide();
                f.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Sai tên tài khoản hoặc mật khẩu!");
            }
        }

        // --- HÀM XỬ LÝ SQL ---
        private Account LayTaiKhoan(string tk, string mk)
        {
            try
            {
                string query = "SELECT * FROM ACCOUNT WHERE UserName = @user AND PassWord = @pass";

                using (SqlConnection connection = new SqlConnection(strKetNoi))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@user", tk);
                    command.Parameters.AddWithValue("@pass", mk);

                    DataTable data = new DataTable();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(data);

                    // Nếu tìm thấy tài khoản
                    if (data.Rows.Count > 0)
                    {
                        return new Account(data.Rows[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message);
            }
            return null;
        }

        // Nút Thoát (Viết thêm cho đủ bộ nếu cần)
        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}