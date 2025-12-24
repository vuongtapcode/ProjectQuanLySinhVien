# PROJECT QUẢN LÝ SINH VIÊN - C# WINFORMS

## 1. Giới thiệu
Đây là ứng dụng **Quản Lý Sinh Viên** được xây dựng bằng **C# Windows Forms**, phục vụ cho mục đích học tập môn **Lập trình trực quan**.

Chương trình được thiết kế với giao diện trực quan, kết nối trực tiếp với cơ sở dữ liệu SQL Server để quản lý hồ sơ sinh viên, lớp học, môn học và điểm số, đồng thời tích hợp cơ chế phân quyền bảo mật.

## 2. Công nghệ sử dụng
* **Ngôn ngữ lập trình:** C#
* **Nền tảng:** .NET Framework (Windows Forms)
* **Cơ sở dữ liệu:** Microsoft SQL Server
* **Công cụ phát triển:** Visual Studio 2019/2022
* **Ngôn ngữ truy vấn:** T-SQL

## 3. Chức năng chính

**Hệ thống Đăng nhập & Phân quyền**
* Giao diện đăng nhập hiện đại, hỗ trợ ghi nhớ tài khoản.
* **Admin:** Có toàn quyền hệ thống (Thêm, Sửa, Xóa dữ liệu, Quản lý tài khoản).
* **Nhân viên (Staff):** Quyền hạn chế, chỉ được phép Xem và Tìm kiếm dữ liệu.

**Quản lý Sinh viên & Đào tạo**
* **Hồ sơ sinh viên:** Quản lý mã số, họ tên, ngày sinh, quê quán.
* **Cơ cấu tổ chức:** Quản lý danh sách Khoa, Lớp học.
* **Học phần:** Quản lý danh sách Môn học và số tín chỉ.

**Quản lý Điểm số & Tìm kiếm**
* **Nhập điểm:** Cập nhật điểm thi kết thúc môn cho sinh viên.
* **Tìm kiếm thông minh:** Hỗ trợ tìm kiếm theo Mã hoặc Tên (hỗ trợ tiếng Việt có dấu và không dấu).
* **Lọc dữ liệu:** Hệ thống tự động lọc danh sách sinh viên theo Lớp và Môn học khi nhập điểm để đảm bảo tính chính xác.

## 4. Tài khoản Demo

Các tài khoản được khởi tạo sẵn trong cơ sở dữ liệu (Bảng `ACCOUNT`):

| Vai trò | Tên đăng nhập | Mật khẩu | Quyền hạn |
|---|---|---|---|
| **Quản trị viên** | admin | admin123 | Toàn quyền (Full Access) |
| **Nhân viên** | staff | staff123 | Chỉ xem và tìm kiếm (Read Only) |

## 5. Hướng dẫn cài đặt

Để ứng dụng hoạt động, vui lòng thực hiện các bước sau:

**Bước 1: Cấu hình Cơ sở dữ liệu**
1.  Mở SQL Server Management Studio.
2.  Chạy file script `SQLQuanLySinhVien.sql` (trong thư mục SQLQuanLySinhVien) để tạo bảng và dữ liệu mẫu.

**Bước 2: Cấu hình kết nối**
1.  Mở project bằng Visual Studio.
2.  Tìm biến chuỗi kết nối `strKetNoi` trong các file code (Form).
3.  Cập nhật tham số `Data Source` thành tên máy chủ SQL của bạn (Ví dụ: `Data Source=DESKTOP-XXXX\SQLEXPRESS`).

**Bước 3: Chạy ứng dụng**
* Nhấn **Start** hoặc **F5** trong Visual Studio để khởi chạy chương trình.

## 6. Nhóm tác giả
Đồ án được thực hiện bởi nhóm sinh viên:

| STT | Họ và Tên | MSSV |
|:---:|---|:---:|
| 1 | **Nguyễn Đại Vương** | 24522050 |
| 2 | **Nguyễn Viết Vinh** | 24522026 |
| 3 | **Trịnh Quang Giang** | 24520425 |

---
*Lưu ý: Project phục vụ mục đích học tập và nghiên cứu.*