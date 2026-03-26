using Microsoft.AspNetCore.Mvc;

namespace SV22T1020659.Admin.Controllers
{
    /// <summary>
    /// Cac chuc nang lien quan den nha cung cap
    /// </summary>
    public class Supplier : Controller
    {/// <summary>
    /// TIm kiem va hien thi danh sach nha cung cap
    /// </summary>
    /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Bo sung nha cung cap moi
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung nhà cung cấp";
            return View("Edit");
        }/// <summary>
        /// Chinh sua,cap nhat nha cung 
        /// </summary>
        /// <param name="id">Ma nha cap cap can cap nhat</param>
        /// <returns></returns>
        public IActionResult Edit(int id)
        {
            ViewBag.Title = "Cập nhật nhà cung cấp";
            return View();
        }/// <summary>
        /// Xoa nha cung cap
        /// </summary>
        /// <param name="id">Ma nha cung cap can xoa</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            return View();
        }
    }
}
