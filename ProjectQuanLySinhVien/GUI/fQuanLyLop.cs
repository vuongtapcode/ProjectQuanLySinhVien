using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyLop : Form
    {
        // 1. CHUỖI KẾT NỐI
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";

        public fQuanLyLop()
        {
            InitializeComponent();

            // Tự động kích hoạt nút bấm
            GanSuKien();

            // Tải dữ liệu
            LoadComboBoxKhoa();
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
            if (txbMaLop.Text.Trim() == "" || txbTenLop.Text.Trim() == "")
            {
                MessageBox.Show("Vui lòng nhập Mã lớp và Tên lớp!"); return;
            }
            if (cboKhoa.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Khoa!"); return;
            }

            using (SqlConnection conn = new SqlConnection(strKetNoi))
            {
                try
                {
                    conn.Open();

                    // Check trùng
                    string checkSQL = "SELECT COUNT(*) FROM LOP WHERE MaLop = @ma";
                    SqlCommand cmdCheck = new SqlCommand(checkSQL, conn);
                    cmdCheck.Parameters.AddWithValue("@ma", txbMaLop.Text.Trim());

                    if ((int)cmdCheck.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Mã lớp này đã tồn tại!"); return;
                    }

                    // Thêm
                    string sql = "INSERT INTO LOP (MaLop, TenLop, MaKhoa) VALUES (@ma, @ten, @makhoa)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ma", txbMaLop.Text.Trim());
                    cmd.Parameters.AddWithValue("@ten", txbTenLop.Text.Trim());
                    cmd.Parameters.AddWithValue("@makhoa", cboKhoa.SelectedValue.ToString());

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

        // ================= NÚT SỬA (Đã Fix logic kiểm tra) =================
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

        // ================= NÚT XÓA (Đã Fix logic kiểm tra) =================
        // ================= NÚT XÓA (Phiên bản thân thiện: Kiểm tra trước khi xóa) =================
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txbMaLop.Text == "") return;

            // Hỏi xác nhận trước cho chắc ăn
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
                        // Nếu có sinh viên thì báo lỗi dễ hiểu và DỪNG LẠI NGAY
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
                    // Lỗi khác (ví dụ mất mạng, lỗi server...)
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
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txbMaLop.Text = row.Cells["MaLop"].Value.ToString();
                txbTenLop.Text = row.Cells["TenLop"].Value.ToString();

                string maKhoa = row.Cells["MaKhoa"].Value.ToString();
                cboKhoa.SelectedValue = maKhoa;

                txbMaLop.Enabled = false;
            }
        }
    }
}