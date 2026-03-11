using RegistrationSample.Application.DTOs;

namespace RegistrationSample.Application.Interfaces;

public interface IUserProfileService
{
    Task<UserProfileDto> GetProfileAsync(string userId);
    Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
}
