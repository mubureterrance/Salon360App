using Salon360App.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Salon360App.Models
{
    public class CustomerType: BaseEntry
    {
        [Key]
        public int CustomerTypeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Customer Type")]
        public string Name { get; set; } // e.g., "Regular", "VIP", "Walk In"

        [StringLength(255)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Code { get; set; } // Optional safe enum mapping

        [NotMapped]
        public DefaultCustomerType? CustomerTypes =>
            Enum.TryParse<DefaultCustomerType>(Code, out var type) ? type : (DefaultCustomerType?)null;

        // Navigation properties
        public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    }
}
