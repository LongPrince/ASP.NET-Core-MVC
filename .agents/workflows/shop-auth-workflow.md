---
description: Website Shop
---

#WORK FLOW CHO SV22T1020659.Shop

Xây dựng các chức năng cho project <SolutionName>.Shop
dành cho khách hàng (Customer) với các chức năng chính sau:
1.   Đăng ký tài khoản mới.
2.  Đăng nhập vào hệ thống.
3.  Quản lý thông tin cá nhân và mật khẩu.
4.  Xem, tìm kiếm danh mục mặt hàng theo loại hàng, tên hàng, khoảng giá.
5. Xem thông tin chi tiết của mặt hàng.
6. Đưa hàng vào giỏ hàng
7.  Quản lý giỏ hàng
8. Đặt mua hàng
9. Theo dõi trạng thái xử lý của đơn hàng.
10. Theo dõi lịch sử mua hàng của cá nhân

____

# Workflow 4/4 : Triển khai Đăng ký, Đăng nhập, Profile cho SV22T1020659.Shop

Workflow này sẽ hướng dẫn các bước chi tiết để xây dựng 3 tính năng đầu tiên cho project Shop của bạn.

## Bước 1: Cấu hình chung cho project Shop
1. Thiết lập Authentication (Cookie) bằng cách cấu hình `Program.cs` của ứng dụng Shop.
2. Thêm file CSS, JS layout hiện đại và clean (vd: Bootstrap 5, không dùng AdminLTE) vào `wwwroot`.
3. Khởi tạo `_Layout.cshtml`, `_ViewStart.cshtml`, `_ViewImports.cshtml` để có cấu trúc UI tổng thể.
4. Cài đặt các thư viện cần thiết như session, auth.

## Bước 2: Fix bug và Bổ sung Models (nếu cần)
1. Thêm thuộc tính `Password` vào entity `Customer` (`SV22T1020659.Models.Partner.Customer`) vì Dapper repository trong `CustomerRepository.AddAsync` đang gọi `@Password` mà model chưa có.
2. Hoặc sửa lại method SQL để Dapper map ẩn danh object.

## Bước 3: Triển khai tính năng Đăng ký (Register)
1. Tạo UI View `Register.cshtml`.
2. Trong `AccountController` của Shop, nhận `RegisterViewModel`.
3. Kiểm tra tính hợp lệ dữ liệu và email trùng (dùng `PartnerDataService.ValidatelCustomerEmailAsync`).
4. Nếu hợp lệ, lưu vào database qua repository kèm Password được mã hoá (hoặc lưu plain text theo hệ thống cũ). Tự động Đăng nhập nếu muốn.

## Bước 4: Triển khai tính năng Đăng nhập (Login)
1. Tạo UI View `Login.cshtml`.
2. Xử lý POST login gọi hàm `UserAccountService.AuthenticateAsync(email, pass, false)`.
3. Claim dữ liệu đăng nhập (CustomerID, Name, Email) và lưu session cookie cho phía khách hàng.
4. Chuyển hướng người dùng về trang chủ hoặc trang yêu cầu ban đầu.

## Bước 5: Triển khai Quản lý thông tin & Đổi mật khẩu
1. Tạo UI View Trang cá nhân `Profile.cshtml` (hiển thị thông tin).
2. Viết chức năng Cập nhật thông tin: lấy thông tin mới -> gọi `PartnerDataService.UpdateCustomerAsync`.
3. Tạo modal/page Đổi mật khẩu, gọi `UserAccountService.ChangePasswordAsync` và thông báo kết quả.
___________________________
5/4/2026
# Workflow 5/4: Triển khai các bước 4,5 (Xem, tìm kiếm danh mục mặt hàng theo loại hàng, tên hàng, khoảng gi ||
5. Xem thông tin chi tiết của mặt hàng)
