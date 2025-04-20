using System.ComponentModel.DataAnnotations;

namespace Salon360App.Enums
{
    public enum DefaultCustomerType
    {
        [Display(Name = "Walk-In")]
        WalkIn = 1,

        [Display(Name = "Regular")]
        Regular = 2,

        [Display(Name = "VIP Client")]
        VIP = 3,

        [Display(Name = "Subscriber")]
        Subscriber = 4,

        [Display(Name = "Corporate Client")]
        Corporate = 5
    }
}
