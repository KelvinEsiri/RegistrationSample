using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RegistrationSample.Application.DTOs;

namespace RegistrationSample.Web.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/auth/register", content);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var auth = JsonSerializer.Deserialize<AuthResponseDto>(responseBody, _jsonOptions);
            HttpContext.Session.SetString("Token", auth!.Token);
            HttpContext.Session.SetString("UserId", auth.UserId);
            HttpContext.Session.SetString("FullName", auth.FullName);
            return RedirectToAction("Index", "Profile");
        }

        var error = await response.Content.ReadAsStringAsync();
        try
        {
            var errObj = JsonSerializer.Deserialize<JsonElement>(error, _jsonOptions);
            ModelState.AddModelError("", errObj.GetProperty("message").GetString() ?? "Registration failed.");
        }
        catch { ModelState.AddModelError("", "Registration failed. Please try again."); }

        return View(model);
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("API");
        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/api/auth/login", content);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var auth = JsonSerializer.Deserialize<AuthResponseDto>(responseBody, _jsonOptions);
            HttpContext.Session.SetString("Token", auth!.Token);
            HttpContext.Session.SetString("UserId", auth.UserId);
            HttpContext.Session.SetString("FullName", auth.FullName);
            return RedirectToAction("Index", "Profile");
        }

        ModelState.AddModelError("", "Invalid email or password.");
        return View(model);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
