using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using RegistrationSample.Domain.Interfaces;

namespace RegistrationSample.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("Email");
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"] ?? "RegistrationSample", emailSettings["SenderEmail"] ?? "noreply@registrationsample.com"));
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = body };

        var smtpHost = emailSettings["Host"];
        if (string.IsNullOrWhiteSpace(smtpHost))
        {
            Console.WriteLine($"[EMAIL] To: {to} | Subject: {subject}");
            Console.WriteLine($"[EMAIL] Body: {body}");
            return;
        }

        int smtpPort = int.TryParse(emailSettings["Port"], out var parsedPort) ? parsedPort : 587;
        using var client = new SmtpClient();
        await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(emailSettings["Username"], emailSettings["Password"]);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendWelcomeEmailAsync(string to, string name)
    {
        var subject = "Welcome to RegistrationSample!";
        var body = $@"
<html><body>
<h2>Welcome, {name}!</h2>
<p>Thank you for registering with RegistrationSample. Your account has been successfully created.</p>
<p>You can now log in and view your profile.</p>
<br/>
<p>Best regards,<br/>RegistrationSample Team</p>
</body></html>";
        await SendEmailAsync(to, subject, body);
    }

    public async Task SendProfileUpdateEmailAsync(string to, string name)
    {
        var subject = "Profile Updated - RegistrationSample";
        var body = $@"
<html><body>
<h2>Hello, {name}!</h2>
<p>Your profile information has been successfully updated.</p>
<p>If you did not make this change, please contact support immediately.</p>
<br/>
<p>Best regards,<br/>RegistrationSample Team</p>
</body></html>";
        await SendEmailAsync(to, subject, body);
    }
}
