using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using RegistrationSample.Web.Models;
using RegistrationSample.Web.Services;

namespace RegistrationSample.Web.Pages;

public partial class Login
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private TokenService TokenService { get; set; } = default!;

    private LoginViewModel model = new();
    private string? errorMessage;
    private bool isLoading;

    protected override void OnInitialized()
    {
        if (TokenService.IsAuthenticated)
            Navigation.NavigateTo("/profile");
    }

    private async Task HandleLogin()
    {
        isLoading = true;
        errorMessage = null;

        var response = await Http.PostAsJsonAsync("/api/auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            var auth = await response.Content.ReadFromJsonAsync<AuthResponseViewModel>();
            await TokenService.SetAuthAsync(auth!.Token, auth.FullName, auth.UserId);
            Navigation.NavigateTo("/profile");
        }
        else
        {
            errorMessage = "Invalid email or password.";
        }

        isLoading = false;
    }
}
