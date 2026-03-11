using Microsoft.AspNetCore.Mvc;

namespace RegistrationSample.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Login", "Account");
    }
}
