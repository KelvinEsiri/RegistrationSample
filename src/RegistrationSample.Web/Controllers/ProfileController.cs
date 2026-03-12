using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RegistrationSample.Web.Models;

namespace RegistrationSample.Web.Controllers;

public class ProfileController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ProfileController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private string? GetToken() => HttpContext.Session.GetString("Token");

    private IActionResult? RedirectIfNotAuthenticated()
    {
        if (string.IsNullOrEmpty(GetToken()))
            return RedirectToAction("Login", "Account");
        return null;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var redirect = RedirectIfNotAuthenticated();
        if (redirect != null) return redirect;

        var client = _httpClientFactory.CreateClient("API");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

        var response = await client.GetAsync("/api/profile");
        if (!response.IsSuccessStatusCode)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        var body = await response.Content.ReadAsStringAsync();
        var profile = JsonSerializer.Deserialize<UserProfileViewModel>(body, _jsonOptions);
        return View(profile);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var redirect = RedirectIfNotAuthenticated();
        if (redirect != null) return redirect;

        var client = _httpClientFactory.CreateClient("API");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

        var response = await client.GetAsync("/api/profile");
        if (!response.IsSuccessStatusCode)
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        var body = await response.Content.ReadAsStringAsync();
        var profile = JsonSerializer.Deserialize<UserProfileViewModel>(body, _jsonOptions);

        var updateDto = new EditProfileViewModel
        {
            FirstName = profile!.FirstName,
            MiddleName = profile.MiddleName,
            LastName = profile.LastName,
            DateOfBirth = profile.DateOfBirth,
            Gender = profile.Gender,
            MaritalStatus = profile.MaritalStatus,
            Phone = profile.Phone,
            ProfilePictureUrl = profile.ProfilePictureUrl,
            Address = profile.Address,
            City = profile.City,
            State = profile.State,
            Country = profile.Country,
            PostalCode = profile.PostalCode,
            Occupation = profile.Occupation,
            Employer = profile.Employer,
            YearsOfExperience = profile.YearsOfExperience,
            LinkedInUrl = profile.LinkedInUrl,
            Institution = profile.Institution,
            Degree = profile.Degree,
            FieldOfStudy = profile.FieldOfStudy,
            GraduationYear = profile.GraduationYear,
            GradePointAverage = profile.GradePointAverage,
            Certifications = profile.Certifications,
            StudentId = profile.StudentId,
            AcademicYear = profile.AcademicYear
        };

        return View(updateDto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        var redirect = RedirectIfNotAuthenticated();
        if (redirect != null) return redirect;

        if (!ModelState.IsValid) return View(model);

        var client = _httpClientFactory.CreateClient("API");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

        var json = JsonSerializer.Serialize(model);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PutAsync("/api/profile", content);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }

        var error = await response.Content.ReadAsStringAsync();
        try
        {
            var errObj = JsonSerializer.Deserialize<JsonElement>(error, _jsonOptions);
            ModelState.AddModelError("", errObj.GetProperty("message").GetString() ?? "Update failed.");
        }
        catch { ModelState.AddModelError("", "Update failed. Please try again."); }

        return View(model);
    }
}
