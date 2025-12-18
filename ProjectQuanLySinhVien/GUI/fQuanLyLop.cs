using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyLop : Form
    {
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";

        public fQuanLyLop()
        {
            InitializeComponent();
            LoadComboBoxKhoa();
            LoadGrid();
            PhanQuyen();
        }
        void PhanQuyen()
        {
            if (BienToanCuc.LoaiTaiKhoan == 0) // Là Nhân viên
            {
                btnThem.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
            }
        }
        

        // ================= HÀM TẢI DANH SÁCH KHOA =================
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

        // ================= HÀM TẢI DANH SÁCH LỚP =================
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

        // ================= NÚT THÊM =================
        private void btnThem_Click(object sender, EventArgs e)
        {
            // --- KIỂM TRA ĐẦU VÀO ---
            string maLop = txbMaLop.Text.Trim();
            string tenLop = txbTenLop.Text.Trim();

            // 1. Kiểm tra thiếu cả hai
            if (string.IsNullOrEmpty(maLop) && string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập Mã lớp và Tên lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbMaLop.Focus();
                return;
            }

            // 2. Kiểm tra chỉ thiếu Mã lớp
            if (string.IsNullOrEmpty(maLop))
            {
                MessageBox.Show("Vui lòng nhập Mã lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbMaLop.Focus();
                return;
            }

            // 3. Kiểm tra chỉ thiếu Tên lớp
            if (string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập Tên lớp!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbTenLop.Focus();
                return;
            }

            // 4. Kiểm tra chọn Khoa
            if (cboKhoa.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Khoa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboKhoa.Focus();
                return;
            }

            // --- PHẦN XỬ LÝ SQL ---
            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // Check trùng
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

                    // Thêm
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

        // ================= NÚT SỬA  =================
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

                    // Kiểm tra số dòng bị ảnh hưởng
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

        
        // ================= NÚT XÓA  =================
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

                    // --- BƯỚC 1: KIỂM TRA XEM CÓ SINH VIÊN KHÔNG ---
                    string sqlCheck = "SELECT COUNT(*) FROM SINHVIEN WHERE MaLop = @ma";
                    SqlCommand cmdCheck = new SqlCommand(sqlCheck, conn);
                    cmdCheck.Parameters.AddWithValue("@ma", txbMaLop.Text);

                    int soLuongSinhVien = (int)cmdCheck.ExecuteScalar(); // Lấy số lượng SV

                    if (soLuongSinhVien > 0)
                    {
                        // Nếu có sinh viên thì báo lỗi và DỪNG LẠI NGAY
                        MessageBox.Show("Không thể xóa lớp này!\n\nLý do: Lớp [" + txbMaLop.Text + "] đang có " + soLuongSinhVien + " sinh viên theo học.\n\nHãy xóa hoặc chuyển lớp cho các sinh viên này trước.",
                                        "Cảnh báo ràng buộc", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // --- BƯỚC 2: NẾU TRỐNG (0 SINH VIÊN) THÌ MỚI XÓA ---
                    string sqlDelete = "DELETE FROM LOP WHERE MaLop = @ma";
                    SqlCommand cmdDelete = new SqlCommand(sqlDelete, conn);
                    cmdDelete.Parameters.AddWithValue("@ma", txbMaLop.Text);

                    int rows = cmdDelete.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("Đã xóa lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadGrid(); // Tải lại bảng
                        LamMoi();   // Xóa trắng ô nhập
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

        // ================= NÚT LÀM MỚI =================
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

        // ================= CLICK BẢNG =================
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 1. Chặn click vào dòng tiêu đề (-1) hoặc dòng trống cuối cùng
            if (e.RowIndex < 0 || e.RowIndex == dataGridView1.NewRowIndex) return;

            try
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txbMaLop.Text = row.Cells[0].Value.ToString();
                txbTenLop.Text = row.Cells[1].Value.ToString();

                // 3. Xử lý ComboBox Khoa (Kiểm tra null cho an toàn)
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

       
    }
}