using Salon360App.Enums;

namespace Salon360App.ViewModels.StaffViewModels
{
    public class StaffDetailsViewModel
    {
        public int StaffId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public string? Bio { get; set; }
        public DateTime HireDate { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; }
    }
}
