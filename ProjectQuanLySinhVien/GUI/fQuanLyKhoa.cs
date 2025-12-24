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
            PhanQuyen();
        }
        void PhanQuyen()
        {
            if (BienToanCuc.LoaiTaiKhoan == 0) 
            {
                btnThem.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                txtMaKhoa.Enabled = false;
                txtTenKhoa.Enabled = false;

                this.Text = "Quản Lý Khoa (Quyền: Nhân viên - Chỉ xem)";
            }
            else
            {
                this.Text = "Quản Lý Khoa (Quyền: Admin)";
            }
        
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
           
            if (string.IsNullOrWhiteSpace(txtMaKhoa.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã Khoa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaKhoa.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenKhoa.Text))
            {
                MessageBox.Show("Vui lòng nhập Tên Khoa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKhoa.Focus(); 
                return;
            }
            string ma = txtMaKhoa.Text.Trim();
            if (KiemTraMaKhoaTonTai(ma))
            {
                MessageBox.Show("Mã khoa [" + ma + "] đã tồn tại.\nVui lòng nhập mã khác.", "Trùng mã", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaKhoa.Focus();
                txtMaKhoa.SelectAll();
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

                MessageBox.Show("Thêm khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ResetForm();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    MessageBox.Show("Lỗi: Mã khoa bị trùng trong CSDL!", "Lỗi trùng khóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("Lỗi SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open) conn.Close();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaKhoa.Text))
            {
                MessageBox.Show("Vui lòng chọn Khoa cần sửa từ danh sách!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenKhoa.Text))
            {
                MessageBox.Show("Tên Khoa không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenKhoa.Focus();
                return;
            }

            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();

                string sql = "UPDATE KHOA SET TenKhoa=@ten WHERE MaKhoa=@ma";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", txtMaKhoa.Text.Trim());
                cmd.Parameters.AddWithValue("@ten", txtTenKhoa.Text.Trim());

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    MessageBox.Show("Cập nhật thông tin Khoa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ResetForm();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy Mã Khoa để sửa (Có thể đã bị xóa).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
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

        private void txbTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { 
                btnTim.PerformClick();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tuKhoa = txbTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(tuKhoa)) { 
                LoadData(); return; 
            }

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
              
                string sql = @"SELECT * FROM KHOA 
                               WHERE MaKhoa LIKE '%' + @kw + '%' 
                                  OR TenKhoa COLLATE SQL_Latin1_General_CP1_CI_AI LIKE N'%' + @kw + '%'";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", tuKhoa);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy Khoa nào phù hợp!", "Thông báo");
                }
            }
            catch (Exception ex) { 
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message); 
            }
        }
    }
}