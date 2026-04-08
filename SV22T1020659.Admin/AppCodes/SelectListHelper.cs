using Microsoft.AspNetCore.Mvc.Rendering;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Common;
using SV22T1020659.Models.Sales;

namespace SV22T1020659.Admin
{
    /// <summary>
    /// Lớp cung cấp các hàm tiện ích dùng cho SelectList (DropDownList)
    /// </summary>
    public static class SelectListHelper
    {
        /// <summary>
        /// Danh sách các tỉnh thành
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SelectListItem>> ProvincesAsync()
        {
            var list = new List<SelectListItem>();
            var result = await DictionaryDataService.ListProvincesAsync();
            foreach (var item in result)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách loại hàng
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SelectListItem>> Categories()
        {
            var list = new List<SelectListItem>();
            var input = new PaginationSearchInput() { Page = 1, PageSize = 1000, SearchValue = "" };
            var result = await CatalogDataService.ListCategoriesAsync(input);
            foreach (var item in result.DataItems)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách nhà cung cấp
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SelectListItem>> Suppliers()
        {
            var list = new List<SelectListItem>();
            var input = new PaginationSearchInput() { Page = 1, PageSize = 1000, SearchValue = "" };
            var result = await PartnerDataService.ListSuppliersAsync(input);
            foreach (var item in result.DataItems)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách người giao hàng
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SelectListItem>> Shippers()
        {
            var list = new List<SelectListItem>();
            var input = new PaginationSearchInput() { Page = 1, PageSize = 1000, SearchValue = "" };
            var result = await PartnerDataService.ListShippersAsync(input);
            foreach (var item in result.DataItems)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ShipperID.ToString(),
                    Text = item.ShipperName
                });
            }
            return list;
        }

        /// <summary>
        /// Danh sách khách hàng
        /// </summary>
        /// <returns></returns>
        public static async Task<List<SelectListItem>> Customers()
        {
            var list = new List<SelectListItem>();
            var input = new PaginationSearchInput() { Page = 1, PageSize = 1000, SearchValue = "" };
            var result = await PartnerDataService.ListCustomersAsync(input);
            foreach (var item in result.DataItems)
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CustomerID.ToString(),
                    Text = item.CustomerName
                });
            }
            return list;
        }

        /// <summary>
        /// Các trạng thái của đơn hàng
        /// </summary>
        /// <returns></returns>
        public static List<SelectListItem> OrderStatus()
        {
            return new List<SelectListItem>
            {
                new SelectListItem() { Value = "", Text = "-- Trạng thái ---" },
                new SelectListItem() { Value = OrderStatusEnum.New.ToString(), Text = OrderStatusEnum.New.GetDescription() },
                new SelectListItem() { Value = OrderStatusEnum.Accepted.ToString(), Text = OrderStatusEnum.Accepted.GetDescription() },
                new SelectListItem() { Value = OrderStatusEnum.Shipping.ToString(), Text = OrderStatusEnum.Shipping.GetDescription() },
                new SelectListItem() { Value = OrderStatusEnum.Completed.ToString(), Text = OrderStatusEnum.Completed.GetDescription() },
                new SelectListItem() { Value = OrderStatusEnum.Rejected.ToString(), Text = OrderStatusEnum.Rejected.GetDescription() },
                new SelectListItem() { Value = OrderStatusEnum.Cancelled.ToString(), Text = OrderStatusEnum.Cancelled.GetDescription() },
            };
        }
    }
}
