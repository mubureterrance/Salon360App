using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Salon360App.Models
{
    public class BaseEntry
    {
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CreatedBy))]
        public int? CreatedById { get; set; }
        public virtual User CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(UpdatedBy))]
        public int? UpdatedById { get; set; }
        public virtual User UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        [Required]
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [NotMapped]
        public bool IsModified => UpdatedAt.HasValue;

    }
}
