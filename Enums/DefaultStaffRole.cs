using System.ComponentModel.DataAnnotations;

namespace Salon360App.Enums
{
    public enum DefaultStaffRole
    {
        [Display(Name = "Stylist")]
        Stylist = 1,

        [Display(Name = "Barber")]
        Barber = 2,

        [Display(Name = "Nail Technician")]
        NailTech = 3,

        [Display(Name = "Masseuse")]
        Masseuse = 4,

        [Display(Name = "Receptionist")]
        Receptionist = 5,

        [Display(Name = "Manager")]
        Manager = 6,

        [Display(Name = "Owner")]
        Owner = 7,

        [Display(Name = "Administrator")]
        Admin = 8
    }
}
