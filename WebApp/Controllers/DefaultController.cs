using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class DefaultController(HttpClient httpClient) : Controller
{

    private readonly HttpClient _httpClient = httpClient;

    public IActionResult Home()
    {
        return View();
    }

    [Route("/error")]
    public IActionResult Error404(int statusCode)
    {
        return View();
    }

    [Route("/denied")]
    public IActionResult AccessDenied(int statusCode)
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(SubscribeViewModel model)
    {
        if (ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("https://localhost:7223/api/subscribe", content);

            if(response.IsSuccessStatusCode)
            {
                TempData["StatusMessage"] = "You are now subscribed";
            }

            else if(response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["StatusMessage"] = "You are already subscribed";
            }

            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                TempData["StatusMessage"] = "Unauthorized";
            }
        }

        else
        {
            TempData["StatusMessage"] = "Invalid email address";
        }

        return RedirectToAction("Home","Default","subscribe");
    }
}
