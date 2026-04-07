using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.DataLayers.SQLServer;
using SV22T1020659.Models.Partner;
using SV22T1020659.Models.Security;
using System.Threading.Tasks;

namespace SV22T1020659.BusinessLayers
{
    public class AccountDataService
    {
        private readonly IUserAccountRepository _employeeAccountDB;
        private readonly IUserAccountRepository _customerAccountDB;

        /// <summary>
        /// Khởi tạo AccountDataService không cần truyền tham số
        /// </summary>
        public AccountDataService()
        {
            string connectionString = Configuration.ConnectionString;
            _employeeAccountDB = new EmployeeAccountRepository(connectionString);
            _customerAccountDB = new CustomerAccountRepository(connectionString);
        }

        #region Xử lý tài khoản Nhân viên (Dành cho Admin)

        /// <summary>
        /// Kiểm tra thông tin đăng nhập của nhân viên
        /// </summary>
        public async Task<UserAccount?> AuthorizeEmployeeAsync(string userName, string password)
        {
            return await _employeeAccountDB.AuthenticateAsync(userName, password);
        }

        /// <summary>
        /// Thay đổi mật khẩu của nhân viên
        /// </summary>
        public async Task<bool> ChangeEmployeePasswordAsync(string userName, string password)
        {
            return await _employeeAccountDB.ChangePasswordAsync(userName, password);
        }

        #endregion

        #region Xử lý tài khoản Khách hàng (Dành cho Shop)

        /// <summary>
        /// Kiểm tra thông tin đăng nhập của khách hàng
        /// </summary>
        public async Task<UserAccount?> AuthorizeCustomerAsync(string userName, string password)
        {
            return await _customerAccountDB.AuthenticateAsync(userName, password);
        }

        /// <summary>
        /// Thay đổi mật khẩu của khách hàng
        /// </summary>
        public async Task<bool> ChangeCustomerPasswordAsync(string userName, string password)
        {
            return await _customerAccountDB.ChangePasswordAsync(userName, password);
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới
        /// </summary>
        public async Task<int> RegisterCustomerAsync(Customer data, string password)
        {
            if (_customerAccountDB is CustomerAccountRepository customerRepo)
            {
                return await customerRepo.RegisterCustomerAsync(data, password);
            }
            return 0; // Thất bại
        }

        #endregion
    }
}
