using Microsoft.AspNetCore.Mvc;
using System;

namespace SV22T1020659.Admin.Controllers
{
    public class Product : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Detail(int id)
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult Delete ()
        {
            return View();
        }
        public IActionResult ListAttributes(int id)
        {
            return View();
        }
        public IActionResult CreateAttributes(int id)
        {
            return View();
        }
        public IActionResult EditAttributes(int id,int attributeID)
        {
            return View();
        }
        public IActionResult DeleteAttributes(int id, int attributeID)
        {
            return View();
        }
        public IActionResult ListPhoto(int id)
        {
            return View();
        }
        public IActionResult CreateListPhoto(int id)
        {
            return View();
        }
        public IActionResult EditPhoto(int id,int photoid)
        {
            return View();
        }
        public IActionResult DeletePhoto(int id, int photoid)
        {
            return View();
        }
    }
}
