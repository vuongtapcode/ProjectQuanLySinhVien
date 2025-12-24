using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ProjectQuanLySinhVien.DTO;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fSinhVien : Form
    {
        private Account loginAccount;
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True;TrustServerCertificate=True";
        SqlConnection conn = null;

        public fSinhVien(Account acc)
        {
            InitializeComponent();
            this.loginAccount = acc;
            this.Text = "Quản Lý Sinh Viên - Xin chào: " + acc.DisplayName;
            LoadComboBoxKhoa();
            LoadDataSinhVien();
            PhanQuyen();
        }
        void PhanQuyen()
        {
            if (BienToanCuc.LoaiTaiKhoan == 0)
            {
           
                btnThem.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                
                txbMSSV.ReadOnly = true;
                txbTenSV.ReadOnly = true;
                txbQueQuan.ReadOnly = true;

                dtpkNgaySinh.Enabled = false;
                stpkNgayNhapHoc.Enabled = false;
                radNam.Enabled = false;
                radNu.Enabled = false;

                cbKhoa.Enabled = false;
                cbLop.Enabled = false;

                this.Text = "Quản Lý Sinh Viên - Xin chào: " + loginAccount.DisplayName + " (Quyền: Nhân viên - Chỉ xem)";
            }
            else
            {
                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;
                txbMSSV.ReadOnly = false;
                txbTenSV.ReadOnly = false;
                txbQueQuan.ReadOnly = false;

                dtpkNgaySinh.Enabled = true;
                stpkNgayNhapHoc.Enabled = true;
                radNam.Enabled = true;
                radNu.Enabled = true;
                cbKhoa.Enabled = true;
                cbLop.Enabled = true;

                this.Text = "Quản Lý Sinh Viên - Xin chào: " + loginAccount.DisplayName + " (Quyền: Admin)";
            }
        }
        private void btnTim_Click(object sender, EventArgs e)
        {
            string tuKhoa = txbTimKiem.Text.Trim();

            if (string.IsNullOrEmpty(tuKhoa))
            {
                LoadDataSinhVien();
                return;
            }

            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();
                string sql = @"
                SELECT SV.MaSV, SV.HoTen, SV.NgaySinh, SV.GioiTinh, SV.QueQuan, SV.NgayNhapHoc, SV.MaLop, L.MaKhoa
                FROM SINHVIEN SV LEFT JOIN LOP L ON SV.MaLop = L.MaLop
                WHERE SV.HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE N'%' + @kw + '%' OR SV.MaSV LIKE '%' + @kw + '%'"; 
                

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@kw", tuKhoa);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++) dt.Rows[i]["STT"] = i + 1;

                dataGridView1.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào!", "Kết quả");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }
        private void txbTimKiem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnTim.PerformClick();            
            }
        }
        private void FillComboBox(string sql, ComboBox cb, string hienThi, string giaTri)
        {
            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                cb.DataSource = dt;
                cb.DisplayMember = hienThi; 
                cb.ValueMember = giaTri;    
                cb.SelectedIndex = -1;      
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải ComboBox: " + ex.Message);
            }
        }
        private void LoadComboBoxKhoa()
        {
            FillComboBox("SELECT MaKhoa, TenKhoa FROM KHOA", cbKhoa, "TenKhoa", "MaKhoa");
        }
        private void cbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbKhoa.SelectedValue != null)
            {
                if (cbKhoa.SelectedValue is DataRowView) return;

                string maKhoa = cbKhoa.SelectedValue.ToString();
                string sql = "SELECT MaLop, TenLop FROM LOP WHERE MaKhoa = '" + maKhoa + "'";
                FillComboBox(sql, cbLop, "TenLop", "MaLop");
            }
        }
        private void LoadDataSinhVien()
        {
            try
            {
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();

                string sql = @"
                    SELECT 
                        SV.MaSV, SV.HoTen, SV.NgaySinh, SV.GioiTinh, SV.QueQuan, SV.NgayNhapHoc,
                        SV.MaLop, L.MaKhoa
                    FROM SINHVIEN SV
                    LEFT JOIN LOP L ON SV.MaLop = L.MaLop";

                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dt.Columns.Add("STT", typeof(int));
                for (int i = 0; i < dt.Rows.Count; i++) dt.Rows[i]["STT"] = i + 1;

                dataGridView1.AutoGenerateColumns = false;
                if (dataGridView1.Columns["Column1"] != null) dataGridView1.Columns["Column1"].DataPropertyName = "STT";
                if (dataGridView1.Columns["Column2"] != null) dataGridView1.Columns["Column2"].DataPropertyName = "MaSV";
                if (dataGridView1.Columns["Column3"] != null) dataGridView1.Columns["Column3"].DataPropertyName = "HoTen";
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

                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }
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
                string maKhoaTrongBang = row.Cells["Column9"].Value?.ToString();
                string maLopTrongBang = row.Cells["Column8"].Value?.ToString();
                cbKhoa.SelectedValue = maKhoaTrongBang;
                if (cbLop.Items.Count > 0)
                {
                    cbLop.SelectedValue = maLopTrongBang;
                }

                txbMSSV.Enabled = false; 
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txbMSSV.Text) || string.IsNullOrWhiteSpace(txbTenSV.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã và Tên Sinh Viên!", "Cảnh báo"); return;
            }
            if (string.IsNullOrWhiteSpace(txbQueQuan.Text))
            {
                MessageBox.Show("Vui lòng nhập Quê Quán!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txbQueQuan.Focus(); return;
            }
            if (cbKhoa.SelectedIndex == -1 || cbLop.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Khoa và Lớp!", "Cảnh báo"); return;
            }

            if (KiemTraTrungMSSV(txbMSSV.Text))
            {
                MessageBox.Show("Mã SV đã tồn tại!", "Lỗi"); return;
            }

            try
            {
                string sql = "INSERT INTO SINHVIEN (MaSV, HoTen, NgaySinh, GioiTinh, QueQuan, NgayNhapHoc, MaLop) " +
                             "VALUES (@ma, @ten, @ngaysinh, @gt, @que, @nhaphoc, @malop)";

                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ma", txbMSSV.Text);
                cmd.Parameters.AddWithValue("@ten", txbTenSV.Text);
                cmd.Parameters.AddWithValue("@ngaysinh", dtpkNgaySinh.Value);
                cmd.Parameters.AddWithValue("@gt", radNam.Checked ? "Nam" : "Nữ");
                cmd.Parameters.AddWithValue("@que", txbQueQuan.Text);
                cmd.Parameters.AddWithValue("@nhaphoc", stpkNgayNhapHoc.Value);
                cmd.Parameters.AddWithValue("@malop", cbLop.SelectedValue.ToString());

                cmd.ExecuteNonQuery();
                MessageBox.Show("Thêm thành công!");
                LoadDataSinhVien(); ResetForm();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txbMSSV.Text == "") return;
            if (cbLop.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Lớp mới cần chuyển đến (nếu có)!", "Thông báo"); return;
            }

            try
            {
                string sql = "UPDATE SINHVIEN SET HoTen=@ten, NgaySinh=@ngaysinh, GioiTinh=@gt, QueQuan=@que, NgayNhapHoc=@nhaphoc, MaLop=@malop WHERE MaSV=@ma";

                if (conn.State == ConnectionState.Closed) conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@ma", txbMSSV.Text);
                cmd.Parameters.AddWithValue("@ten", txbTenSV.Text);
                cmd.Parameters.AddWithValue("@ngaysinh", dtpkNgaySinh.Value);
                cmd.Parameters.AddWithValue("@gt", radNam.Checked ? "Nam" : "Nữ");
                cmd.Parameters.AddWithValue("@que", txbQueQuan.Text);
                cmd.Parameters.AddWithValue("@nhaphoc", stpkNgayNhapHoc.Value);
                cmd.Parameters.AddWithValue("@malop", cbLop.SelectedValue.ToString());

                cmd.ExecuteNonQuery();
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
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    new SqlCommand("DELETE FROM KETQUA WHERE MaSV='" + txbMSSV.Text + "'", conn).ExecuteNonQuery();
                    new SqlCommand("DELETE FROM SINHVIEN WHERE MaSV='" + txbMSSV.Text + "'", conn).ExecuteNonQuery();

                    MessageBox.Show("Xóa thành công!");
                    LoadDataSinhVien(); ResetForm();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
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
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            catch { return true; }
        }

        private void ResetForm()
        {
            txbMSSV.Text = ""; txbTenSV.Text = ""; txbQueQuan.Text = "";
            radNam.Checked = true; txbMSSV.Enabled = true;
            cbKhoa.SelectedIndex = -1;
            cbLop.DataSource = null; 
        }

        private void btnTaiLai_Click(object sender, EventArgs e) { 
            ResetForm(); 
            LoadDataSinhVien(); 
        }
        private void quảnLýTàiKhoảnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BienToanCuc.LoaiTaiKhoan == 0)
            {
                MessageBox.Show("Bạn không có quyền truy cập vào quản lý tài khoản!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; 
            }
            fQuanLyTaiKhoan f = new fQuanLyTaiKhoan();
            this.Hide();
            f.ShowDialog();
            this.Show();
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
            if (result == DialogResult.OK)
            {
                this.Close();
            }
        }

        
    }
}
