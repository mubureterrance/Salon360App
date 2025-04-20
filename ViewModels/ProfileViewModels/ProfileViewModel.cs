using Salon360App.Enums;
using System.ComponentModel.DataAnnotations;

namespace Salon360App.ViewModels.ProfileViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

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

        [Display(Name = "Profile Image")]
        [Url(ErrorMessage = "Please Select image")]
        [DataType(DataType.ImageUrl)]
        public string? ProfileImageUrl { get; set; }


        public UserType UserType { get; set; }
        public string? CustomerTypeName { get; set; }
        public string? StaffRoleName { get; set; }


    }
}
