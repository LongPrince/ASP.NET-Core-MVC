# Báo cáo hoàn thành: Giai đoạn 1 Shop App (Đăng ký, Đăng nhập, Hồ sơ)

## 🎯 Mục tiêu hoàn thành
Đã triển khai thành công 3 chức năng đầu tiên của hệ thống `SV22T1020659.Shop` dành cho Khách hàng:
1. Đăng ký tài khoản mới.
2. Đăng nhập vào hệ thống.
3. Quản lý thông tin cá nhân và mật khẩu.

## 🛠️ Các thay đổi chính đã thực hiện
**Giao diện (Frontend)**
- Cài đặt `_Layout.cshtml` mới sử dụng **Bootstrap 5** thay cho cấu trúc AdminLTE, tạo luồng gió mới, thanh thoát, phù hợp với trải nghiệm mua sắm E-commerce.
- Cấu hình file `wwwroot/css/styles.css` cung cấp các Utility Class, Box Shadow hiện đại, bo góc mềm mại.

**Bảo mật & Database (Backend)**
- Cấu hình độc lập Session và Cookie Authentication (với scope `SV22T1020659.CustomerCookie`), đảm bảo sự cách ly phiên với project Admin.
- Vá lỗi tham chiếu cho SQL Dapper: Đã khai báo thuộc tính `Password` vào model base `Customer` (`SV22T1020659.Models.Partner.Customer`) để khớp câu lệnh `INSERT` của `CustomerRepository.cs` (bước này đã giải quyết một lỗi nghẽn rất nghiêm trọng khiến Dapper không thực thi được).

**Controllers & Views (Tính năng)**
- `AccountController.cs` với 4 Endpoints cốt lõi: `Login`, `Register`, `Profile`, `ChangePassword`, `Logout`.
- Kiểm duyệt đầu vào (Data Validation) bằng logic ViewModels (`RegisterViewModel`, `LoginViewModel`, `ProfileViewModel`, `ChangePasswordViewModel`). Đảm bảo bắt lỗi các trường bắt buộc, email trùng lắp qua `PartnerDataService.ValidatelCustomerEmailAsync`.
- Tự động Claims phân quyền role `"Customer"` và auto sign-in sau khi Đăng ký thành công.
- 4 View tương ứng (`Register.cshtml`, `Login.cshtml`, `Profile.cshtml`, `ChangePassword.cshtml`) được thiết kế thẩm mỹ cao, dạng Card-Auth hiện đại (không xài AdminLTE).

## 🧪 Kết quả Validation
> [!NOTE]
> Project Shop đang chạy một cách mượt mà và độc lập tại Port `5150`. Hiện tại anh đã có thể đăng nhập thử, chỉnh sửa profile và đổi password thông qua tài khoản khách hàng mới tạo.
- Cấu hình tự binding với CSDL `LiteCommerceDB` hoàn chỉnh trong mục config.
- Việc đăng xuất khỏi Shop cũng hoàn toàn **không** làm mất session của tài khoản Admin ở Port `5175`.
