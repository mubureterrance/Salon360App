using System.ComponentModel.DataAnnotations;

namespace Salon360App.Enums
{
    public enum UserType
    {
        [Display(Name = "Customer")]
        Customer = 1,

        [Display(Name = "Staff")]
        Staff = 2,

        [Display(Name = "Admin")]
        Admin = 3
    }
}
