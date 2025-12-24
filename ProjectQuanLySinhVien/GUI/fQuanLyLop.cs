using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyLop : Form
    {
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection conn = null;
        public fQuanLyLop()
        {
            InitializeComponent();
            LoadComboBoxKhoa();
            LoadGrid();
            PhanQuyen();
        }
        void PhanQuyen()
        {
            if (BienToanCuc.LoaiTaiKhoan == 0) 
            {
                btnThem.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                txbMaLop.ReadOnly = true;
                txbTenLop.ReadOnly = true;
                cboKhoa.Enabled = false;
                this.Text = "Quản Lý Lớp (Quyền: Nhân viên - Chỉ xem)";
            }
            else
            {
                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnLamMoi.Enabled = true;

                txbMaLop.ReadOnly = false;
                txbTenLop.ReadOnly = false;
                cboKhoa.Enabled = true;
                this.Text = "Quản Lý Lớp (Quyền: Admin)";
            }
        
        }
        void LoadComboBoxKhoa()
        {
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaKhoa, TenKhoa FROM KHOA";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        cboKhoa.DataSource = dt;
                        cboKhoa.DisplayMember = "MaKhoa";
                        cboKhoa.ValueMember = "MaKhoa";
                    }
                }
                catch { }
            }
        }
        void LoadGrid()
        {
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM LOP";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải Lớp: " + ex.Message);
                }
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            string maLop = txbMaLop.Text.Trim();
            string tenLop = txbTenLop.Text.Trim();
            if (string.IsNullOrEmpty(maLop) && string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập Mã lớp và Tên lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbMaLop.Focus();
                return;
            }
            if (string.IsNullOrEmpty(maLop))
            {
                MessageBox.Show("Vui lòng nhập Mã lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbMaLop.Focus();
                return;
            }
            if (string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập Tên lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbTenLop.Focus();
                return;
            }
            if (cboKhoa.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Khoa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboKhoa.Focus();
                return;
            }
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string checkSQL = "SELECT COUNT(*) FROM LOP WHERE MaLop = @ma";
                    SqlCommand cmdCheck = new SqlCommand(checkSQL, conn);
                    cmdCheck.Parameters.AddWithValue("@ma", maLop);

                    if ((int)cmdCheck.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Mã lớp [" + maLop + "] đã tồn tại trong hệ thống!", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txbMaLop.Focus();
                        txbMaLop.SelectAll();
                        return;
                    }
                    string sql = "INSERT INTO LOP (MaLop, TenLop, MaKhoa) VALUES (@ma, @ten, @makhoa)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ma", maLop);
                    cmd.Parameters.AddWithValue("@ten", tenLop);
                    cmd.Parameters.AddWithValue("@makhoa", cboKhoa.SelectedValue.ToString());

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm lớp mới thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadGrid();
                    LamMoi();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thêm: " + ex.Message);
                }
            }
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txbMaLop.Text == "") return;

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string sql = "UPDATE LOP SET TenLop = @ten, MaKhoa = @makhoa WHERE MaLop = @ma";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ma", txbMaLop.Text);
                    cmd.Parameters.AddWithValue("@ten", txbTenLop.Text);
                    cmd.Parameters.AddWithValue("@makhoa", cboKhoa.SelectedValue.ToString());
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!");
                        LoadGrid();
                        LamMoi();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy Mã lớp [" + txbMaLop.Text + "] để sửa!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi sửa: " + ex.Message);
                }
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txbMaLop.Text == "") return;
            if (MessageBox.Show("Bạn có chắc muốn xóa lớp [" + txbMaLop.Text + "] không?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string sqlCheck = "SELECT COUNT(*) FROM SINHVIEN WHERE MaLop = @ma";
                    SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn);
                    cmdCheck.Parameters.AddWithValue("@ma", txbMaLop.Text);

                    int soLuongSinhVien = (int)cmdCheck.ExecuteScalar(); 

                    if (soLuongSinhVien > 0)
                    {
                        MessageBox.Show("Không thể xóa lớp này!\n\nLý do: Lớp [" + txbMaLop.Text + "] đang có " + soLuongSinhVien + " sinh viên theo học.\n\nHãy xóa hoặc chuyển lớp cho các sinh viên này trước.",
                                        "Cảnh báo ràng buộc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    string sqlDelete = "DELETE FROM LOP WHERE MaLop = @ma";
                    SqlCommand cmdDelete = new SqlCommand(sqlDelete, conn);
                    cmdDelete.Parameters.AddWithValue("@ma", txbMaLop.Text);

                    int rows = cmdDelete.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Đã xóa lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGrid(); 
                        LamMoi();   
                    }
                    else
                    {
                        MessageBox.Show("Lớp này không còn tồn tại để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                  
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message);
                }
            }
        }
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        void LamMoi()
        {
            txbMaLop.Clear();
            txbTenLop.Clear();
            txbMaLop.Enabled = true;
            if (cboKhoa.Items.Count > 0) cboKhoa.SelectedIndex = 0;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex == dataGridView1.NewRowIndex) return;

            try
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txbMaLop.Text = row.Cells[0].Value.ToString();
                txbTenLop.Text = row.Cells[1].Value.ToString();
                if (row.Cells[2].Value != null && row.Cells[2].Value != DBNull.Value)
                {
                    string maKhoa = row.Cells[2].Value.ToString();
                    cboKhoa.SelectedValue = maKhoa;
                }

                txbMaLop.Enabled = false;
            }
            catch (Exception)
            {
                
            }
        }

        private void txbTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTim.PerformClick();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tuKhoa = txbTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(tuKhoa)) { 
                LoadGrid(); return; 
            } 

            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();
                string sql = @"SELECT * FROM LOP 
                       WHERE MaLop LIKE '%' + @kw + '%' 
                          OR TenLop COLLATE SQL_Latin1_General_CP1_CI_AI LIKE N'%' + @kw + '%'";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", tuKhoa);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy Lớp nào phù hợp!", "Thông báo");
                }
            }
            catch (Exception ex) { 
                MessageBox.Show("Lỗi: " + ex.Message); 
            }
        }
    }
}