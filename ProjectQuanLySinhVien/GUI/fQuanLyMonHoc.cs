using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyMonHoc : Form
    {
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection conn = null;
        public fQuanLyMonHoc()
        {
            InitializeComponent();
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
                txtMaMon.ReadOnly = true;
                txtTenMon.ReadOnly = true;
                nmrSoTinChi.Enabled = false;
                this.Text = "Quản Lý Môn Học (Quyền: Nhân viên - Chỉ xem)";
            }
            else
            {
                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                btnLamMoi.Enabled = true;

                txtMaMon.ReadOnly = false;
                txtTenMon.ReadOnly = false;
                nmrSoTinChi.Enabled = true;

                this.Text = "Quản Lý Môn Học (Quyền: Admin)";
            
        }
    }
        private void LoadGrid()
        {
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM MONHOC";
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
            if (txtMaMon.Text.Trim() == "" || txtTenMon.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!"); return;
            }

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM MONHOC WHERE MaMH = @MaMH";
                    SqlCommand cmdCheck = new SqlCommand(checkQuery, conn);
                    cmdCheck.Parameters.AddWithValue("@MaMH", txtMaMon.Text.Trim());

                    if ((int)cmdCheck.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Mã môn học này đã tồn tại!"); return;
                    }
                    string query = "INSERT INTO MONHOC (MaMH, TenMH, SoTinChi) VALUES (@MaMH, @TenMH, @SoTinChi)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MaMH", txtMaMon.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenMH", txtTenMon.Text.Trim());
                    cmd.Parameters.AddWithValue("@SoTinChi", nmrSoTinChi.Value);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm thành công!");

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
            if (string.IsNullOrWhiteSpace(txtMaMon.Text))
            {
                MessageBox.Show("Vui lòng chọn môn học cần sửa!", "Thông báo");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Tên môn học không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenMon.Focus(); 
                return;
            }
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE MONHOC SET TenMH = @TenMH, SoTinChi = @SoTinChi WHERE MaMH = @MaMH";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@MaMH", txtMaMon.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenMH", txtTenMon.Text.Trim());
                    cmd.Parameters.AddWithValue("@SoTinChi", nmrSoTinChi.Value);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGrid(); 
                        LamMoi();  
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy Mã môn học [" + txtMaMon.Text + "] để sửa.\n(Có thể mã này đã bị xóa bởi người khác).", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi sửa dữ liệu: " + ex.Message);
                }
            }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txtMaMon.Text.Trim() == "") return;

            if (MessageBox.Show("Bạn có chắc muốn xóa môn " + txtMaMon.Text + "?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM MONHOC WHERE MaMH = @MaMH";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@MaMH", txtMaMon.Text.Trim());

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Xóa thành công!");
                        LoadGrid();
                        LamMoi();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy mã môn để xóa!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa môn này (Đang có điểm thi hoặc sinh viên học môn này)!\nChi tiết: " + ex.Message);
                }
            }
        }
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LamMoi();
        }

        private void LamMoi()
        {
            txtMaMon.Clear();
            txtTenMon.Clear();
            nmrSoTinChi.Value = 3;
            txtMaMon.Enabled = true; 
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex == dataGridView1.NewRowIndex) return;

            try
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtMaMon.Text = row.Cells[0].Value.ToString();
                txtTenMon.Text = row.Cells[1].Value.ToString();
                if (row.Cells[2].Value != DBNull.Value && row.Cells[2].Value != null)
                {
                    nmrSoTinChi.Value = Convert.ToDecimal(row.Cells[2].Value);
                }
                else
                {
                    nmrSoTinChi.Value = 0;
                }

                txtMaMon.Enabled = false; 
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
                string sql = @"SELECT * FROM MONHOC 
                       WHERE MaMH LIKE '%' + @kw + '%' 
                          OR TenMH COLLATE SQL_Latin1_General_CP1_CI_AI LIKE N'%' + @kw + '%'";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", tuKhoa);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy Môn học nào phù hợp!", "Thông báo");
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