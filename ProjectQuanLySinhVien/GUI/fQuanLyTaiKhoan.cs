using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyTaiKhoan : Form
    {
        // 1. CHUỖI KẾT NỐI CHUẨN (Đã sửa tên máy và DB)
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";

        public fQuanLyTaiKhoan()
        {
            InitializeComponent();

            // 2. Tự động kích hoạt nút bấm
            GanSuKien();

            // 3. Tự động tải dữ liệu
            LoadGrid();
        }

        private void GanSuKien()
        {
            btnThem.Click += new EventHandler(btnThem_Click);
            btnSua.Click += new EventHandler(btnSua_Click);
            btnXoa.Click += new EventHandler(btnXoa_Click);
            btnLamMoi.Click += new EventHandler(btnLamMoi_Click);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
        }

        // ================= LOAD GRID =================
        void LoadGrid()
        {
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT UserName, DisplayName, PassWord, Type FROM ACCOUNT";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
                }
            }
        }

        // ================= THÊM =================
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txbTenDangNhap.Text.Trim() == "" || txbMatKhau.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Tên đăng nhập và Mật khẩu!"); return;
            }

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // Check trùng
                    string checkQuery = "SELECT COUNT(*) FROM ACCOUNT WHERE UserName = @User";
                    SqlCommand cmdCheck = new SqlCommand(checkQuery, conn);
                    cmdCheck.Parameters.AddWithValue("@User", txbTenDangNhap.Text.Trim());

                    if ((int)cmdCheck.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Tên đăng nhập đã tồn tại!"); return;
                    }

                    // Thêm
                    string query = "INSERT INTO ACCOUNT (UserName, DisplayName, PassWord, Type) VALUES (@User, @Name, @Pass, @Type)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@User", txbTenDangNhap.Text.Trim());
                    cmd.Parameters.AddWithValue("@Name", txbTenHienThi.Text.Trim());
                    cmd.Parameters.AddWithValue("@Pass", txbMatKhau.Text.Trim());
                    cmd.Parameters.AddWithValue("@Type", (int)nmLoaiTaiKhoan.Value);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm thành công!");
                    LoadGrid();
                    ClearForm();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi thêm: " + ex.Message); }
            }
        }

        // ================= SỬA =================
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txbTenDangNhap.Text == "") return;

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE ACCOUNT SET DisplayName = @Name, PassWord = @Pass, Type = @Type WHERE UserName = @User";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@User", txbTenDangNhap.Text);
                    cmd.Parameters.AddWithValue("@Name", txbTenHienThi.Text);
                    cmd.Parameters.AddWithValue("@Pass", txbMatKhau.Text);
                    cmd.Parameters.AddWithValue("@Type", (int)nmLoaiTaiKhoan.Value);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Sửa thành công!");
                        LoadGrid();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy tài khoản!");
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi sửa: " + ex.Message); }
            }
        }

        // ================= XÓA =================
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txbTenDangNhap.Text == "") return;
            if (MessageBox.Show("Xóa tài khoản này?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.No) return;

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM ACCOUNT WHERE UserName = @User", conn);
                    cmd.Parameters.AddWithValue("@User", txbTenDangNhap.Text);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadGrid();
                        ClearForm();
                    }
                    else MessageBox.Show("Lỗi khi xóa!");
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e) { ClearForm(); }

        void ClearForm()
        {
            txbTenDangNhap.Clear();
            txbTenHienThi.Clear();
            txbMatKhau.Clear();
            nmLoaiTaiKhoan.Value = 0;
            txbTenDangNhap.Enabled = true;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridViewRow r = dataGridView1.Rows[e.RowIndex];
            txbTenDangNhap.Text = r.Cells["UserName"].Value.ToString();
            txbTenHienThi.Text = r.Cells["DisplayName"].Value.ToString();
            txbMatKhau.Text = r.Cells["PassWord"].Value.ToString();
            if (r.Cells["Type"].Value != DBNull.Value)
                nmLoaiTaiKhoan.Value = Convert.ToDecimal(r.Cells["Type"].Value);
            txbTenDangNhap.Enabled = false;
        }

        // --- Hàm này để sửa lỗi label1_Click bị thiếu (Cứ để đây cho hết lỗi) ---
        private void label1_Click(object sender, EventArgs e)
        {
        }
    }
}