using Salon360App.Enums;
using System.ComponentModel.DataAnnotations;

namespace Salon360App.ViewModels.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string Firstname { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Address { get; set; }

        [Display(Name = "Profile Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        [DataType(DataType.ImageUrl)]
        public string? ProfileImageUrl { get; set; }

        public bool RememberMe { get; set; }
    }
}
