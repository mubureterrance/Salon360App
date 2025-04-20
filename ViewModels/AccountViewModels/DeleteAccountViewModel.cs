using System.ComponentModel.DataAnnotations;

namespace Salon360App.ViewModels.AccountViewModels
{
    public class DeleteAccountViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Account Deletion")]
        [Compare("ConfirmDeletion", ErrorMessage = "You must confirm account deletion.")]
        public string ConfirmDeletion { get; set; } = "DELETE";
    }
}