using Microsoft.AspNetCore.Mvc;
using SV22T1020659.Shop.Models;
using System.Diagnostics;
using SV22T1020659.BusinessLayers;
using SV22T1020659.Models.Catalog;

namespace SV22T1020659.Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var input = new ProductSearchInput()
            {
                Page = 1,
                PageSize = 8,
                SearchValue = "",
                CategoryID = 0,
                SupplierID = 0,
                MinPrice = 0,
                MaxPrice = 0
            };
            var data = await CatalogDataService.ListProductsAsync(input);
            return View(data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
