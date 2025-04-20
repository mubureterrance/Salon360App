using System.ComponentModel.DataAnnotations;
using Salon360App.Enums;
using Salon360App.Models;

namespace Salon360App.ViewModels.AccountViewModels
{
    public class RegisterStaffViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public Gender Gender { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Profile Image URL")]
        public string ProfileImageUrl { get; set; }

        [Required]
        [Display(Name = "Staff Role")]
        public int StaffRoleId { get; set; }

        [Display(Name = "Bio")]
        public string Bio { get; set; }

        public IEnumerable<StaffRole> AvailableRoles { get; set; } = new List<StaffRole>();
    }
}