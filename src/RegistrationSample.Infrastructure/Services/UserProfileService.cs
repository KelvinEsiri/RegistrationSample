using Microsoft.AspNetCore.Identity;
using RegistrationSample.Application.DTOs;
using RegistrationSample.Application.Interfaces;
using RegistrationSample.Domain.Entities;
using RegistrationSample.Domain.Interfaces;

namespace RegistrationSample.Infrastructure.Services;

public class UserProfileService : IUserProfileService
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _emailService;

    public UserProfileService(UserManager<User> userManager, IEmailService emailService)
    {
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<UserProfileDto> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        return MapToDto(user);
    }

    public async Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.DateOfBirth = dto.DateOfBirth;
        user.Gender = dto.Gender;
        user.PhoneNumber = dto.Phone;
        user.Address = dto.Address;
        user.City = dto.City;
        user.State = dto.State;
        user.Country = dto.Country;
        user.PostalCode = dto.PostalCode;
        user.Institution = dto.Institution;
        user.Degree = dto.Degree;
        user.FieldOfStudy = dto.FieldOfStudy;
        user.GraduationYear = dto.GraduationYear;
        user.GradePointAverage = dto.GradePointAverage;
        user.Certifications = dto.Certifications;
        user.StudentId = dto.StudentId;
        user.AcademicYear = dto.AcademicYear;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        await _emailService.SendProfileUpdateEmailAsync(user.Email!, $"{user.FirstName} {user.LastName}");

        return MapToDto(user);
    }

    private static UserProfileDto MapToDto(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email!,
        DateOfBirth = user.DateOfBirth,
        Gender = user.Gender,
        Phone = user.PhoneNumber ?? string.Empty,
        Address = user.Address,
        City = user.City,
        State = user.State,
        Country = user.Country,
        PostalCode = user.PostalCode,
        ProfilePictureUrl = user.ProfilePictureUrl,
        Institution = user.Institution,
        Degree = user.Degree,
        FieldOfStudy = user.FieldOfStudy,
        GraduationYear = user.GraduationYear,
        GradePointAverage = user.GradePointAverage,
        Certifications = user.Certifications,
        StudentId = user.StudentId,
        AcademicYear = user.AcademicYear,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };
}
