using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Salon360App.Models
{
    [Index(nameof(UserId))]
    public class Customer: BaseEntry
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey(nameof(CustomerType))]
        public int CustomerTypeId { get; set; }
        public virtual CustomerType CustomerType { get; set; }

        //ToDo Navigation properties

    }
}
