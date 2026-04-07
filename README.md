# 📦 Tài Liệu Kiến Trúc Dự Án

> **Công nghệ:** ASP.NET Core MVC · C# · SQL Server · Razor Views  
> **Mô hình kiến trúc:** N-Layer (Multi-tier / Phân tầng)

---

## 📋 Mục Lục

1. [Tổng Quan Dự Án](#1-tổng-quan-dự-án)
2. [Sơ Đồ Kiến Trúc Tổng Thể](#2-sơ-đồ-kiến-trúc-tổng-thể)
3. [Mô Tả Chi Tiết Từng Tầng](#3-mô-tả-chi-tiết-từng-tầng)
   - [Tầng 1 – Models](#tầng-1--models-sv22t1020659models)
   - [Tầng 2 – DataLayers](#tầng-2--datalayers-sv22t1020659datalayers)
   - [Tầng 3 – BusinessLayers](#tầng-3--businesslayers-sv22t1020659businesslayers)
   - [Tầng 4 – Admin (Presentation)](#tầng-4--admin-sv22t1020659admin)
   - [Tầng 5 – Shop (Presentation)](#tầng-5--shop-sv22t1020659shop)
4. [Quy Trình Xử Lý Dữ Liệu](#4-quy-trình-xử-lý-dữ-liệu)
5. [Sơ Đồ Mối Liên Hệ Giữa Các Tầng](#5-sơ-đồ-mối-liên-hệ-giữa-các-tầng)
6. [Ví Dụ Minh Họa](#6-ví-dụ-minh-họa)
7. [Tóm Tắt Chức Năng Theo Module](#7-tóm-tắt-chức-năng-theo-module)
8. [Hướng Dẫn Cài Đặt & Chạy Dự Án](#8-hướng-dẫn-cài-đặt--chạy-dự-án)

---

## 1. Tổng Quan Dự Án

**LiteCommerce** là một hệ thống thương mại điện tử thu nhỏ (mini e-commerce) được xây dựng theo mô hình **N-Layer Architecture** (Kiến trúc đa tầng). Dự án gồm 5 project trong cùng một Solution:

| Project | Loại | Vai trò |
|---|---|---|
| `SV22T1020659.Models` | Class Library | Định nghĩa các lớp thực thể (Entity/Domain Model) |
| `SV22T1020659.DataLayers` | Class Library | Truy cập và thao tác với cơ sở dữ liệu |
| `SV22T1020659.BusinessLayers` | Class Library | Xử lý nghiệp vụ, logic kinh doanh |
| `SV22T1020659.Admin` | ASP.NET Core MVC | Giao diện quản trị dành cho nhân viên/admin |
| `SV22T1020659.Shop` | ASP.NET Core MVC | Giao diện cửa hàng dành cho khách hàng |

---

## 2. Sơ Đồ Kiến Trúc Tổng Thể

```
┌─────────────────────────────────────────────────────────────────┐
│                     PRESENTATION LAYER                          │
│                                                                 │
│   ┌──────────────────────┐    ┌──────────────────────────────┐  │
│   │  SV22T1020659.Admin  │    │    SV22T1020659.Shop         │  │
│   │  (Quản trị hệ thống) │    │    (Cửa hàng cho khách)      │  │
│   └──────────┬───────────┘    └──────────────┬───────────────┘  │
└──────────────┼──────────────────────────────┼───────────────────┘
               │  gọi hàm                     │  gọi hàm
               ▼                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                    BUSINESS LAYER                               │
│              SV22T1020659.BusinessLayers                        │
│  CatalogDataService | PartnerDataService | SalesDataService     │
│  HRDataService | DictionaryDataService | UserAccountService     │
└──────────────────────────────┬──────────────────────────────────┘
                               │  gọi hàm qua Interface
                               ▼
┌─────────────────────────────────────────────────────────────────┐
│                     DATA LAYER                                  │
│              SV22T1020659.DataLayers                            │
│                                                                 │
│   Interfaces/                  SQLServer/                       │
│   IGenericRepository<T>   →    CategoryRepository               │
│   IProductRepository      →    ProductRepository                │
│   IOrderRepository        →    OrderRepository                  │
│   IEmployeeRepository     →    EmployeeRepository               │
│   ICustomerRepository     →    CustomerRepository               │
│   IUserAccountRepository  →    EmployeeAccountRepository        │
│                           →    CustomerAccountRepository        │
└──────────────────────────────┬──────────────────────────────────┘
                               │  sử dụng
                               ▼
┌─────────────────────────────────────────────────────────────────┐
│                      MODELS LAYER                               │
│               SV22T1020659.Models                               │
│                                                                 │
│   Catalog/   HR/       Partner/   Sales/    Common/  Security/  │
│   Product    Employee  Customer   Order     PagedResult         │
│   Category             Supplier   CartItem  PaginationInput     │
│   ProductPhoto         Shipper    OrderDetail                   │
│   ProductAttribute                OrderViewInfo                 │
└──────────────────────────────┬──────────────────────────────────┘
                               │  đọc/ghi
                               ▼
                    ┌──────────────────┐
                    │  SQL Server DB   │
                    │  LiteCommerceDB  │
                    └──────────────────┘
```

---

## 3. Mô Tả Chi Tiết Từng Tầng

---

### Tầng 1 – Models (`SV22T1020659.Models`)

> **Ý nghĩa:** Tầng định nghĩa các **lớp thực thể** (class) ánh xạ với bảng dữ liệu trong SQL Server và các lớp hỗ trợ phân trang, tìm kiếm. Tầng này **không có logic xử lý**, chỉ chứa dữ liệu.

#### Cấu trúc thư mục và file

| Thư mục | File | Mô tả |
|---|---|---|
| `Catalog/` | `Product.cs` | Lớp mặt hàng: mã, tên, giá, ảnh, danh mục, nhà cung cấp |
| | `Category.cs` | Lớp loại hàng (danh mục) |
| | `ProductAttribute.cs` | Lớp thuộc tính mở rộng của mặt hàng (VD: màu sắc, kích cỡ) |
| | `ProductPhoto.cs` | Lớp ảnh của mặt hàng |
| | `ProductSearchInput.cs` | Lớp tham số tìm kiếm mặt hàng (từ khóa, loại, nhà CC, giá) |
| `HR/` | `Employee.cs` | Lớp nhân viên: mã, họ tên, email, mật khẩu, vai trò |
| `Partner/` | `Customer.cs` | Lớp khách hàng: mã, tên, email, địa chỉ, tỉnh |
| | `Supplier.cs` | Lớp nhà cung cấp |
| | `Shipper.cs` | Lớp người giao hàng |
| `Sales/` | `Order.cs` | Lớp đơn hàng chính |
| | `OrderDetail.cs` | Lớp chi tiết dòng hàng trong đơn |
| | `OrderViewInfo.cs` | Lớp hiển thị thông tin đơn hàng (join nhiều bảng) |
| | `CartItem.cs` | Lớp mặt hàng trong giỏ hàng (lưu session) |
| | `OrderStatusEnum.cs` | Enum trạng thái đơn hàng: Khởi tạo → Duyệt → Giao → Hoàn tất |
| | `OrderStatusExtensions.cs` | Extension method chuyển trạng thái → tên hiển thị |
| `Common/` | `PagedResult<T>` | Lớp generic chứa kết quả phân trang |
| | `PaginationSearchInput.cs` | Lớp tham số phân trang cơ bản (trang, số dòng, từ khóa) |
| | `PageItem.cs` | Lớp đại diện một trang trong danh sách phân trang |
| `Security/` | `UserAccount.cs` | Lớp tài khoản người dùng sau khi xác thực (Claims) |
| `DataDictionary/` | `Province.cs` | Lớp danh mục tỉnh/thành phố |

---

### Tầng 2 – DataLayers (`SV22T1020659.DataLayers`)

> **Ý nghĩa:** Tầng **truy cập dữ liệu** – chịu trách nhiệm kết nối và thực thi các câu SQL/Stored Procedure trên SQL Server. Được thiết kế theo mẫu **Repository Pattern** với Interface để dễ thay đổi DBMS trong tương lai.

#### Cấu trúc thư mục và file

**📁 Interfaces/** – Định nghĩa hợp đồng (contract) cho mọi Repository:

| File | Mô tả |
|---|---|
| `IGenericRepository<T>` | Interface tổng quát cho CRUD cơ bản: `ListAsync`, `GetAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `IsUsedAsync` |
| `IProductRepository` | Mở rộng từ `IGenericRepository<Product>`, thêm: thuộc tính, ảnh |
| `IOrderRepository` | Mở rộng từ `IGenericRepository<Order>`, thêm: chi tiết đơn, chuyển trạng thái |
| `IEmployeeRepository` | Interface nhân viên |
| `ICustomerRepository` | Interface khách hàng |
| `IDataDictionaryRepository` | Interface danh mục tỉnh thành |
| `IUserAccountRepository` | Interface xác thực tài khoản |

**📁 SQLServer/** – Triển khai cụ thể cho SQL Server:

| File | Mô tả |
|---|---|
| `ProductRepository.cs` | Thực thi SQL cho mặt hàng, ảnh, thuộc tính |
| `CategoryRepository.cs` | Thực thi SQL cho loại hàng |
| `OrderRepository.cs` | Thực thi SQL cho đơn hàng, chi tiết, trạng thái |
| `EmployeeRepository.cs` | Thực thi SQL cho nhân viên |
| `CustomerRepository.cs` | Thực thi SQL cho khách hàng |
| `SupplierRepository.cs` | Thực thi SQL cho nhà cung cấp |
| `ShipperRepository.cs` | Thực thi SQL cho người giao hàng |
| `ProvinceRepository.cs` | Thực thi SQL cho danh mục tỉnh/thành |
| `EmployeeAccountRepository.cs` | Xác thực tài khoản nhân viên |
| `CustomerAccountRepository.cs` | Xác thực tài khoản khách hàng |

---

### Tầng 3 – BusinessLayers (`SV22T1020659.BusinessLayers`)

> **Ý nghĩa:** Tầng **xử lý nghiệp vụ** – là trung gian giữa tầng Presentation và DataLayers. Chứa logic kinh doanh (validate dữ liệu, kiểm tra điều kiện), quyết định khi nào được phép thêm/xóa/sửa. Các lớp này đều là `static` để dùng trực tiếp từ Controller.

#### Các file chính

| File | Mô tả |
|---|---|
| `Configuration.cs` | Lưu trữ `ConnectionString` toàn cục. **Bắt buộc phải gọi `Initialize()`** khi khởi động app |
| `CatalogDataService.cs` | Nghiệp vụ mặt hàng và loại hàng: CRUD, kiểm tra ràng buộc trước khi xóa |
| `PartnerDataService.cs` | Nghiệp vụ khách hàng, nhà cung cấp, người giao hàng |
| `HRDataService.cs` | Nghiệp vụ nhân viên |
| `SalesDataService.cs` | Nghiệp vụ đơn hàng: tạo đơn, duyệt, từ chối, hủy, hoàn tất, giao hàng |
| `DictionaryDataService.cs` | Lấy danh mục tỉnh/thành phố |
| `UserAccountService.cs` | Xác thực tài khoản (đăng nhập, đổi mật khẩu) cho cả nhân viên lẫn khách hàng |

#### Ví dụ nghiệp vụ trong `CatalogDataService`:
```csharp
// Xóa loại hàng chỉ được phép nếu không có mặt hàng nào thuộc loại đó
public static async Task<bool> DeleteCategoryAsync(int CategoryID)
{
    if (await categoryDB.IsUsedAsync(CategoryID))
        return false;  // Có dữ liệu liên quan → không cho xóa

    return await categoryDB.DeleteAsync(CategoryID);
}
```

---

### Tầng 4 – Admin (`SV22T1020659.Admin`)

> **Ý nghĩa:** Ứng dụng web **dành cho nhân viên/quản trị viên** để quản lý toàn bộ dữ liệu hệ thống. Được bảo vệ bởi Cookie Authentication và phân quyền theo vai trò.

#### Cấu trúc thư mục

```
SV22T1020659.Admin/
├── Program.cs               ← Khởi động app, cấu hình middleware, DI
├── AppCodes/
│   ├── ApplicationContext.cs  ← Tiện ích truy cập HttpContext, Session, Config
│   ├── ApiResult.cs           ← Chuẩn hóa JSON response trả về cho AJAX (success/error)
│   ├── CryptHelper.cs         ← Mã hóa MD5 mật khẩu
│   ├── SelectListHelper.cs    ← Tạo SelectList cho DropDown (khách, tỉnh, shipper)
│   └── WebSecurityModels.cs   ← Claims, User Roles (Administrator/Sales/...)
├── Controllers/
│   ├── AccountController.cs   ← Đăng nhập / Đăng xuất / Đăng ký / Truy cập bị từ chối
│   ├── HomeController.cs      ← Trang chủ
│   ├── ProductController.cs   ← CRUD mặt hàng, ảnh, thuộc tính
│   ├── CategoryController.cs  ← CRUD loại hàng
│   ├── CustomerController.cs  ← CRUD khách hàng
│   ├── EmployeeController.cs  ← CRUD nhân viên
│   ├── SupplierController.cs  ← CRUD nhà cung cấp
│   ├── ShipperController.cs   ← CRUD người giao hàng
│   └── OrderController.cs     ← Quản lý đơn hàng, giỏ hàng, tạo đơn, chuyển trạng thái
├── Models/                    ← ViewModel riêng của Admin
├── Views/
│   ├── Account/               ← Login, Register, ChangePassword, AccessDenied
│   ├── Home/                  ← Index.cshtml
│   ├── Product/               ← Index, Edit, Detail, Photo, Attribute views
│   ├── Category/              ← Index, Edit views
│   ├── Customer/              ← Index, Edit views
│   ├── Employee/              ← Index, Edit views
│   ├── Supplier/              ← Index, Create, Edit, Delete, Search views
│   ├── Shipper/               ← Index, Create, Edit, Delete, Search views
│   ├── Order/                 ← Index, Detail, Create, ShowCart, EditCartItem,
│   │                             DeleteCartItem, Search, SearchProduct, Shipping,
│   │                             Accept, Reject, Cancel, Finish views
│   └── Shared/                ← _Layout.cshtml, _ValidationScriptsPartial
├── wwwroot/                   ← CSS, JS, Bootstrap, ảnh sản phẩm (images/products/)
└── appsettings.json           ← ConnectionString, cấu hình
```

#### Các file quan trọng

| File | Chức năng |
|---|---|
| `Program.cs` | Đăng ký dịch vụ (DI), cấu hình Cookie Auth, Session, Route, khởi tạo `BusinessLayers.Configuration` |
| `ApplicationContext.cs` | Wrapper tĩnh cho `HttpContext`, Session (đọc/ghi JSON), đường dẫn wwwroot, cấu hình |
| `ApiResult.cs` | Lớp chuẩn hóa JSON trả về cho AJAX: `{ success, message, data }` |
| `SelectListHelper.cs` | Tạo `IEnumerable<SelectListItem>` cho các dropdown: khách hàng, tỉnh, shipper |
| `WebSecurityModels.cs` | Định nghĩa roles: `Administrator`, `Sales`; extension `User.GetUserData()` |
| `CryptHelper.cs` | Hash mật khẩu bằng MD5 |
| `OrderController.cs` | Xử lý toàn bộ vòng đời đơn hàng + giỏ hàng nội bộ (session-based), AJAX cart operations |

#### Phân quyền Admin
```csharp
[Authorize(Roles = $"{WebUserRoles.Sales},{WebUserRoles.Administrator}")]
public class OrderController : Controller { ... }
```

#### Luồng xử lý đơn hàng trong Admin (`OrderController`)

| Trạng thái | Action | View |
|---|---|---|
| Khởi tạo đơn | `Create` → `Init` | `Create.cshtml`, `ShowCart.cshtml` |
| Thêm mặt hàng | `SearchProduct` → `AddToCart` | `SearchProduct.cshtml` |
| Chỉnh sửa cart | `EditCartItem` | `EditCartItem.cshtml` |
| Xóa mặt hàng | `DeleteCartItem` | `DeleteCartItem.cshtml` |
| Xóa đơn hàng | `Delete` | `Delete.cshtml` |
| Duyệt đơn | `Accept` | `Accept.cshtml` |
| Từ chối | `Reject` | `Reject.cshtml` |
| Giao hàng | `Shipping` | `Shipping.cshtml` |
| Hoàn tất | `Finish` | `Finish.cshtml` |
| Hủy đơn | `Cancel` | `Cancel.cshtml` |

---

### Tầng 5 – Shop (`SV22T1020659.Shop`)

> **Ý nghĩa:** Ứng dụng web **dành cho khách hàng** – xem sản phẩm, đăng ký, đăng nhập, quản lý tài khoản. Cùng chia sẻ CSDL với Admin thông qua BusinessLayers.

#### Cấu trúc thư mục

```
SV22T1020659.Shop/
├── Program.cs               ← Khởi động, thêm StaticFiles từ Admin/wwwroot/images
├── AppCodes/
│   ├── CryptHelper.cs         ← Mã hóa MD5 (bản riêng của Shop)
│   └── WebExtension.cs        ← Tiện ích Session (lưu/đọc Cart), helper hiển thị
├── Controllers/
│   ├── HomeController.cs      ← Trang chủ Shop
│   ├── ProductController.cs   ← Xem danh sách sản phẩm, lọc, chi tiết sản phẩm
│   ├── CartController.cs      ← Quản lý giỏ hàng (thêm/xóa/cập nhật) qua AJAX
│   ├── OrderController.cs     ← Đặt hàng, xem lịch sử đơn hàng của khách hàng
│   └── AccountController.cs   ← Đăng ký, Đăng nhập, Đăng xuất, Profile, Đổi MK
├── Models/
│   ├── LoginViewModel.cs          ← ViewModel form đăng nhập
│   ├── RegisterViewModel.cs       ← ViewModel form đăng ký
│   └── ProfileViewModel.cs        ← ViewModel trang thông tin cá nhân
├── Views/
│   ├── Account/               ← Login, Register, Profile, ChangePassword
│   ├── Home/                  ← Index (trang chủ shop)
│   ├── Product/               ← Index (danh sách + lọc), Detail (chi tiết sản phẩm)
│   ├── Cart/                  ← Index (giỏ hàng: xem, cập nhật số lượng, xóa)
│   ├── Order/                 ← Index (lịch sử đơn), Checkout (thanh toán),
│   │                             Details (chi tiết đơn), Payment (xác nhận)
│   └── Shared/                ← _Layout.cshtml (navbar, footer, giỏ hàng mini)
├── wwwroot/                   ← CSS, JS riêng của Shop
└── appsettings.json           ← ConnectionString (chung với Admin)
```

#### Đặc điểm nổi bật của Shop

- **Dùng chung ảnh với Admin:** `Program.cs` cấu hình `PhysicalFileProvider` trỏ sang `../SV22T1020659.Admin/wwwroot/images/products`
- **Xác thực khách hàng:** Cookie riêng `SV22T1020659.CustomerCookie`, Claims bao gồm `Role = "Customer"`
- **Tự động đăng nhập** sau khi đăng ký thành công
- **Giỏ hàng lưu trên Session:** `CartController` lưu `List<CartItem>` vào Session dưới dạng JSON và cập nhật qua AJAX
- **Đặt hàng trực tiếp:** `OrderController` lấy thông tin từ Session (giỏ hàng + địa chỉ), gọi `SalesDataService` để tạo đơn hàng
- **Luồng đặt hàng:** `Cart/Index` → `Order/Checkout` → `Order/Payment` → `Order/Details`

```csharp
// Shop chia sẻ ảnh từ Admin
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(
        Path.Combine(..., "SV22T1020659.Admin", "wwwroot", "images", "products")),
    RequestPath = "/images/products"
});
```

#### Các file quan trọng trong Shop

| File | Chức năng |
|---|---|
| `CartController.cs` | Thêm/xóa/cập nhật số lượng sản phẩm trong giỏ hàng; trả về JSON qua AJAX |
| `OrderController.cs` | Checkout (xác nhận đơn), lưu đơn hàng vào DB, xem lịch sử đơn |
| `AccountController.cs` | Đăng ký khách mới, đăng nhập/xuất, xem/sửa Profile |
| `WebExtension.cs` | Extension method đọc/ghi Cart từ Session, helper format tiền tệ |

---

## 4. Quy Trình Xử Lý Dữ Liệu

### Luồng xử lý điển hình (Request → Response)

```
Browser gửi HTTP Request
        │
        ▼
[Routing Middleware]  →  Tìm đúng Controller/Action
        │
        ▼
[Authentication/Authorization]  →  Kiểm tra Cookie, Role
        │
        ▼
[Controller (Admin/Shop)]
  │  1. Nhận dữ liệu từ Request (Form, Query String)
  │  2. Gọi hàm trong BusinessLayers
  │
  ▼
[BusinessLayers - DataService]
  │  1. Validate nghiệp vụ (kiểm tra ràng buộc)
  │  2. Gọi Repository qua Interface
  │
  ▼
[DataLayers - Repository]
  │  1. Tạo kết nối SQL Server (từ ConnectionString)
  │  2. Thực thi Stored Procedure / SQL Query
  │  3. Map kết quả → Model object
  │
  ▼
[SQL Server - LiteCommerceDB]
        │
        ▼ (trả kết quả ngược lên)
[Controller]  →  Tạo ViewModel  →  Return View(model)
        │
        ▼
[Razor View (.cshtml)]  →  Render HTML
        │
        ▼
Browser nhận HTTP Response (HTML)
```

---

## 5. Sơ Đồ Mối Liên Hệ Giữa Các Tầng

```
SV22T1020659.Admin ──────────────────────────────────────────────────┐
       │  tham chiếu (project reference)                             │
       ▼                                                             │
SV22T1020659.BusinessLayers                                          │
       │  tham chiếu                                                 │
       ▼                                                             │
SV22T1020659.DataLayers                                              │
       │  tham chiếu                                                 │
       ▼                                                             │
SV22T1020659.Models ←────────────────────────────────────────────────┘
       (tất cả các project đều tham chiếu đến Models)

SV22T1020659.Shop ────────────────────────────────────────────────────┐
       │  tham chiếu                                                  │
       ▼                                                              │
SV22T1020659.BusinessLayers (dùng chung với Admin)                    │
       │                                                              │
       ▼                                                              │
SV22T1020659.DataLayers                                               │
       │                                                              │
       ▼                                                              │
SV22T1020659.Models ←-────────────────────────────────────────────────┘
```

### Quy tắc phụ thuộc (Dependency Rules)

| Tầng | Phụ thuộc vào |
|---|---|
| Models | ❌ Không phụ thuộc vào ai |
| DataLayers | Models |
| BusinessLayers | Models + DataLayers |
| Admin / Shop | Models + BusinessLayers |

> ⚠️ **Quan trọng:** Admin và Shop **không được** gọi trực tiếp DataLayers. Mọi thao tác dữ liệu phải đi qua BusinessLayers.

---

## 6. Ví Dụ Minh Họa

### Ví dụ 1: Khách hàng xem danh sách sản phẩm (Shop)

```
GET /Product/Index?SearchValue=áo&CategoryID=3&MinPrice=100000

1. ProductController.Index(input) [Shop]
   └─ input = { SearchValue="áo", CategoryID=3, MinPrice=100000, Page=1 }

2. CatalogDataService.ListProductsAsync(input) [BusinessLayers]
   └─ Không có logic validate đặc biệt, chuyển thẳng xuống DataLayer

3. ProductRepository.ListAsync(input) [DataLayers/SQLServer]
   └─ Exec SQL/SP với các tham số tìm kiếm
   └─ Trả về PagedResult<Product>

4. ProductController tạo ProductSearchViewModel:
   - Result = danh sách sản phẩm (phân trang)
   - Categories = tất cả loại hàng (cho sidebar lọc)

5. Return View(model) → Razor render Product/Index.cshtml
```

---

### Ví dụ 2: Nhân viên tạo đơn hàng mới (Admin)

```
POST /Order/Init
Body: { customerID=5, deliveryProvince="HCM", deliveryAddress="123 Lê Lợi" }

1. OrderController.Init(...) [Admin]
   ├─ Kiểm tra giỏ hàng (Cart) từ Session
   ├─ Validate: cart không rỗng, có đủ thông tin khách

2. SalesDataService.AddOrderAsync(order) [BusinessLayers]
   └─ Gọi OrderRepository.AddAsync(order)

3. OrderRepository.AddAsync(order) [DataLayers]
   └─ INSERT INTO Orders ...
   └─ Trả về OrderID mới

4. Với mỗi CartItem:
   SalesDataService.AddDetailAsync(detail)
   └─ INSERT INTO OrderDetails ...

5. Xóa giỏ hàng khỏi Session
6. Return Json(orderID) → Frontend redirect sang trang chi tiết đơn hàng
```

---

### Ví dụ 3: Đăng ký tài khoản khách hàng (Shop)

```
POST /Account/Register
Form: { CustomerName, Email, Password, ConfirmPassword, Phone, Province, Address }

1. AccountController.Register(model) [Shop]
   ├─ Validate ModelState (annotation)
   ├─ PartnerDataService.ValidatelCustomerEmailAsync(email) → kiểm tra trùng email

2. PartnerDataService.AddCustomerAsync(customer) [BusinessLayers]
   └─ CustomerRepository.AddAsync(customer) [DataLayers]
      └─ INSERT INTO Customers ... (Password = MD5 hash)
      └─ Trả về CustomerID mới

3. Tự động đăng nhập:
   └─ Tạo ClaimsPrincipal với Role="Customer"
   └─ HttpContext.SignInAsync() → ghi cookie

4. Redirect → Shop/Home/Index
```

---

### Ví dụ 4: Khách hàng thêm vào giỏ hàng và đặt hàng (Shop)

```
POST /Cart/AddToCart
Body: { ProductID=12, Quantity=2 }

1. CartController.AddToCart(productID, quantity) [Shop]
   ├─ Đọc giỏ hàng hiện tại từ Session (List<CartItem>)
   ├─ Nếu sản phẩm đã có → tăng số lượng
   ├─ Nếu chưa có → thêm mới CartItem
   └─ Lưu lại vào Session (JSON)
   └─ Return Json({ success=true, cartCount=... })

---

POST /Order/Checkout
Form: { deliveryAddress, deliveryProvince }

1. OrderController.Checkout(model) [Shop]
   ├─ Lấy thông tin khách hàng từ Claims (UserID)
   ├─ Lấy Cart từ Session → kiểm tra không rỗng

2. SalesDataService.AddOrderAsync(order) [BusinessLayers]
   └─ INSERT INTO Orders → trả về OrderID

3. Với mỗi CartItem trong Session:
   SalesDataService.AddDetailAsync(detail)
   └─ INSERT INTO OrderDetails

4. Xóa giỏ hàng khỏi Session
5. Redirect → Order/Details/{orderID}
```

---

### Ví dụ 5: Nhân viên duyệt / từ chối đơn hàng (Admin – AJAX)

```
POST /Order/Accept   hoặc   /Order/Reject
Body: { orderID=7, note="Đơn hợp lệ" }

1. OrderController.Accept(orderID, note) [Admin]
   ├─ Kiểm tra quyền (Authorize: Sales/Administrator)
   ├─ Validate: đơn phải ở trạng thái "Khởi tạo"

2. SalesDataService.AcceptOrderAsync(orderID, employeeID, note) [BusinessLayers]
   └─ Gọi OrderRepository.AcceptAsync(...)

3. OrderRepository.AcceptAsync(...) [DataLayers]
   └─ UPDATE Orders SET Status=2, EmployeeID=..., Notes=... WHERE OrderID=7

4. Return ApiResult.Success("Đơn hàng đã được duyệt")
   └─ Frontend nhận JSON → cập nhật badge trạng thái, hiện thông báo
```

---

### Ví dụ 6: Nhân viên tìm kiếm và thêm sản phẩm vào giỏ hàng (Admin)

```
GET  /Order/SearchProduct?searchValue=áo+thun&page=1   (modal tìm kiếm)
POST /Order/AddToCart   { productID=5, salePrice=150000, quantity=3 }

1. OrderController.SearchProduct(searchValue, page) [Admin]
   └─ CatalogDataService.ListProductsAsync(...) → render SearchProduct.cshtml (partial)

2. OrderController.AddToCart(productID, salePrice, quantity) [Admin]
   ├─ Đọc Cart (List<CartItem>) từ Session
   ├─ Nếu đã có productID → cập nhật số lượng & giá
   ├─ Nếu chưa có → thêm CartItem mới
   └─ Ghi Session → Return JSON { success, cartCount, html (ShowCart partial) }

3. Frontend nhận JSON → cập nhật bảng giỏ hàng không tải lại trang
```

---

## 7. Tóm Tắt Chức Năng Theo Module

### 🛠 Admin – Quản Trị Hệ Thống

| Module | Chức năng chính |
|---|---|
| **Tài khoản** | Đăng nhập, đăng xuất, đăng ký nhân viên, đổi mật khẩu |
| **Mặt hàng** | CRUD sản phẩm, quản lý ảnh, thuộc tính mở rộng |
| **Loại hàng** | CRUD danh mục sản phẩm (kiểm tra ràng buộc trước khi xóa) |
| **Khách hàng** | CRUD khách hàng, tìm kiếm/phân trang |
| **Nhân viên** | CRUD nhân viên, phân quyền role |
| **Nhà cung cấp** | CRUD supplier, liên kết với sản phẩm |
| **Người giao hàng** | CRUD shipper, gán vào đơn hàng khi giao |
| **Đơn hàng** | Tạo đơn (session cart), tìm kiếm, xem chi tiết, chuyển trạng thái (Duyệt/Từ chối/Giao/Hoàn tất/Hủy) |

### 🛒 Shop – Cửa Hàng Khách Hàng

| Module | Chức năng chính |
|---|---|
| **Tài khoản** | Đăng ký, đăng nhập, xem/sửa profile, đổi mật khẩu |
| **Sản phẩm** | Xem danh sách (lọc theo danh mục, giá, từ khóa), xem chi tiết |
| **Giỏ hàng** | Thêm/xóa/cập nhật số lượng qua AJAX, xem tổng tiền |
| **Đặt hàng** | Checkout (nhập địa chỉ giao hàng), xác nhận thanh toán, xem lịch sử đơn hàng |

---

> [!NOTE]
> Admin và Shop **dùng chung cơ sở dữ liệu** (`LiteCommerceDB`) và **BusinessLayers**. Mọi cập nhật sản phẩm/đơn hàng từ Admin sẽ phản ánh ngay trên Shop và ngược lại.

---

## 8. Hướng Dẫn Cài Đặt & Chạy Dự Án

### Yêu cầu môi trường

| Phần mềm | Phiên bản tối thiểu |
|---|---|
| .NET SDK | 8.0 trở lên |
| SQL Server | 2019 / 2022 (hoặc SQL Server Express) |
| Visual Studio / VS Code | Visual Studio 2022+ |

### Các bước cài đặt

**Bước 1:** Clone hoặc tải dự án về máy local.

**Bước 2:** Tạo cơ sở dữ liệu bằng cách chạy file SQL:
```sql
-- 1. Tạo tables
LiteCommerceDB_CreateTables.sql

-- 2. Nạp dữ liệu mẫu (nếu có)
LiteCommerceDB_Update2026.sql
```

**Bước 3:** Cấu hình `ConnectionString` trong cả hai project:

```json
// SV22T1020659.Admin/appsettings.json
// SV22T1020659.Shop/appsettings.json
{
  "ConnectionStrings": {
    "LiteCommerceDB": "Server=.;Database=LiteCommerceDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**Bước 4:** Đặt **Multiple Startup Projects** trong Visual Studio:
- Click chuột phải vào Solution → `Properties`
- Chọn `Multiple startup projects`
- Đặt cả `SV22T1020659.Admin` và `SV22T1020659.Shop` → `Start`

**Bước 5:** Nhấn `F5` hoặc `Ctrl+F5` để chạy.

### URL mặc định

| Ứng dụng | URL mặc định |
|---|---|
| Admin | `https://localhost:7046` (hoặc port khác trong Properties) |
| Shop | `https://localhost:7115` (hoặc port khác trong Properties) |

> [!IMPORTANT]
> Ảnh sản phẩm được lưu trong `SV22T1020659.Admin/wwwroot/images/products/`. Shop đọc ảnh từ đường dẫn này qua `PhysicalFileProvider`. Đảm bảo thư mục này tồn tại khi chạy Shop.

---

 --Cập nhật: 07/04/2026
