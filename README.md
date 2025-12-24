# 📚 Project Quản Lý Sinh Viên – C# WinForms

## 📌 Giới thiệu
Đây là ứng dụng **Quản Lý Sinh Viên** được xây dựng bằng **C# WinForms**, phục vụ cho mục đích học tập môn **Lập trình trực quan**.  
Chương trình cung cấp giao diện trực quan để quản lý sinh viên, lớp học và người dùng, đồng thời có phân quyền đăng nhập rõ ràng.

---

## 🛠 Công nghệ sử dụng
- Ngôn ngữ: **C#**
- Nền tảng: **.NET WinForms**
- IDE: **Visual Studio**
- Cơ sở dữ liệu: **SQL Server**
- Ngôn ngữ truy vấn: **T-SQL**

---

## ✨ Chức năng chính

### 🔐 1. Đăng nhập & phân quyền
- Đăng nhập hệ thống bằng tài khoản
- Phân quyền người dùng:
  - **Admin**: toàn quyền hệ thống
  - **Staff**: quyền hạn chế
- Kiểm tra thông tin đăng nhập hợp lệ

### 👨‍🎓 2. Quản lý sinh viên
- Hiển thị danh sách sinh viên
- Thêm sinh viên mới
- Cập nhật thông tin sinh viên
- Xóa sinh viên
- Tìm kiếm sinh viên theo mã, tên hoặc lớp

### 🏫 3. Quản lý lớp học
- Thêm, sửa, xóa lớp học
- Liên kết sinh viên với lớp
- Hiển thị danh sách sinh viên theo lớp

### 📊 4. Giao diện trực quan
- Sử dụng WinForms
- DataGridView hiển thị dữ liệu
- Thao tác thông qua Button, TextBox, ComboBox
- Giao diện dễ sử dụng, phù hợp cho người dùng phổ thông

---

## 🔐 Tài khoản đăng nhập (Demo)

Các tài khoản demo được **khởi tạo sẵn trong cơ sở dữ liệu**:

| Vai trò | Tên đăng nhập | Mật khẩu |
|------|-------------|---------|
| Admin | admin | admin123 |
| Staff | staff | staff123 |

📌 Khi cần reset dữ liệu, chỉ cần **chạy lại file SQL** trong thư mục `SQLQuanLySinhVien`.

---

## 📥 Hướng dẫn tải project

### Cách 1: Download trực tiếp (khuyến nghị)
1. Truy cập repository GitHub của project
2. Nhấn nút **Code** → **Download ZIP**
3. Giải nén file vừa tải
4. Mở file `ProjectQuanLySinhVien.sln` bằng **Visual Studio**

### Cách 2: Clone bằng Git
```bash
git clone https://github.com/vuongtapcode/ProjectQuanLySinhVien.git


---

## ⚠️ Lưu ý
- Không commit các thư mục:
  - `.vs/`
  - `bin/`
  - `obj/`
- Đảm bảo máy đã cài đặt **SQL Server**
- Project phục vụ mục đích học tập, không dùng cho môi trường thực tế

---

## 👨‍💻 Tác giả
1. **Nguyễn Đại Vương** – MSSV: **24522050**  
2. **Nguyễn Viết Vinh** – MSSV: **24522026**  
3. **Trịnh Quang Giang** – MSSV: **24520425**


