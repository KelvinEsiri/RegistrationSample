using Microsoft.JSInterop;

namespace RegistrationSample.Web.Services;

public class TokenService
{
    private readonly IJSRuntime _js;

    public TokenService(IJSRuntime js)
    {
        _js = js;
    }

    public string? Token { get; private set; }
    public string? FullName { get; private set; }
    public string? UserId { get; private set; }
    public string? PendingSuccessMessage { get; private set; }

    public event Action? OnChange;

    public async Task InitializeAsync()
    {
        Token = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_token");
        FullName = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_fullname");
        UserId = await _js.InvokeAsync<string?>("localStorage.getItem", "auth_userid");
    }

    public async Task SetAuthAsync(string token, string fullName, string userId)
    {
        Token = token;
        FullName = fullName;
        UserId = userId;
        await _js.InvokeVoidAsync("localStorage.setItem", "auth_token", token);
        await _js.InvokeVoidAsync("localStorage.setItem", "auth_fullname", fullName);
        await _js.InvokeVoidAsync("localStorage.setItem", "auth_userid", userId);
        OnChange?.Invoke();
    }

    public async Task ClearAsync()
    {
        Token = null;
        FullName = null;
        UserId = null;
        await _js.InvokeVoidAsync("localStorage.removeItem", "auth_token");
        await _js.InvokeVoidAsync("localStorage.removeItem", "auth_fullname");
        await _js.InvokeVoidAsync("localStorage.removeItem", "auth_userid");
        OnChange?.Invoke();
    }

    public void SetSuccessMessage(string message) => PendingSuccessMessage = message;

    public string? ConsumeSuccessMessage()
    {
        var msg = PendingSuccessMessage;
        PendingSuccessMessage = null;
        return msg;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
}
