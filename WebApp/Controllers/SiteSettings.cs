using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class SiteSettings : Controller
{
    public IActionResult ChangeTheme(string mode)
    {
        // Create CookieOptions object with SameSite attribute set to Strict
        var option = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(60),
            SameSite = SameSiteMode.Strict  // Set SameSite attribute to Strict
        };

        // Append the "ThemeMode" cookie to the response with the specified options
        Response.Cookies.Append("ThemeMode", mode, option);

        // Return Ok result indicating success
        return Ok();
    }


    [HttpPost]
    public IActionResult CookieConsent()
    {
        var option = new CookieOptions
        {
            Expires = DateTime.Now.AddYears(1),
            HttpOnly = true,
            Secure = true
        };

        Response.Cookies.Append("CookieConsent", "true", option);
        return Ok();
    }
}
