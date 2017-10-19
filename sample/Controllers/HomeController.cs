using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        [OutputCache(Duration = 600)]
        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Api()
        {
            HttpContext.EnableOutputCaching(TimeSpan.FromMinutes(1));
            return View("Index");
        }

        [OutputCache(Duration = 600, VaryByParam = "foo")]
        public IActionResult Query()
        {
            return View("Index");
        }

        [OutputCache(Profile = "default")]
        public IActionResult Profile()
        {
            return View("Index");
        }

        public IActionResult Redirect()
        {
            return RedirectToActionPermanent("Index");
        }
    }
}
