using Microsoft.AspNetCore.Mvc;

namespace ShoppingCart.PL.Controllers.Admin
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LogInError()
        {
            return View();
        }
    }
}
