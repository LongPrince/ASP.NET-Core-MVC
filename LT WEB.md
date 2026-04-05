\*Controller

\-Các hàm public trong controller ở đây được gọi là Action và thằng đéo này được gọi từ URL

Cú pháp URL: http://..../Action/

URL: https://...//Student/CongHaiSo?a=2\&b=7



https://...//Student/CongHaiSo?a=2\&b=7



\*View



\_\_\_\_

**06/03/2026**



**Trong tổ chức three-layer**

\- Có những loại model nào ?

\- Cách tổ chức model có những cách tiếp cận nào ( triết lý tổ chức model) ?



**Model**

\- Biểu diễn cho các đối tượng CSDL : ***Entity***

\- Biểu diễn cho quy trình truy vấn phức tạp: ***DTO***

\- Trình diễn dữ liệu cho người đang xem : ***View Models***

\- Nhận dữ liệu đầu vào: ***Input Model or Request Model***



Model -> rất nhiều class | Phải phân chia Domain

Nguyễn lý phát triển phần mềm DDD (Domain-Driven Design)



**Các Domain**

\-Data Dictionary

\-Partner

\-HR : xử lý nghiệp vụ

\-Catalog (ue) :  thuộc tính loại hàng

\-Sale: Order

\-Security

\-Common

\-



\_\_\_\_

**10/03/2026 \[ DATA LAYER ]**

Các đối tượng Supplier, Customer, Shipper, Employee, Category cần các phép xử lý dữ liệu

"tương tự" nhau: (CRUD đơn giản)

\-Tìm kiếm, lấy dữ liệu dưới dạng phân trang

\-Truy vấn dữ liệu của 1 bản ghi  

\-Insert được một bản ghi vào bảng

\-Update được một dữ liệu vào bảng

\-Delete một bản ghi từ bảng 

\-Kiểm tra xem một bản ghi có dữ liệu liên quan hay không ?(Để ngăn chặn không cho xóa)

\-> Tạo interface định nghĩa các phép xử lý dữ liệu cần cài đặt trên đối tượng này

\-**Interface IGennericRepository<T>**

Với Customer và Employee,Email được sử dụng làm tên đăng nhập -> Email không được trùng -> cần có chức năng kiếm tra Email có trùng không ? => cần hàm ValidateEmail

=> interface trên chưa đủ chức năng cần cài đặt => thêm interface xử lý dữ liệu cho Employee và cho Customer kế thừa interface trên

\->IEmployeeRepository:**IGenericRepository<Employee>**

\->IcustomerRepository:**IGenericRepository<Customer>**



Đối với từ điển dữ liệu:

\-Lấy được ds dữ liệu

\->interface cho tử điển dữ liệu

\->interface IDataDictionaryRepository<T>



Đối với mặt hàng

\-Tìm kiếm, hiển thị dữ liệu phân trang

&#x20;	Đầu vào : lớp **PaginationSearchIput**  chưa đáp ứng được yêu cầu => Tạo lớp đầu 	vào cho tìm kiém mặt hàng **ProductSearchIput** kế thừa **PaginationSearchIpu**

\-Lấy được thông tin 1 mặt hàng

\-Bổ sung, cập nhật, xóa, mặt hàng đã có dữ liệu liên quan chưa ?

\-Lấy danh sách thuộc tính.

\-Lấy thông tin 1 thuộc tính

\-Bổ sung thuộc tính

\-CẬP nhật thuộc tính

\-Xóa thuộc tính

Lấy danh sách ảnh mặt hàng

\-Lấy thông tin 1 ảnh

\-Bổ sung ảnh

\-Cập nhật ảnh

\-Xóa ảnh

\->interface IProductRepository



Đối với đơn hàng: 

\-Tìm kiếm, hiển thị phân trang

&#x20;+Đầu vào: OrderSearchIput kế thừa PaginationSearchInput

&#x20;+Đầu ra: sử dụng lớp OrderViewInfo để biểu diễn thông tin của đơn hàng(không sd Order)

\-Xem thông tin đầy đủ của một đơn hàng

\-Bổ sung đơn hàng

\-Cập nhật đơn hàng

\-Xóa đơn hàng

\-Lấy danh sách mặt hàng bán trong đơn hàng

\-Lấy thông tin 1 mặt hàng bán trong đơn hàng

\-Cập nhật 1 mặt hàng trong đơn hàng (sl,giá)

\-Xóa mặt hàng khỏi đơn hàng.

Liên quan đến tài khoản (của Employee, của Custormer)

* Kiểm tra thông tin đăng nhập hợp lệ ?
* Đổi mật khẩu

\-> **Interface IUserAccountRepository** 





\_\_

PROMT 1: 

CODE: 

using LiteCommerce.Models.Common;



namespace LiteCommerce.DataLayers.Interfaces

{

&#x20;   /// <summary>

&#x20;   /// Định nghĩa các phép xử lý dữ liệu đơn giản trên một

&#x20;   /// kiểu dữ liệu T nào đó (T là một Entity/DomainModel nào đó)

&#x20;   /// </summary>

&#x20;   /// <typeparam name="T"></typeparam>

&#x20;   public interface IGenericRepository<T> where T : class

&#x20;   {

&#x20;       /// <summary>

&#x20;       /// Truy vấn, tìm kiếm dữ liệu và trả về kết quả dưới dạng được phân trang

&#x20;       /// </summary>

&#x20;       /// <param name="input">Đầu vào tìm kiếm, phân trang</param>

&#x20;       /// <returns></returns>

&#x20;       Task<PagedResult<T>> ListAsync(PaginationSearchInput input);

&#x20;       /// <summary>

&#x20;       /// Lấy dữ liệu của một bản ghi có mã là id (trả về null nếu không có dữ liệu)

&#x20;       /// </summary>

&#x20;       /// <param name="id">Mã của dữ liệu cần lấy</param>

&#x20;       /// <returns></returns>

&#x20;       Task<T?> GetAsync(int id);

&#x20;       /// <summary>

&#x20;       /// Bổ sung một bản ghi vào bảng trong CSDL

&#x20;       /// </summary>

&#x20;       /// <param name="data">Dữ liệu cần bổ sung</param>

&#x20;       /// <returns>Mã của dòng dữ liệu được bổ sung (thường là IDENTITY)</returns>

&#x20;       Task<int> AddAsync(T data);

&#x20;       /// <summary>

&#x20;       /// Cập nhật một bản ghi trong bảng của CSDL

&#x20;       /// </summary>

&#x20;       /// <param name="data">Dữ liệu cần cập nhật</param>

&#x20;       /// <returns></returns>

&#x20;       Task<bool> UpdateAsync(T data);

&#x20;       /// <summary>

&#x20;       /// Xóa bản ghi có mã là id

&#x20;       /// </summary>

&#x20;       /// <param name="id">Mã của bản ghi cần xóa</param>

&#x20;       /// <returns></returns>

&#x20;       Task<bool> DeleteAsync(int id);

&#x20;       /// <summary>

&#x20;       /// Kiểm tra xem một bản ghi có mã là id có dữ liệu liên quan hay không?

&#x20;       /// </summary>

&#x20;       /// <param name="id"></param>

&#x20;       /// <returns></returns>

&#x20;       Task<bool> IsUsed(int id);

&#x20;   }

}

/////

namespace LiteCommerce.Models.Partner

{

&#x20;   /// <summary>

&#x20;   /// Nhà cung cấp

&#x20;   /// </summary>

&#x20;   public class Supplier

&#x20;   {

&#x20;       /// <summary>

&#x20;       /// Mã nhà cung cấp

&#x20;       /// </summary>

&#x20;       public int SupplierID { get; set; }

&#x20;       /// <summary>

&#x20;       /// Tên nhà cung cấp

&#x20;       /// </summary>

&#x20;       public string SupplierName { get; set; } = string.Empty;

&#x20;       /// <summary>

&#x20;       /// Tên giao dịch

&#x20;       /// </summary>

&#x20;       public string ContactName { get; set; } = string.Empty;

&#x20;       /// <summary>

&#x20;       /// Tỉnh thành

&#x20;       /// </summary>

&#x20;       public string? Province { get; set; }

&#x20;       /// <summary>

&#x20;       /// Địa chỉ

&#x20;       /// </summary>

&#x20;       public string? Address { get; set; }

&#x20;       /// <summary>

&#x20;       /// Điện thoại

&#x20;       /// </summary>

&#x20;       public string? Phone { get; set; }

&#x20;       /// <summary>

&#x20;       /// Email

&#x20;       /// </summary>

&#x20;       public string? Email { get; set; }

&#x20;   }

}





Yêu cầu:

\-Constructor của lớp có tham số đầu vào là connection String

\-Sử dụng thư viện Dapper,Microsoft.SqlClinet

\-Lớp này nằm trong namespace LiteCommerce.Datalayers.SQLServer

\-Viết summary cho lớp và các hàm trong lớp

\_\_\_\_

PROMT 2:

cài đặt chop lớp shipper

namespace LiteCommerce.Models.Partner

{

&#x20;   /// <summary>

&#x20;   /// Nhà cung cấp

&#x20;   /// </summary>

&#x20;   public class Supplier

&#x20;   {

&#x20;       /// <summary>

&#x20;       /// Mã nhà cung cấp

&#x20;       /// </summary>

&#x20;       public int SupplierID { get; set; }

&#x20;       /// <summary>

&#x20;       /// Tên nhà cung cấp

&#x20;       /// </summary>

&#x20;       public string SupplierName { get; set; } = string.Empty;

&#x20;       /// <summary>

&#x20;       /// Tên giao dịch

&#x20;       /// </summary>

&#x20;       public string ContactName { get; set; } = string.Empty;

&#x20;       /// <summary>

&#x20;       /// Tỉnh thành

&#x20;       /// </summary>

&#x20;       public string? Province { get; set; }

&#x20;       /// <summary>

&#x20;       /// Địa chỉ

&#x20;       /// </summary>

&#x20;       public string? Address { get; set; }

&#x20;       /// <summary>

&#x20;       /// Điện thoại

&#x20;       /// </summary>

&#x20;       public string? Phone { get; set; }

&#x20;       /// <summary>

&#x20;       /// Email

&#x20;       /// </summary>

&#x20;       public string? Email { get; set; }

&#x20;   }

}

\--

PROMT 3:

cài đặt chop lớp  Category

namespace LiteCommerce.Models.Catalog

{

&#x20;   /// <summary>

&#x20;   /// Loại hàng

&#x20;   /// </summary>

&#x20;   public class Category

&#x20;   {

&#x20;       /// <summary>

&#x20;       /// Mã loại hàng

&#x20;       /// </summary>

&#x20;       public int CategoryID { get; set; }

&#x20;       /// <summary>

&#x20;       /// Tên loại hàng

&#x20;       /// </summary>

&#x20;       public string CategoryName { get; set; } = string.Empty;

&#x20;       /// <summary>

&#x20;       /// Mô tả loại hàng

&#x20;       /// </summary>

&#x20;       public string? Description { get; set; }

&#x20;   }

}



\---

PROMT 4:



Cho interface sau:

using LiteCommerce.Models.Partner;



namespace LiteCommerce.DataLayers.Interfaces

{

&#x20;   /// <summary>

&#x20;   /// Định nghĩa các phép xử lý dữ liệu trên Customer

&#x20;   /// </summary>

&#x20;   public interface ICustomerRepository : IGenericRepository<Customer>

&#x20;   {

&#x20;       /// <summary>

&#x20;       /// Kiểm tra xem một địa chỉ email có hợp lệ hay không?

&#x20;       /// </summary>

&#x20;       /// <param name="email">Email cần kiểm tra</param>

&#x20;       /// <param name="id">

&#x20;       /// Nếu id = 0: Kiểm tra email của khách hàng mới.

&#x20;       /// Nếu id <> 0: Kiểm tra email đối với khách hàng đã tồn tại

&#x20;       /// </param>

&#x20;       /// <returns></returns>

&#x20;       Task<bool> ValidateEmailAsync(string email, int id = 0);

&#x20;   }

}

Sử dụng interface trên để cài đặt cho Customer

namespace LiteCommerce.Models.Partner

{

&#x20;   /// <summary>

&#x20;   /// Khách hàng

&#x20;   /// </summary>

&#x20;   public class Customer

&#x20;   {

&#x20;       /// <summary>

&#x20;       /// Mã khách hàng

&#x20;       /// </summary>

&#x20;       public int CustomerID { get; set; }

&#x20;       /// <summary>

&#x20;       /// Tên khách hàng

&#x20;       /// </summary>

&#x20;       public string CustomerName { get; set; } = string.Empty;

&#x20;       /// <summary>

&#x20;       /// Tên giao dịch

&#x20;       /// </summary>

&#x20;       public string ContactName { get; set; } = string.Empty;

&#x20;       /// <summary>

&#x20;       /// Tỉnh/thành

&#x20;       /// </summary>

&#x20;       public string? Province { get; set; }

&#x20;       /// <summary>

&#x20;       /// Địa chỉ

&#x20;       /// </summary>

&#x20;       public string? Address { get; set; }

&#x20;       /// <summary>

&#x20;       /// Điện thoại

&#x20;       /// </summary>

&#x20;       public string? Phone { get; set; }

&#x20;       /// <summary>

&#x20;       /// Email

&#x20;       /// </summary>

&#x20;       public string Email { get; set; } = string.Empty;

&#x20;       /// <summary>

&#x20;       /// Khách hàng hiện có bị khóa hay không?

&#x20;       /// </summary>

&#x20;       public bool? IsLocked { get; set; }

&#x20;   }

}

\_\_\_

PROMT 5: 

---
13/3
các lớp xử lý dữ liệu đã cài đặt
-SupplierRepository: IGenericRepository<Supplier>
-ShipperRepository: IGenericRepository<Shipper>
-CategoryRepository: IGenericRepository<Category>
-CustormerRepository: ICustomerRepository
-EmployeeRepository: IEmployeeRepository
-ProductRepository: IProductRepository
-OrderRepository: IOderRepository
-ProvinceRepository: IDataDictionaryRepository<Province>
-EmployeeAccountRepository; IUserAccountRepository
-CustormerAccountRepository: IUserAccoutRepository

Cài đặt packge Newtonsoft.json cho Admin
+Tạo thư mục Appcodes trong Admin, copy file ApplicationContext.cs vào thư mục ( chú ý đôi namespace)
+ Sửa lại code file Program.cs theo mẫu (chú ý đổi namespace)
+ Bổ sung cho appsettings.json cấu hình ConnectionStrings kết nối đến CSDL LiteCommerceDB
 (PROMT: Viết chuỗi tham số kết nối đến csdl LiteCommerceDB trong SQL Server trên máy tính cục bộ, sử dụng Windows Authentication, dùng cho ASP.NET Core MVC)
+ Bổ sung lớp Configuration (trong Business)
+ Bổ sung lớp PartnerDataService trong BusinessLayers
 -Tìm kiếm và lấy danh sách nhà cc
 -Bổ sung,sửa xóa.

BusinessLayer:
-ParterDataService
-DictionaryDataService
-HRDataSerivce
-CatalogDataService
-SalesDataService
-SecurityDataService

trong action có kiểu trả về cho view thì view phải khai báo kiểu dữ liệu

--- 17/3 
Nhược điểm tìm kiếm phân trang: load lại toàn trang -> chỉ cần load lại phần kết quả tìm kiếm
- Không lưu lại được " trạng thái" làm việc
- Phải biết được kiểu của dữ liệu mà action truyền cho view là gì
- Trong view nhận dữ liệu của Action, phải khai báo kiểu dữ liệu nhận được bởi lệnh:
  @model kiểu dữ liệu
-Trong view, dự liệu mà được nhận từ Action được lưu trong thuộc tính có tên là Model
Khi action truyền dữ liệu cho view dưới dạng model
-Sử dụng sesion để lưu lại điều kiện tìm kiếm
- Khi thực hiện tìm kiếm (search): lưu lại điều kiện (input) vào một biến sesion
- Khi vào chức năng đăng nhập điều kiện tìm kiếm (Index): kiểm tra nếu trong sesion đang lưu điều kiện tìm kiếm thì lấy điều kiện này để dùng, nếu ko có
thì mới tạo đk mặc định.
Trong ứng web, sesion là gì ?

Clean code: nếu có biến nào sử dụng 2 lần trở lên thì nên khai báo const


Tìm kiếm phân trang cho: mục còn lại


=====
27/3
PROMT viết ở trang view/order/create
viết hàm JavaScript thuần show cart có chức năng get trang order/.... ( trang create ở order view)
---------z
viet ham JavaScript addCartItem(event,from ) có chức năng POST dữ liệu trong form lên API/Order/AddCartItem. Kết quả trả về  của api là một đối tượng json có 2 trường code và message  ( TRONG  search product)
nếu code =0  thì thông báo lỗi message
nếu code !=0 thì gọi hàm showcart
============

viết hàm javascript delete cartItem(productID) có chức năng POST đến API Order/DeletecartItem?product={productID}. Sau khi thực hiện xong nhận đc kết quả thì gọi hàm showcart
=============
đoạn code sau ko bắt đc sự kiện nếu nạp trang bằng ajac, cách giải quyết ntn
document.querySelectorAll(".open-modal").forEach(element => {                
     element.addEventListener("click", function (e) {                    
        openModal(e, this);
    });
});  ///

===='
viêt hàm javascript updateCartItem(event, form) có chức năng Post dữ liệu lên API/Order/UpdateCartItem
nếu code =0  thì thông báo lỗi message
ngược lại đóng modal có id là dialogModal và gọi hàm showcart
modal dùng bootstrap

=====
31/03

Thư viên security Owin
-Nguyên tắc chung:
+ Người sử dụng cung cấp thông tin để kiểm tra (user/name + password/Sinh trắc học) AuthID/OpenID.
+ Hệ thống kiểm tra xem thông tin có hợp lệ không ? Nếu hợp lệ cấp cho người dùng một "chứng nhận" (principal) và giao chứng nhận cho người dùng (cookie), trong web api access token.
+ Phía client lưu giữ cookie, và đính kèm cookie ( trong phần header) mỗi khi có request lên server.
+ Phía server dựa vào cookie để kiểm tra xem người dùng có hợp lệ không.
 
2 thuật ngữ:
-Authentication: Hệ thống kiểm tra thông tin hợp lệ hay không -> cấp chứng nhận (b1)
-Authorization:  Kiểm tra người dùng có giấy chứng nhận hợp lệ không  (b2)
Trong ASP.NET Core, muốn sử dụng Auth thì phải đăng ký.

Để sử dụng cơ chế Authorization đối với các Controller hoặc action, đặt chỉ thị sau ở phía trước:
[Authorization] hết , cấm nhầm hơn bỏ sót
để bỏ Authorize
[AllowAnonymous]

Trong Action hoặc View, thông qua thuộc tính User để lấy được "giấy chứng nhận" đã cấp cho Client

Trong trường hợp cần kiểm tra quyền của người dùng (đã đăng nhập), sử dụng chỉ thị: 
[Authorize](Roles="DanhSachQuyen)

asp-for dung khi co modles
NHÓM 2.01_CSDL PHÂN TÁN - GỬI THẦY BÀI TẬP CHƯƠNG 4
BÀI TẬP CHƯƠNG 3 TRUY VẤN CSDL PHÂN TÁN - NHÓM 1
LỚP: Hệ cơ sở dữ liệu phân tán - Nhóm 2

build : dotnet run --project SV22T1020659.Admin\SV22T1020659.Admin.csproj
dotnet clean
