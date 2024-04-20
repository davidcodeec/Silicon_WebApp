using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly CategoryService _categoryService;
        private readonly CourseService _courseService;
        private readonly IConfiguration _configuration;

        public CoursesController(CategoryService categoryService, CourseService courseService, IConfiguration configuration)
        {
            _categoryService = categoryService;
            _courseService = courseService;
            _configuration = configuration;
        }

        [Route("/courses")]
        public async Task<IActionResult> Index(string category = "", string searchQuery = "", int pageNumber = 1, int pageSize = 6)
        {
            try
            {
                // Retrieve the bearer token from the cookie
                if (HttpContext.Request.Cookies.TryGetValue("AccessToken", out var token))
                {
                    // Retrieve the API key from the configuration
                    var apiKey = _configuration["ApiKey:Secret"] ?? ""; // Providing a non-null default value
                    var courseResult = await _courseService.GetCoursesAsync(token, category, searchQuery, pageNumber, pageSize, apiKey);

                    // Construct the view model
                    var viewModel = new CourseIndexViewModel
                    {
                        Categories = await _categoryService.GetCategoriesAsync(),
                        Courses = courseResult?.Courses ?? Enumerable.Empty<Course>(),
                        Pagination = new Pagination
                        {
                            PageSize = pageSize,
                            CurrentPage = pageNumber,
                            TotalPages = courseResult?.TotalPages ?? 0,
                            TotalItems = courseResult?.TotalItems ?? 0
                        }
                    };

                    return View(viewModel);
                }
                else
                {
                    // Handle the case where the bearer token is not found
                    return RedirectToAction("Login", "Account");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the request
                Debug.WriteLine($"Error occurred: {ex.Message}");
                return RedirectToAction("Error404", "Default");
            }
        }
    }
}
