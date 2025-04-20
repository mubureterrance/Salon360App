using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Salon360App.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Salon360App.Models
{
    [Index(nameof(UserType))]
    public class User : IdentityUser<int>
    {

        [Required]
        [StringLength(100)]
        [PersonalData]
        public string Firstname { get; set; }

        [Required]
        [StringLength(100)]
        [PersonalData]
        public string Lastname { get; set; }

        [Required]
        [PersonalData]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        [PersonalData]
        public string Address { get; set; }

        [StringLength(1000)]
        public string? ProfileImageUrl { get; set; }

        [Required]
        [EnumDataType(typeof(UserType))]
        public UserType UserType { get; set; } = UserType.Customer; // "Staff", "Customer", etc.

        [NotMapped]
        public string FullName => $"{Firstname?.Trim()} {Lastname?.Trim()}".Trim();

        [InverseProperty(nameof(Customer.User))]
        public virtual Customer CustomerProfile { get; set; }

        [InverseProperty(nameof(Staff.User))]
        public virtual Staff StaffProfile { get; set; }

        [DataType(DataType.Date)]
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? LastLoginDate { get; set; }

        [StringLength(50)]
        public string? LastLoginIP { get; set; }

        public int FailedLoginAttempts { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LockoutEndDate { get; set; }

        [Required]
        public bool TwoFactorEnabled { get; set; } = false;

        [StringLength(100)]
        public string? TwoFactorSecret { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastPasswordChangeDate { get; set; }

        //TODO: public virtual ICollection<LoginLog> LoginLogs { get; set; } = new List<LoginLog>(); 
        public virtual ICollection<BaseEntry> CreatedEntries { get; set; } = new List<BaseEntry>();
        public virtual ICollection<BaseEntry> UpdatedEntries { get; set; } = new List<BaseEntry>();

    }
}
