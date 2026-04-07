using SV22T1020659.DataLayers.Interfaces;
using SV22T1020659.DataLayers.SQLServer;
using SV22T1020659.Models.Security;
using System.Threading.Tasks;
using SV22T1020659.Models.Partner;

namespace SV22T1020659.BusinessLayers
{
    /// <summary>
    /// Cung cấp các dịch vụ liên quan đến tài khoản
    /// </summary>
    public static class UserAccountService
    {
        private static readonly IUserAccountRepository employeeAccountDB;
        private static readonly IUserAccountRepository customerAccountDB;

        /// <summary>
        /// Constructor
        /// </summary>
        static UserAccountService()
        {
            employeeAccountDB = new EmployeeAccountRepository(Configuration.ConnectionString);
            customerAccountDB = new CustomerAccountRepository(Configuration.ConnectionString);
        }

        /// <summary>
        /// Xác thực tài khoản
        /// </summary>
        public static async Task<UserAccount?> AuthenticateAsync(string userName, string password, bool isEmployee = true)
        {
            if (isEmployee)
                return await employeeAccountDB.AuthenticateAsync(userName, password);
            else
                return await customerAccountDB.AuthenticateAsync(userName, password);
        }

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        public static async Task<bool> ChangePasswordAsync(string userName, string password, bool isEmployee = true)
        {
            if (isEmployee)
                return await employeeAccountDB.ChangePasswordAsync(userName, password);
            else
                return await customerAccountDB.ChangePasswordAsync(userName, password);
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng mới
        /// </summary>
        public static async Task<int> RegisterCustomerAsync(Customer data, string password)
        {
            var customerRepo = customerAccountDB as CustomerAccountRepository;
            if (customerRepo != null)
            {
                return await customerRepo.RegisterCustomerAsync(data, password);
            }
            return 0;
        }
    }
}
