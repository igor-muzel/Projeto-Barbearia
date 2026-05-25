using Microsoft.AspNetCore.Mvc;

namespace ProjectBarber.Controllers
{
    public class AuthContoller : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}
