namespace RegistrationSample.Domain.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendWelcomeEmailAsync(string to, string name);
    Task SendProfileUpdateEmailAsync(string to, string name);
}
