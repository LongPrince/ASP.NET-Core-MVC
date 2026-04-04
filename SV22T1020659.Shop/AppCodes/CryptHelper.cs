using System.Security.Cryptography;
using System.Text;

namespace SV22T1020659.Shop
{
    /// <summary>
    /// Lớp cung cấp các hàm tiện ích sử dụng cho mã hóa MD5
    /// </summary>
    public static class CryptHelper
    {
        /// <summary>
        /// Mã hóa MD5 một chuỗi
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
