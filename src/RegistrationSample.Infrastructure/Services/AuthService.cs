using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RegistrationSample.Application.DTOs;
using RegistrationSample.Application.Interfaces;
using RegistrationSample.Domain.Entities;
using RegistrationSample.Domain.Interfaces;

namespace RegistrationSample.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists.");

        var user = new User
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            PhoneNumber = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Country = dto.Country,
            PostalCode = dto.PostalCode,
            Institution = dto.Institution,
            Degree = dto.Degree,
            FieldOfStudy = dto.FieldOfStudy,
            GraduationYear = dto.GraduationYear,
            GradePointAverage = dto.GradePointAverage,
            Certifications = dto.Certifications,
            StudentId = dto.StudentId,
            AcademicYear = dto.AcademicYear,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _emailService.SendWelcomeEmailAsync(user.Email, $"{user.FirstName} {user.LastName}");

        return GenerateToken(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return GenerateToken(user);
    }

    private AuthResponseDto GenerateToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddHours(8);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new("fullName", $"{user.FirstName} {user.LastName}")
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: creds);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
            Email = user.Email!,
            FullName = $"{user.FirstName} {user.LastName}",
            Expiration = expiration
        };
    }
}
