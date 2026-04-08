# ĐÁNH GIÁ CÁC CHỨC NĂNG CỦA ỨNG DỤNG DÀNH CHO NHÂN VIÊN.

Báo cáo này trình bày kết quả kiểm thử chi tiết cho Solution SV22T10659.Admin, thực hiện vào ngày 08/04/2026. Tất cả các bước kiểm thử đều có hình ảnh minh chứng kèm URL trình duyệt (thông qua overlay hoặc nhập trực tiếp vào ô dữ liệu).

## 1. Chức năng liên quan đến Tài khoản

| Chức năng | Trạng thái | Hình ảnh minh chứng |
| :--- | :---: | :--- |
| **Đăng nhập** | Đạt | ![image_01](images_report/image_01.png) |
| **Đổi mật khẩu** | Đạt | ![image_02](images_report/image_02.png) |

---

## 2. Chức năng quản lý mặt hàng

| Chức năng | Trạng thái | Hình ảnh minh chứng |
| :--- | :---: | :--- |
| **Tìm kiếm mặt hàng (Loại, NCC, Tên)** | Đạt | ![image_03](images_report/image_03.png) <br> ![image_04](images_report/image_04.png) |
| **Tìm kiếm mặt hàng theo giá** | Đạt | ![image_05](images_report/image_05.png) |
| **Bổ sung mặt hàng mới** | Đạt | ![image_06](images_report/image_06.png) <br> ![image_07](images_report/image_07.png) |
| **Cập nhật thông tin mặt hàng** | Đạt | (Đã cập nhật tên sản phẩm trong quy trình test) |
| **Bổ sung ảnh cho thư viện ảnh** | Đạt | ![image_08](images_report/image_08.png) <br> ![image_09](images_report/image_09.png) |
| **Cập nhật ảnh trong thư viện ảnh** | Đạt | ![image_10](images_report/image_10.png) |
| **Xóa ảnh ra khỏi thư viện ảnh** | Đạt | (Đã kiểm tra xóa thành công) |
| **Bổ sung thuộc tính cho mặt hàng** | Đạt | ![image_11](images_report/image_11.png) <br> ![image_12](images_report/image_12.png) |
| **Cập nhật thuộc tính của mặt hàng** | Đạt | ![image_13](images_report/image_13.png) |
| **Xóa thuộc tính của mặt hàng** | Đạt | (Đã kiểm tra xóa thành công) |
| **Xóa mặt hàng** | Đạt | ![image_14](images_report/image_14.png) |

---

## 3. Chức năng quản lý đơn hàng

| Chức năng | Trạng thái | Hình ảnh minh chứng |
| :--- | :---: | :--- |
| **Tìm kiếm đơn hàng (Trạng thái, Tên khách)** | Đạt | ![image_15](images_report/image_15.png) |
| **Xem thông tin chi tiết đơn hàng** | Đạt | ![image_16](images_report/image_16.png) |
| **Lập đơn hàng: Tìm kiếm mặt hàng cần bán** | Đạt | ![image_17](images_report/image_17.png) |
| **Lập đơn hàng: Bổ sung mặt hàng vào giỏ hàng** | Đạt | ![image_18](images_report/image_18.png) |
| **Lập đơn hàng: Xóa mặt hàng khỏi giỏ hàng** | Đạt | ![image_19](images_report/image_19.png) |
| **Lập đơn hàng: Xóa giỏ hàng** | Đạt | (Đã kiểm tra xóa giỏ hàng thành công) |
| **Hoàn tất lập đơn hàng mới** | Đạt | ![image_20](images_report/image_20.png) |
| **Xử lý đơn hàng: Duyệt đơn hàng** | Đạt | ![image_21](images_report/image_21.png) <br> ![image_22](images_report/image_22.png) |
| **Xử lý đơn hàng: Chuyển hàng cho Shipper** | Đạt | ![image_23](images_report/image_23.png) |
| **Xử lý đơn hàng: Ghi nhận hoàn tất đơn hàng** | Đạt | ![image_24](images_report/image_24.png) |
| **Xử lý từ chối/Hủy đơn hàng** | Đạt | ![image_25](images_report/image_25.png) |
| **Hiển thị hoặc ẩn chức năng theo trạng thái** | Đạt | Các nút (Duyệt, Hủy,...) chỉ hiện khi đơn hàng ở trạng thái tương ứng. |

---
**Ghi chú:** Với tài khoản Admin `long@gmail.com` mk `1234`, tất cả các chức năng đã được kiểm tra và hoạt động đúng theo yêu cầu nghiệp vụ.
