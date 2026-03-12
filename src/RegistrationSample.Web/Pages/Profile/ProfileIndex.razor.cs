using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using RegistrationSample.Web.Models;
using RegistrationSample.Web.Services;

namespace RegistrationSample.Web.Pages.Profile;

public partial class ProfileIndex
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private TokenService TokenService { get; set; } = default!;

    private UserProfileViewModel? profile;
    private bool isLoading = true;
    private string? successMessage;

    protected override async Task OnInitializedAsync()
    {
        if (!TokenService.IsAuthenticated)
        {
            Navigation.NavigateTo("/account/login");
            return;
        }

        successMessage = TokenService.ConsumeSuccessMessage();

        var response = await Http.GetAsync("/api/profile");
        if (response.IsSuccessStatusCode)
        {
            profile = await response.Content.ReadFromJsonAsync<UserProfileViewModel>();
        }
        else
        {
            await TokenService.ClearAsync();
            Navigation.NavigateTo("/account/login");
        }

        isLoading = false;
    }
}
