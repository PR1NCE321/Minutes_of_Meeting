using Microsoft.AspNetCore.Mvc;

namespace MOM.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            
            
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
