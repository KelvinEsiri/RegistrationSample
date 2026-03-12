using System.ComponentModel.DataAnnotations;

namespace RegistrationSample.Web.Models;

public class EditProfileViewModel
{
    [Required] public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    [Required] public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    [Url] public string ProfilePictureUrl { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    // Professional
    public string Occupation { get; set; } = string.Empty;
    public string Employer { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    [Url] public string LinkedInUrl { get; set; } = string.Empty;
    // Academic
    public string Institution { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string GradePointAverage { get; set; } = string.Empty;
    public string Certifications { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
}
