using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyMonHoc : Form
    {
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";

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
            }
        }
        
        

        // ================= LOAD GRID (Tải dữ liệu) =================
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

        // ================= THÊM (INSERT) =================
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

                    // 1. Kiểm tra trùng
                    string checkQuery = "SELECT COUNT(*) FROM MONHOC WHERE MaMH = @MaMH";
                    SqlCommand cmdCheck = new SqlCommand(checkQuery, conn);
                    cmdCheck.Parameters.AddWithValue("@MaMH", txtMaMon.Text.Trim());

                    if ((int)cmdCheck.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Mã môn học này đã tồn tại!"); return;
                    }

                    // 2. Thêm mới
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

        // ================= SỬA (UPDATE) =================
        private void btnSua_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã chọn/nhập Mã môn chưa
            if (string.IsNullOrWhiteSpace(txtMaMon.Text))
            {
                MessageBox.Show("Vui lòng chọn môn học cần sửa!", "Thông báo");
                return;
            }

            // 2. Kiểm tra Tên môn có bị bỏ trống không
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Tên môn học không được để trống!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenMon.Focus(); // Đưa con trỏ chuột về ô Tên môn để nhập lại
                return;
            }

            // 3. Thực hiện Update
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

                    // Kiểm tra xem có dòng nào được cập nhật không
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGrid(); // Tải lại bảng
                        LamMoi();   // Xóa trắng ô nhập
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

        // ================= XÓA (DELETE) =================
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

        // ================= LÀM MỚI =================
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

        // ================= CLICK VÀO BẢNG =================
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex == dataGridView1.NewRowIndex) return;

            try
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtMaMon.Text = row.Cells[0].Value.ToString();
                txtTenMon.Text = row.Cells[1].Value.ToString();

                // Xử lý số tín chỉ 
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

        
    }
}