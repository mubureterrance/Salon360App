using Salon360App.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Salon360App.Models
{
    public class StaffRole : BaseEntry
    {
        [Key]
        public int StaffRoleId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public decimal? BaseRate { get; set; }

        public virtual ICollection<Staff> Staffs { get; set; } = new List<Staff>();

        [StringLength(50)]
        public string? Code { get; set; } // Optional safe enum mapping

        [NotMapped]
        public DefaultStaffRole? StaffRoleEnum =>
            Enum.TryParse<DefaultStaffRole>(Code, out var role) ? role : (DefaultStaffRole?)null;
    }

}
