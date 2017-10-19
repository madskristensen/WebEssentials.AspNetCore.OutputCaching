using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using WebEssentials.AspNetCore.OutputCaching;
using Microsoft.Net.Http.Headers;

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
            Response.Headers.Add(HeaderNames.CacheControl, "no-store, no-cache, must-revalidate");
            return RedirectToActionPermanent("Index");
        }
    }
}
