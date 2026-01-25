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
            // For now, accept any login and redirect to Dashboard
            // In a real app, validate against DB here
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            // In a real app, sign out the user here (e.g. HttpContext.SignOutAsync)
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
