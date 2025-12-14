using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyMonHoc : Form
    {
        // 1. Chuỗi kết nối chuẩn
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";

        public fQuanLyMonHoc()
        {
            InitializeComponent();

            // --- QUAN TRỌNG: GỌI HÀM NÀY ĐỂ TỰ ĐỘNG NỐI SỰ KIỆN CHO NÚT BẤM ---
            // (Giúp bạn không cần chỉnh trong Designer nữa)
            GanSuKien();

            // Tải dữ liệu lên bảng ngay khi mở form
            LoadGrid();
        }

        // Hàm này sẽ tự động bắt sự kiện Click cho các nút
        private void GanSuKien()
        {
            btnThem.Click += new EventHandler(btnThem_Click);
            btnSua.Click += new EventHandler(btnSua_Click);
            btnXoa.Click += new EventHandler(btnXoa_Click);
            btnLamMoi.Click += new EventHandler(btnLamMoi_Click);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
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
            if (txtMaMon.Text.Trim() == "") return;

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

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        MessageBox.Show("Cập nhật thành công!");
                        LoadGrid();
                        LamMoi();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy mã môn để sửa!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi sửa: " + ex.Message);
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
            txtMaMon.Enabled = true; // Cho phép nhập lại mã
        }

        // ================= CLICK VÀO BẢNG =================
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            txtMaMon.Text = row.Cells["MaMH"].Value.ToString();
            txtTenMon.Text = row.Cells["TenMH"].Value.ToString();

            if (row.Cells["SoTinChi"].Value != DBNull.Value)
                nmrSoTinChi.Value = Convert.ToDecimal(row.Cells["SoTinChi"].Value);
            else
                nmrSoTinChi.Value = 0;

            txtMaMon.Enabled = false; // Khóa mã lại
        }
    }
}