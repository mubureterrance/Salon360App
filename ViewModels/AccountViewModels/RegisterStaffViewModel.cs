using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Salon360App.Enums;
using Salon360App.Models;

namespace Salon360App.ViewModels.AccountViewModels
{
    public class RegisterStaffViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Please select a gender")]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date of birth is required")]
        [CustomValidation(typeof(RegisterStaffViewModel), "ValidateDateOfBirth")]
        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-18);

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile ProfileImage { get; set; }

        [Required(ErrorMessage = "Please select a staff role")]
        [Display(Name = "Staff Role")]
        public int StaffRoleId { get; set; }

        [Display(Name = "Bio")]
        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string Bio { get; set; }

        public IEnumerable<SelectListItem>? AvailableRoles { get; set; }

        public static ValidationResult ValidateDateOfBirth(DateTime dateOfBirth, ValidationContext context)
        {
            var minimumAge = 18;
            var maximumAge = 100;
            var age = DateTime.UtcNow.Year - dateOfBirth.Year;

            if (DateTime.UtcNow < dateOfBirth.AddYears(age))
                age--;

            if (age < minimumAge)
                return new ValidationResult($"Staff member must be at least {minimumAge} years old");

            if (age > maximumAge)
                return new ValidationResult($"Staff member cannot be older than {maximumAge} years");

            return ValidationResult.Success;
        }
    }
}