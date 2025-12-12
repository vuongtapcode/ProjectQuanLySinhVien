using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyDiem : Form
    {
        // Cấu hình chuỗi kết nối
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True";
        SqlConnection conn = null;

        public fQuanLyDiem()
        {
            InitializeComponent();
            LoadDataComboBox(); // Load danh sách Lớp và Môn ngay khi mở
        }

        // --- 1. LOAD DỮ LIỆU CHO COMBOBOX ---
        private void LoadDataComboBox()
        {
            try
            {
                conn = new SqlConnection(strKetNoi);
                conn.Open();

                // Load Lớp
                SqlDataAdapter daLop = new SqlDataAdapter("SELECT MaLop, TenLop FROM LOP", conn);
                DataTable dtLop = new DataTable();
                daLop.Fill(dtLop);
                cboLop.DataSource = dtLop;
                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";

                // Load Môn Học
                SqlDataAdapter daMon = new SqlDataAdapter("SELECT MaMH, TenMH FROM MONHOC", conn);
                DataTable dtMon = new DataTable();
                daMon.Fill(dtMon);
                cboMonHoc.DataSource = dtMon;
                cboMonHoc.DisplayMember = "TenMH";
                cboMonHoc.ValueMember = "MaMH";

                // Đặt mặc định chưa chọn gì
                cboLop.SelectedIndex = -1;
                cboMonHoc.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối: " + ex.Message); }
        }

        // --- 2. LOAD DANH SÁCH SINH VIÊN & ĐIỂM ---
        private void LoadDiemSinhVien()
        {
            // Chỉ load khi đã chọn đủ cả Lớp và Môn
            if (cboLop.SelectedValue == null || cboMonHoc.SelectedValue == null) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Câu lệnh SQL: Lấy tất cả SV lớp đó, nếu có điểm thì hiện, chưa có thì hiện NULL
                string sql = @"
                    SELECT 
                        sv.MaSV, 
                        sv.HoTen, 
                        kq.DiemSo 
                    FROM SINHVIEN sv
                    LEFT JOIN KETQUA kq ON sv.MaSV = kq.MaSV AND kq.MaMH = @maMH
                    WHERE sv.MaLop = @maLop";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@maMH", cboMonHoc.SelectedValue);
                cmd.Parameters.AddWithValue("@maLop", cboLop.SelectedValue);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // --- SỰ KIỆN CHỌN COMBOBOX ---
        private void cboLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDiemSinhVien();
        }

        private void cboMonHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDiemSinhVien();
        }

        // --- SỰ KIỆN CLICK BẢNG ---
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtMaSV.Text = row.Cells["colMaSV"].Value.ToString();
                txtTenSV.Text = row.Cells["colTenSV"].Value.ToString();
                txtDiem.Text = row.Cells["colDiem"].Value.ToString();

                txtDiem.Focus(); // Đặt con trỏ vào ô nhập điểm ngay
            }
        }

        // --- NÚT THÊM (Chỉ dùng khi chưa có điểm) ---
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Kiểm tra xem đã có điểm chưa
                if (KiemTraDiemTonTai())
                {
                    MessageBox.Show("Sinh viên này đã có điểm môn này rồi! Vui lòng dùng nút SỬA.");
                    return;
                }

                // Nếu chưa có thì Thêm mới
                string sql = "INSERT INTO KETQUA (MaSV, MaMH, DiemSo) VALUES (@sv, @mh, @d)";
                ExecuteSQL(sql);
                MessageBox.Show("Nhập điểm thành công!");
                LoadDiemSinhVien();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // --- NÚT SỬA (Chỉ dùng khi đã có điểm) ---
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // Kiểm tra xem có điểm để sửa không
                if (!KiemTraDiemTonTai())
                {
                    MessageBox.Show("Sinh viên này chưa có điểm! Vui lòng dùng nút THÊM.");
                    return;
                }

                // Nếu có rồi thì Update
                string sql = "UPDATE KETQUA SET DiemSo=@d WHERE MaSV=@sv AND MaMH=@mh";
                ExecuteSQL(sql);
                MessageBox.Show("Sửa điểm thành công!");
                LoadDiemSinhVien();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        // --- NÚT XÓA ---
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txtMaSV.Text == "") return;

            if (MessageBox.Show("Bạn muốn xóa điểm môn này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string sql = "DELETE FROM KETQUA WHERE MaSV=@sv AND MaMH=@mh";

                    // Code xóa đặc biệt vì Delete không cần tham số DiemSo
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@sv", txtMaSV.Text);
                    cmd.Parameters.AddWithValue("@mh", cboMonHoc.SelectedValue);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Xóa điểm thành công!");
                    LoadDiemSinhVien();
                    txtDiem.Text = "";
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
            }
        }

        // --- NÚT LÀM MỚI ---
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadDiemSinhVien();
            txtMaSV.Text = "";
            txtTenSV.Text = "";
            txtDiem.Text = "";
        }

        // --- CÁC HÀM PHỤ TRỢ (Để code gọn hơn) ---

        // 1. Kiểm tra đầu vào
        private bool CheckInput()
        {
            if (txtMaSV.Text == "" || cboMonHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Sinh viên và Môn học!");
                return false;
            }
            float diem;
            if (!float.TryParse(txtDiem.Text, out diem) || diem < 0 || diem > 10)
            {
                MessageBox.Show("Điểm phải là số từ 0 đến 10!");
                return false;
            }
            return true;
        }

        // 2. Kiểm tra xem điểm đã có trong SQL chưa
        private bool KiemTraDiemTonTai()
        {
            string check = "SELECT COUNT(*) FROM KETQUA WHERE MaSV=@sv AND MaMH=@mh";
            SqlCommand cmdCheck = new SqlCommand(check, conn);
            cmdCheck.Parameters.AddWithValue("@sv", txtMaSV.Text);
            cmdCheck.Parameters.AddWithValue("@mh", cboMonHoc.SelectedValue);
            int count = (int)cmdCheck.ExecuteScalar();
            return count > 0;
        }

        // 3. Hàm thực thi SQL chung (Cho Thêm và Sửa)
        private void ExecuteSQL(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sv", txtMaSV.Text);
            cmd.Parameters.AddWithValue("@mh", cboMonHoc.SelectedValue);
            cmd.Parameters.AddWithValue("@d", float.Parse(txtDiem.Text));
            cmd.ExecuteNonQuery();
        }
    }
}