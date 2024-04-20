using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, ApplicationContext context, HttpClient http, IConfiguration configuration) : Controller
{

    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly ApplicationContext _context = context;
    private readonly HttpClient _http = http;
    private readonly IConfiguration _configuration = configuration;



    #region Individual Account | Sign Up
    [Route("/signup")]
    public IActionResult SignUp()
    {
        return View();
    }

    [Route("/signup")]
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel model)
    {
        var standardRole = "User";

        if (ModelState.IsValid)
        {
            if (!await _context.Users.AnyAsync(x => x.Email == model.Email))
            {
                standardRole = await _context.Users.AnyAsync() ? "User" : "SuperAdmin"; // Check if any user exists in the database

                var userEntity = new UserEntity
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                if ((await _userManager.CreateAsync(userEntity, model.Password)).Succeeded)
                {
                    if ((await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false)).Succeeded)
                    {
                        await _userManager.AddToRoleAsync(userEntity, standardRole);
                        return LocalRedirect("/");
                    }
                    else
                    {
                        return LocalRedirect("/signin");
                    }
                }
                else
                {
                    ViewData["StatusMessage"] = "Something went wrong. Try again later or contact customer service.";
                }
            }
            else
            {
                ViewData["StatusMessage"] = "User with the same email already exists.";
            }
        }
        return View(model);
    }


    #endregion



    #region Individual Account | Sign In

    [Route("/signin")]
    public IActionResult SignIn(string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl ?? "/";
        return View();
    }


    [Route("/signin")]
    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl)
    {
        if (ModelState.IsValid)
        {
            if ((await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.IsPersistent, false)).Succeeded)
            {

                var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                
                var response = await _http.PostAsync($"https://localhost:7223/api/Auth/token?key={_configuration["ApiKey:Secret"]}", content);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.Now.AddDays(1)
                    };

                    Response.Cookies.Append("AccessToken", token, cookieOptions);
                }

                return LocalRedirect(returnUrl);
            }

        }

        ViewData["ReturnUrl"] = returnUrl;
        ViewData["StatusMessage"] = "Incorrect email or password";
        return View(model);
    }


    #endregion


    #region Individual Account | Sign Out

    [Route("/signout")]
    public new async Task<IActionResult> SignOut()
    {
        Response.Cookies.Delete("AccessToken");
        await _signInManager.SignOutAsync();
        return RedirectToAction("Home", "Default");
    }

    #endregion


    #region External Account | Facebook

    [HttpGet]

    public IActionResult Facebook()
    {
        var authProps = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", Url.Action("FacebookCallback"));
        return new ChallengeResult("Facebook", authProps);
    }


    [HttpGet]

    public async Task<IActionResult> FacebookCallback()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info != null)
        {
            var userEntity = new UserEntity
            {
                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!,
                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)!,
                Email = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email)!,
                IsExternalAccount = true,
            };

            var user = await _userManager.FindByEmailAsync(userEntity.Email);
            if (user == null)
            {
                var result = await _userManager.CreateAsync(userEntity);
                if (result.Succeeded)
                    user = await _userManager.FindByEmailAsync(userEntity.Email);

            }

            if (user != null)
            {
                if (user.FirstName != userEntity.FirstName || user.LastName != userEntity.LastName || user.Email != userEntity.Email)
                {

                    user.FirstName = userEntity.FirstName;
                    user.LastName = userEntity.LastName;
                    user.Email = userEntity.Email;

                    await _userManager.UpdateAsync(user);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);

                if (HttpContext.User != null)
                    return RedirectToAction("Details", "Account");
            }
        }

        ModelState.AddModelError("InvalidFacebookAuthentication", "danger|Failed to authenticate with Facebook.");
        ViewData["StatusMessage"] = "danger|Failed to authenticate with Facebook.";
        return RedirectToAction("SignIn", "Auth");
    }

    #endregion
}
