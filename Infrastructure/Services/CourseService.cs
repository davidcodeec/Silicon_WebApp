using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class CourseService(HttpClient http, IConfiguration configuration)
    {
        private readonly HttpClient _http = http;
        private readonly IConfiguration _configuration = configuration;

        public async Task<CourseResult> GetCoursesAsync(string bearerToken, string category = "", string searchQuery = "", int pageNumber = 1, int pageSize = 10, string apiKey = "")
        {
            // Set up request headers for the bearer token
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            // Construct the URL with query parameters
            var baseUrl = _configuration["ApiUrls:Courses"];
            var url = $"{baseUrl}?category={Uri.EscapeDataString(category)}&searchQuery={Uri.EscapeDataString(searchQuery)}&pageNumber={pageNumber}&pageSize={pageSize}";

            // Add API key if provided
            if (!string.IsNullOrEmpty(apiKey))
            {
                url += $"&key={apiKey}";
            }

            // Send the GET request
            var response = await _http.GetAsync(url);

            // Process the response
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<CourseResult>(await response.Content.ReadAsStringAsync());
                if (result != null && result.Succeeded)
                    return result;
            }

            return null!;
        }
    }
}
