using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyTaiKhoan : Form
    {
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection conn = null;
        public fQuanLyTaiKhoan()
        {
            InitializeComponent();
            LoadGrid();
        }
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
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbTenDangNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên đăng nhập!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbTenDangNhap.Focus(); return;
            }
            if (string.IsNullOrWhiteSpace(txbTenHienThi.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên hiển thị!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbTenHienThi.Focus(); return;
            }
            if (string.IsNullOrWhiteSpace(txbMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập Mật khẩu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbMatKhau.Focus(); return;
            }
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM ACCOUNT WHERE UserName = @User";
                    SqlCommand cmdCheck = new SqlCommand(checkQuery, conn);
                    cmdCheck.Parameters.AddWithValue("@User", txbTenDangNhap.Text.Trim());

                    if ((int)cmdCheck.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Tên đăng nhập này đã tồn tại!", "Lỗi trùng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txbTenDangNhap.SelectAll();
                        txbTenDangNhap.Focus();
                        return;
                    }
                    string query = "INSERT INTO ACCOUNT (UserName, DisplayName, PassWord, Type) VALUES (@User, @Name, @Pass, @Type)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@User", txbTenDangNhap.Text.Trim());
                    cmd.Parameters.AddWithValue("@Name", txbTenHienThi.Text.Trim());
                    cmd.Parameters.AddWithValue("@Pass", txbMatKhau.Text.Trim());
                    cmd.Parameters.AddWithValue("@Type", (int)nmLoaiTaiKhoan.Value);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm tài khoản thành công!", "Thông báo");

                    LoadGrid();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thêm: " + ex.Message);
                }
            }
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbTenDangNhap.Text))
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần sửa!", "Thông báo"); return;
            }

            if (string.IsNullOrWhiteSpace(txbTenHienThi.Text))
            {
                MessageBox.Show("Tên hiển thị không được để trống!", "Cảnh báo");
                txbTenHienThi.Focus(); return;
            }
            if (string.IsNullOrWhiteSpace(txbMatKhau.Text))
            {
                MessageBox.Show("Mật khẩu không được để trống!", "Cảnh báo");
                txbMatKhau.Focus(); return;
            }

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE ACCOUNT SET DisplayName = @Name, PassWord = @Pass, Type = @Type WHERE UserName = @User";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@User", txbTenDangNhap.Text); 
                    cmd.Parameters.AddWithValue("@Name", txbTenHienThi.Text.Trim());
                    cmd.Parameters.AddWithValue("@Pass", txbMatKhau.Text.Trim());
                    cmd.Parameters.AddWithValue("@Type", (int)nmLoaiTaiKhoan.Value);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo");
                        LoadGrid();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy tài khoản [" + txbTenDangNhap.Text + "] để sửa!", "Lỗi");
                    }
                }
                catch (Exception ex) { MessageBox.Show("Lỗi sửa: " + ex.Message); }
            }
        }
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
            if (e.RowIndex < 0 || e.RowIndex == dataGridView1.NewRowIndex) return;

            try
            {
                DataGridViewRow r = dataGridView1.Rows[e.RowIndex];
                txbTenDangNhap.Text = Convert.ToString(r.Cells[1].Value);
                txbTenHienThi.Text = Convert.ToString(r.Cells[2].Value);
                txbMatKhau.Text = Convert.ToString(r.Cells[3].Value);
                string val = Convert.ToString(r.Cells[4].Value);
                if (!string.IsNullOrEmpty(val))
                {
                    decimal so;
                    if (decimal.TryParse(val, out so))
                    {
                        if (so >= nmLoaiTaiKhoan.Minimum && so <= nmLoaiTaiKhoan.Maximum)
                        {
                            nmLoaiTaiKhoan.Value = so;
                        }
                        else
                        {
                            nmLoaiTaiKhoan.Value = 0;
                        }
                    }
                    else
                    {
                        nmLoaiTaiKhoan.Value = 0;
                    }
                }
                else
                {
                    nmLoaiTaiKhoan.Value = 0;
                }

                txbTenDangNhap.Enabled = false; 
            }
            catch (Exception)
            {
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tuKhoa = txbTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(tuKhoa))
            {
                LoadGrid();
                return;
            }

            try
            {

                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();
                string sql = @"SELECT UserName, DisplayName, PassWord, Type 
                           FROM ACCOUNT 
                           WHERE UserName LIKE '%' + @kw + '%' 
                              OR DisplayName COLLATE SQL_Latin1_General_CP1_CI_AI LIKE N'%' + @kw + '%'";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", tuKhoa);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy Tài khoản nào phù hợp!", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void txbTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTim.PerformClick();
            }
        }
    }
}