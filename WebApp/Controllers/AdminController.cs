using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;


[Authorize(Policy = "Admins")]
public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Policy = "CIOs")]
    public IActionResult Settings()
    {
        return View();
    }
}
