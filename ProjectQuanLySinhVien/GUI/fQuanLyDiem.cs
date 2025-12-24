using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ProjectQuanLySinhVien.GUI
{
    public partial class fQuanLyDiem : Form
    {
        string strKetNoi = @"Data Source=DESKTOP-E2VL8VG\SQLEXPRESS;Initial Catalog=QLSV_DB;Integrated Security=True";
        SqlConnection conn = null;

        public fQuanLyDiem()
        {
            InitializeComponent();
            LoadDataComboBox();
            PhanQuyen();
        }
        void PhanQuyen()
        {
            if (BienToanCuc.LoaiTaiKhoan == 0)
            {
                btnThem.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                txtDiem.ReadOnly = true;
                txtMaSV.ReadOnly = true;
                txtTenSV.ReadOnly = true;
                this.Text = "Quản Lý Điểm (Quyền: Nhân viên - Chỉ xem)";
            }
            else
            {
                btnThem.Enabled = true;
                btnSua.Enabled = true;
                btnXoa.Enabled = true;

                txtDiem.ReadOnly = false;
                this.Text = "Quản Lý Điểm (Quyền: Admin)";
            }
        }
        private object GetComboBoxValue(ComboBox cb)
        {
            if (cb == null) return null;
            var sv = cb.SelectedValue;
            if (sv == null) return null;
            if (sv is DataRowView drv)
            {
                try
                {
                    if (!string.IsNullOrEmpty(cb.ValueMember) && drv.Row.Table.Columns.Contains(cb.ValueMember))
                        return drv[cb.ValueMember];
                    return drv.Row.ItemArray.Length > 0 ? drv.Row.ItemArray[0] : drv.ToString();
                }
                catch
                {
                    return drv.ToString();
                }
            }
            return sv;
        }
        private void LoadDataComboBox()
        {
            try
            {
                conn = new SqlConnection(strKetNoi);
                conn.Open();
                SqlDataAdapter daLop = new SqlDataAdapter("SELECT MaLop, TenLop FROM LOP", conn);
                DataTable dtLop = new DataTable();
                daLop.Fill(dtLop);
                cboLop.DataSource = dtLop;
                cboLop.DisplayMember = "TenLop";
                cboLop.ValueMember = "MaLop";
                SqlDataAdapter daMon = new SqlDataAdapter("SELECT MaMH, TenMH FROM MONHOC", conn);
                DataTable dtMon = new DataTable();
                daMon.Fill(dtMon);
                cboMonHoc.DataSource = dtMon;
                cboMonHoc.DisplayMember = "TenMH";
                cboMonHoc.ValueMember = "MaMH";
                cboLop.SelectedIndex = -1;
                cboMonHoc.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show("Lỗi kết nối: " + ex.Message); }
        }
        private void LoadDiemSinhVien()
        {
            if (GetComboBoxValue(cboLop) == null || GetComboBoxValue(cboMonHoc) == null) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                string sql = @"
                    SELECT 
                        sv.MaSV, 
                        sv.HoTen, 
                        kq.DiemSo 
                    FROM SINHVIEN sv
                    LEFT JOIN KETQUA kq ON sv.MaSV = kq.MaSV AND kq.MaMH = @maMH
                    WHERE sv.MaLop = @maLop";

                SqlCommand cmd = new SqlCommand(sql, conn);
                var maMH = GetComboBoxValue(cboMonHoc);
                var maLop = GetComboBoxValue(cboLop);
                cmd.Parameters.AddWithValue("@maMH", maMH ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@maLop", maLop ?? (object)DBNull.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void cboLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDiemSinhVien();
        }

        private void cboMonHoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDiemSinhVien();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtMaSV.Text = row.Cells["colMaSV"].Value?.ToString() ?? "";
                txtTenSV.Text = row.Cells["colTenSV"].Value?.ToString() ?? "";
                txtDiem.Text = row.Cells["colDiem"].Value?.ToString() ?? "";
                txtDiem.Focus(); 
            }
        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                if (KiemTraDiemTonTai())
                {
                    MessageBox.Show("Sinh viên này đã có điểm môn này rồi! Vui lòng dùng nút SỬA.");
                    return;
                }
                string sql = "INSERT INTO KETQUA (MaSV, MaMH, DiemSo) VALUES (@sv, @mh, @d)";
                ExecuteSQL(sql);
                MessageBox.Show("Nhập điểm thành công!");
                LoadDiemSinhVien();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                if (!KiemTraDiemTonTai())
                {
                    MessageBox.Show("Sinh viên này chưa có điểm! Vui lòng dùng nút THÊM.");
                    return;
                }
                string sql = "UPDATE KETQUA SET DiemSo=@d WHERE MaSV=@sv AND MaMH=@mh";
                ExecuteSQL(sql);
                MessageBox.Show("Sửa điểm thành công!");
                LoadDiemSinhVien();
            }
            catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txtMaSV.Text == "") return;

            if (MessageBox.Show("Bạn muốn xóa điểm môn này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string sql = "DELETE FROM KETQUA WHERE MaSV=@sv AND MaMH=@mh";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@sv", txtMaSV.Text);
                    var maMH = GetComboBoxValue(cboMonHoc);
                    cmd.Parameters.AddWithValue("@mh", maMH ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Xóa điểm thành công!");
                    LoadDiemSinhVien();
                    txtDiem.Text = "";
                }
                catch (Exception ex) { MessageBox.Show("Lỗi xóa: " + ex.Message); }
            }
        }
        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadDiemSinhVien();
            txtMaSV.Text = "";
            txtTenSV.Text = "";
            txtDiem.Text = "";
        }
        private bool CheckInput()
        {
            if (txtMaSV.Text == "" || GetComboBoxValue(cboMonHoc) == null)
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
        private bool KiemTraDiemTonTai()
        {
            string check = "SELECT COUNT(*) FROM KETQUA WHERE MaSV=@sv AND MaMH=@mh";
            SqlCommand cmdCheck = new SqlCommand(check, conn);
            cmdCheck.Parameters.AddWithValue("@sv", txtMaSV.Text);
            var maMH = GetComboBoxValue(cboMonHoc);
            cmdCheck.Parameters.AddWithValue("@mh", maMH ?? (object)DBNull.Value);
            int count = (int)cmdCheck.ExecuteScalar();
            return count > 0;
        }
        private void ExecuteSQL(string sql)
        {
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sv", txtMaSV.Text);
            var maMH = GetComboBoxValue(cboMonHoc);
            cmd.Parameters.AddWithValue("@mh", maMH ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@d", float.Parse(txtDiem.Text));
            cmd.ExecuteNonQuery();
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string tuKhoa = txbTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(tuKhoa))
            {
                LoadDiemSinhVien(); 
                return;
            }
            if (cboLop.SelectedIndex == -1 || cboMonHoc.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Lớp và Môn học trước khi tìm kiếm!", "Thông báo");
                return;
            }

            try
            {
                
                if (conn == null) conn = new SqlConnection(strKetNoi);
                if (conn.State == ConnectionState.Closed) conn.Open();

                
                string sql = @"
                SELECT sv.MaSV, sv.HoTen, kq.DiemSo
                FROM SINHVIEN sv LEFT JOIN KETQUA kq ON sv.MaSV = kq.MaSV AND kq.MaMH = @mh
                WHERE sv.MaLop = @lop AND (sv.HoTen COLLATE SQL_Latin1_General_CP1_CI_AI LIKE N'%' + @kw + '%' OR sv.MaSV LIKE '%' + @kw + '%')";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    object vLop = cboLop.SelectedValue;
                    if (vLop is DataRowView drvLop) vLop = drvLop["MaLop"];
                    cmd.Parameters.AddWithValue("@lop", vLop);

                    object vMon = cboMonHoc.SelectedValue;
                    if (vMon is DataRowView drvMon) vMon = drvMon["MaMH"];
                    cmd.Parameters.AddWithValue("@mh", vMon);

                    cmd.Parameters.AddWithValue("@kw", tuKhoa);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    
                    dataGridView1.AutoGenerateColumns = false; 

                    dataGridView1.DataSource = dt;

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy sinh viên nào trong danh sách này!", "Thông báo");
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
    }
}