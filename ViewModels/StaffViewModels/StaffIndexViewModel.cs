namespace Salon360App.ViewModels.StaffViewModels
{
    public class StaffIndexViewModel
    {
        public int StaffId { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public string RoleName { get; set; }

        public DateTime HireDate { get; set; }

        public string? Bio { get; set; }

        public bool IsActive { get; set; } 
    }
}
