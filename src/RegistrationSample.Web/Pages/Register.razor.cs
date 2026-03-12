using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Text.Json;
using RegistrationSample.Web.Models;
using RegistrationSample.Web.Services;

namespace RegistrationSample.Web.Pages;

public partial class Register
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private TokenService TokenService { get; set; } = default!;

    private RegisterViewModel model = new();
    private string? errorMessage;
    private bool isLoading;

    private async Task HandleRegister()
    {
        isLoading = true;
        errorMessage = null;

        var response = await Http.PostAsJsonAsync("/api/auth/register", model);
        if (response.IsSuccessStatusCode)
        {
            var auth = await response.Content.ReadFromJsonAsync<AuthResponseViewModel>();
            await TokenService.SetAuthAsync(auth!.Token, auth.FullName, auth.UserId);
            Navigation.NavigateTo("/profile");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            try
            {
                var errObj = JsonDocument.Parse(error);
                errorMessage = errObj.RootElement.GetProperty("message").GetString() ?? "Registration failed.";
            }
            catch { errorMessage = "Registration failed. Please try again."; }
        }

        isLoading = false;
    }
}
