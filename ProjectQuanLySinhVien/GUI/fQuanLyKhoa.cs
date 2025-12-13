using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyKhoa : Form
    {
        
        private string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;Connect Timeout=3";
        SqlConnection conn = null;

        public fQuanLyKhoa()
        {
            InitializeComponent();
            LoadData();
            GanSuKien(); 
        }

        
        private void GanSuKien()
        {
            btnThem.Click += new EventHandler(btnThem_Click);
            btnSua.Click += new EventHandler(btnSua_Click);
            btnXoa.Click += new EventHandler(btnXoa_Click);
            btnLamMoi.Click += new EventHandler(btnLamMoi_Click);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
        }

        private void LoadData()
        {
            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);

                
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM KHOA", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Lỗi khi tải dữ liệu Khoa: " + ex.Message, "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Kiểm tra mã khoa đã tồn tại chưa
        private bool KiemTraMaKhoaTonTai(string maKhoa)
        {
            if (string.IsNullOrWhiteSpace(maKhoa)) return false;
            try
            {
                using (var localConn = new SqlConnection(strKetNoi))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM KHOA WHERE MaKhoa=@ma", localConn))
                {
                    cmd.Parameters.AddWithValue("@ma", maKhoa.Trim());
                    localConn.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
            catch (Exception ex)
            {
                // Nếu không thể kiểm tra, báo lỗi và trả về true để ngăn insert trùng khóa do lỗi kết nối
                MessageBox.Show("Không thể kiểm tra tồn tại Mã Khoa. Vui lòng kiểm tra kết nối.\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtMaKhoa.Text = row.Cells["colMaKhoa"].Value?.ToString() ?? string.Empty;
                txtTenKhoa.Text = row.Cells["colTenKhoa"].Value?.ToString() ?? string.Empty;
                txtMaKhoa.Enabled = false;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (txtMaKhoa.Text == "") { MessageBox.Show("Vui lòng nhập Mã Khoa!"); return; }

            string ma = txtMaKhoa.Text.Trim();
            if (KiemTraMaKhoaTonTai(ma))
            {
                MessageBox.Show("Mã khoa đã tồn tại. Vui lòng nhập mã khác.", "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMaKhoa.Focus();
                return;
            }

            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();
                string sql = "INSERT INTO KHOA (MaKhoa, TenKhoa) VALUES (@ma, @ten)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", ma);
                cmd.Parameters.AddWithValue("@ten", txtTenKhoa.Text.Trim());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm thành công!");
                LoadData();
                ResetForm();
            }
            catch (SqlException ex)
            {
                // Sql error codes 2627 and 2601 indicate unique constraint / PK violation
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Không thể thêm: Mã khoa đã tồn tại (khóa chính trùng). Vui lòng nhập mã khác.", "Lỗi trùng khóa", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();
                string sql = "UPDATE KHOA SET TenKhoa=@ten WHERE MaKhoa=@ma";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", txtMaKhoa.Text.Trim());
                cmd.Parameters.AddWithValue("@ten", txtTenKhoa.Text.Trim());
                cmd.ExecuteNonQuery();
                MessageBox.Show("Sửa thành công!");
                LoadData();
                ResetForm();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xóa khoa này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    if (conn == null) conn = new SqlConnection(strKetNoi);
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM KHOA WHERE MaKhoa=@ma", conn);
                    cmd.Parameters.AddWithValue("@ma", txtMaKhoa.Text.Trim());
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Xóa thành công!");
                    LoadData();
                    ResetForm();
                }
                catch (Exception ex) { MessageBox.Show("Không thể xóa: " + ex.Message); }
                finally
                {
                    if (conn != null && conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtMaKhoa.Text = "";
            txtTenKhoa.Text = "";
            txtMaKhoa.Enabled = true;
            txtMaKhoa.Focus();
        }
    }
}