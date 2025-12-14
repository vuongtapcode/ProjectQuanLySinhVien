using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ProjectQuanLySinhVien.DTO;
namespace ProjectQuanLySinhVien.GUI
{
    public partial class fSinhVien : Form
    {
        private Account loginAccount; // Biến lưu tài khoản hiện tại
        // 1. CHUỖI KẾT NỐI (Bạn nói đã đúng, nhưng hãy kiểm tra kỹ tên máy tính nhé)
        // Nếu vẫn không lên, hãy thử đổi lại thành: Data Source=.; ...
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";

        SqlConnection conn = null;

        public fSinhVien()
        {
            InitializeComponent();
            GanSuKien();
            LoadDataSinhVien();
        }

        // --- SỬA LẠI HÀM NÀY ---
        public fSinhVien(Account acc)
        {
            InitializeComponent(); // 1. Vẽ giao diện

            this.loginAccount = acc; // 2. Nhận tài khoản
            this.Text = "Quản Lý Sinh Viên - Xin chào: " + acc.DisplayName;
            // --- CẤU HÌNH ĐỊNH DẠNG NGÀY CHO Ô NHẬP LIỆU ---
            dtpkNgaySinh.Format = DateTimePickerFormat.Custom;
            dtpkNgaySinh.CustomFormat = "dd/MM/yyyy";

            stpkNgayNhapHoc.Format = DateTimePickerFormat.Custom;
            stpkNgayNhapHoc.CustomFormat = "dd/MM/yyyy";
            // 3. QUAN TRỌNG: Phải gọi 2 hàm này thì form mới chạy được
            GanSuKien();        // Gán sự kiện click cho các nút
            LoadDataSinhVien(); // Tải dữ liệu lên bảng
        }
        // --- HÀM TẢI DỮ LIỆU  ---
        private void LoadDataSinhVien()
        {
            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();

                // 1. Lấy dữ liệu từ SQL (Bao gồm cả Mã Khoa từ bảng Lớp)
                string sql = @"
                    SELECT 
                        SV.MaSV, 
                        SV.HoTen, 
                        SV.NgaySinh, 
                        SV.GioiTinh, 
                        SV.QueQuan, 
                        SV.NgayNhapHoc,
                        SV.MaLop, 
                        L.MaKhoa
                    FROM SINHVIEN SV
                    LEFT JOIN LOP L ON SV.MaLop = L.MaLop";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // 2. Tạo cột số thứ tự (STT) chạy bằng code
                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["STT"] = i + 1;
                }

                // 3. --- QUAN TRỌNG NHẤT: MAP DỮ LIỆU VÀO CỘT ---
                dataGridView1.AutoGenerateColumns = false; // Chặn tự sinh cột rác

                // Ép cột giao diện (Column...) nhận đúng tên cột SQL
                if (dataGridView1.Columns["Column1"] != null) dataGridView1.Columns["Column1"].DataPropertyName = "STT";
                if (dataGridView1.Columns["Column2"] != null) dataGridView1.Columns["Column2"].DataPropertyName = "MaSV";
                if (dataGridView1.Columns["Column3"] != null) dataGridView1.Columns["Column3"].DataPropertyName = "HoTen"; // Chú ý: SQL là HoTen
                if (dataGridView1.Columns["Column4"] != null)
                {
                    dataGridView1.Columns["Column4"].DataPropertyName = "NgaySinh";
                    dataGridView1.Columns["Column4"].DefaultCellStyle.Format = "dd/MM/yyyy";
                }
                if (dataGridView1.Columns["Column5"] != null) dataGridView1.Columns["Column5"].DataPropertyName = "GioiTinh";
                if (dataGridView1.Columns["Column6"] != null) dataGridView1.Columns["Column6"].DataPropertyName = "QueQuan";
                if (dataGridView1.Columns["Column7"] != null) dataGridView1.Columns["Column7"].DataPropertyName = "NgayNhapHoc";
                if (dataGridView1.Columns["Column8"] != null) dataGridView1.Columns["Column8"].DataPropertyName = "MaLop";
                if (dataGridView1.Columns["Column9"] != null) dataGridView1.Columns["Column9"].DataPropertyName = "MaKhoa";

                // 4. Đổ dữ liệu vào
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // --- CÁC HÀM XỬ LÝ SỰ KIỆN ---
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txbMSSV.Text = row.Cells["Column2"].Value?.ToString();
                txbTenSV.Text = row.Cells["Column3"].Value?.ToString();

                if (row.Cells["Column4"].Value != DBNull.Value)
                    dtpkNgaySinh.Value = Convert.ToDateTime(row.Cells["Column4"].Value);

                string gioitinh = row.Cells["Column5"].Value?.ToString();
                radNam.Checked = (gioitinh == "Nam");
                radNu.Checked = (gioitinh == "Nữ");

                txbQueQuan.Text = row.Cells["Column6"].Value?.ToString();

                if (row.Cells["Column7"].Value != DBNull.Value)
                    stpkNgayNhapHoc.Value = Convert.ToDateTime(row.Cells["Column7"].Value);

                txbMaLop.Text = row.Cells["Column8"].Value?.ToString();
                txbMaKhoa.Text = row.Cells["Column9"].Value?.ToString();

                txbMSSV.Enabled = false;
            }
        }
        private bool KiemTraTrungMSSV(string maSV)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                string sql = "SELECT COUNT(*) FROM SINHVIEN WHERE MaSV = @ma";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", maSV);

                int count = (int)cmd.ExecuteScalar(); // Trả về số lượng tìm thấy

                return count > 0; // Nếu > 0 tức là đã tồn tại
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kiểm tra trùng: " + ex.Message);
                return true; // Giả sử là trùng để chặn lại cho an toàn
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra rỗng (Validation)
            if (string.IsNullOrWhiteSpace(txbMSSV.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã Sinh Viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbMSSV.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txbTenSV.Text))
            {
                MessageBox.Show("Vui lòng nhập Họ Tên Sinh Viên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbTenSV.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txbMaLop.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã Lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbMaLop.Focus();
                return;
            }

            // 2. Kiểm tra trùng Mã Sinh Viên (Quan trọng nhất để sửa lỗi trong hình)
            if (KiemTraTrungMSSV(txbMSSV.Text))
            {
                MessageBox.Show("Mã sinh viên [" + txbMSSV.Text + "] đã tồn tại trong hệ thống!\nVui lòng nhập mã khác.",
                                "Trùng dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txbMSSV.Focus(); // Đưa con trỏ chuột về lại ô nhập
                txbMSSV.SelectAll(); // Bôi đen để nhập lại cho nhanh
                return;
            }

            // 3. Thực hiện Thêm
            try
            {
                string sql = "INSERT INTO SINHVIEN (MaSV, HoTen, NgaySinh, GioiTinh, QueQuan, NgayNhapHoc, MaLop) " +
                             "VALUES (@ma, @ten, @ngaysinh, @gt, @que, @nhaphoc, @malop)";

                // Bạn có thể copy code ExecuteSQL cũ hoặc viết rõ ra như này cho dễ debug
                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ma", txbMSSV.Text);
                cmd.Parameters.AddWithValue("@ten", txbTenSV.Text);
                cmd.Parameters.AddWithValue("@ngaysinh", dtpkNgaySinh.Value);
                cmd.Parameters.AddWithValue("@gt", radNam.Checked ? "Nam" : "Nữ");
                cmd.Parameters.AddWithValue("@que", txbQueQuan.Text);
                cmd.Parameters.AddWithValue("@nhaphoc", stpkNgayNhapHoc.Value);
                cmd.Parameters.AddWithValue("@malop", txbMaLop.Text);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Thêm sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDataSinhVien(); // Tải lại bảng
                ResetForm();        // Xóa trắng các ô nhập
            }
            catch (SqlException sqlEx)
            {
                // Bắt lỗi SQL cụ thể (Phòng hờ trường hợp khác)
                if (sqlEx.Number == 2627) // Mã lỗi trùng khóa chính trong SQL
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại (Lỗi từ CSDL)!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (sqlEx.Number == 547) // Mã lỗi khóa ngoại (Ví dụ nhập Mã Lớp không tồn tại)
                {
                    MessageBox.Show("Mã Lớp không tồn tại trong hệ thống!", "Lỗi khóa ngoại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi SQL: " + sqlEx.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txbMSSV.Text == "") return;
            try
            {
                ExecuteSQL("UPDATE SINHVIEN SET HoTen=@ten, NgaySinh=@ngaysinh, GioiTinh=@gt, QueQuan=@que, NgayNhapHoc=@nhaphoc, MaLop=@malop WHERE MaSV=@ma");
                MessageBox.Show("Sửa thành công!");
                LoadDataSinhVien(); ResetForm();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txbMSSV.Text == "") return;
            if (MessageBox.Show("Xóa sinh viên này?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    // Xóa điểm trước rồi xóa SV
                    using (SqlConnection c = new SqlConnection(strKetNoi))
                    {
                        c.Open();
                        new SqlCommand("DELETE FROM KETQUA WHERE MaSV='" + txbMSSV.Text + "'", c).ExecuteNonQuery();
                        new SqlCommand("DELETE FROM SINHVIEN WHERE MaSV='" + txbMSSV.Text + "'", c).ExecuteNonQuery();
                    }
                    MessageBox.Show("Xóa thành công!");
                    LoadDataSinhVien(); ResetForm();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }

        // Hàm chạy SQL chung cho gọn
        private void ExecuteSQL(string sql)
        {
            if (conn.State == ConnectionState.Closed) conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ma", txbMSSV.Text);
            cmd.Parameters.AddWithValue("@ten", txbTenSV.Text);
            cmd.Parameters.AddWithValue("@ngaysinh", dtpkNgaySinh.Value);
            cmd.Parameters.AddWithValue("@gt", radNam.Checked ? "Nam" : "Nữ");
            cmd.Parameters.AddWithValue("@que", txbQueQuan.Text);
            cmd.Parameters.AddWithValue("@nhaphoc", stpkNgayNhapHoc.Value);
            cmd.Parameters.AddWithValue("@malop", txbMaLop.Text);
            cmd.ExecuteNonQuery();
        }

        private void ResetForm()
        {
            txbID.Text = ""; txbMSSV.Text = ""; txbTenSV.Text = ""; txbQueQuan.Text = ""; txbMaLop.Text = ""; txbMaKhoa.Text = "";
            radNam.Checked = true; txbMSSV.Enabled = true;
        }

        private void btnTaiLai_Click(object sender, EventArgs e) { ResetForm(); LoadDataSinhVien(); }

        private void GanSuKien()
        {
            btnThem.Click += new EventHandler(btnThem_Click);
            btnSua.Click += new EventHandler(btnSua_Click);
            btnXoa.Click += new EventHandler(btnXoa_Click);
            btnTaiLai.Click += new EventHandler(btnTaiLai_Click);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            quảnLýLớpToolStripMenuItem.Click += new EventHandler(quảnLýLớpToolStripMenuItem_Click);
            quảnLýMônHọcToolStripMenuItem.Click += new EventHandler(quảnLýMônHọcToolStripMenuItem_Click);
            quảnLýKhoaToolStripMenuItem.Click += new EventHandler(quảnLýKhoaToolStripMenuItem_Click);
            quảnLýĐiểmToolStripMenuItem.Click += new EventHandler(quảnLýĐiểmToolStripMenuItem_Click);
            quảnLýTàiKhoảnToolStripMenuItem.Click += new EventHandler(quảnLýTàiKhoảnToolStripMenuItem_Click);
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void quảnLýTàiKhoảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fQuanLyTaiKhoan f = new fQuanLyTaiKhoan();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void quảnLýToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void quảnLýLớpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fQuanLyLop f = new fQuanLyLop();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void quảnLýMônHọcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fQuanLyMonHoc f = new fQuanLyMonHoc();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void quảnLýKhoaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fQuanLyKhoa f = new fQuanLyKhoa();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void quảnLýĐiểmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fQuanLyDiem f = new fQuanLyDiem();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void đăngXuấtToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có thật sự muốn đăng xuất?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                this.Close();
            }

        }

        private void thôngTinTàiKhoảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fThongTinTaiKhoan f = new fThongTinTaiKhoan(this.loginAccount);
            DialogResult result = f.ShowDialog();

            // Nếu bên kia trả về OK (tức là đã đổi mật khẩu thành công)
            if (result == DialogResult.OK)
            {
                this.Close(); // Đóng form Sinh Viên -> Sẽ tự quay về form Đăng Nhập
            }
        }
    }
}
