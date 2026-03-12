using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;
using RegistrationSample.Web.Models;
using RegistrationSample.Web.Services;

namespace RegistrationSample.Web.Pages.Profile;

public partial class ProfileEdit
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private TokenService TokenService { get; set; } = default!;

    private EditProfileViewModel model = new();
    private bool isLoading = true;
    private bool isSubmitting;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        if (!TokenService.IsAuthenticated)
        {
            Navigation.NavigateTo("/account/login");
            return;
        }

        var response = await Http.GetAsync("/api/profile");
        if (response.IsSuccessStatusCode)
        {
            var profile = await response.Content.ReadFromJsonAsync<UserProfileViewModel>();
            if (profile != null)
            {
                model = new EditProfileViewModel
                {
                    FirstName = profile.FirstName,
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
            }
        }
        else
        {
            await TokenService.ClearAsync();
            Navigation.NavigateTo("/account/login");
        }

        isLoading = false;
    }

    private async Task HandleEdit()
    {
        isSubmitting = true;
        errorMessage = null;

        var response = await Http.PutAsJsonAsync("/api/profile", model);
        if (response.IsSuccessStatusCode)
        {
            TokenService.SetSuccessMessage("Profile updated successfully!");
            Navigation.NavigateTo("/profile");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            try
            {
                var errObj = JsonDocument.Parse(error);
                errorMessage = errObj.RootElement.GetProperty("message").GetString() ?? "Update failed.";
            }
            catch { errorMessage = "Update failed. Please try again."; }
        }

        isSubmitting = false;
    }
}
