using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Salon360App.Models
{
    [Index(nameof(UserId))]
    public class Staff: BaseEntry
    {
        [Key]
        public int StaffId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required]
        [ForeignKey(nameof(StaffRole))]
        public int StaffRoleId { get; set; }
        public virtual StaffRole StaffRole { get; set; }

        [StringLength(255)]
        public string? Bio { get; set; }

        [Required]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; } = DateTime.UtcNow;

    }
}
