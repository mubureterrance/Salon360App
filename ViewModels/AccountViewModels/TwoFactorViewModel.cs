namespace Salon360App.ViewModels.AccountViewModels
{
    public class TwoFactorViewModel
    {
        public bool HasAuthenticator { get; set; }
        public bool Is2faEnabled { get; set; }
        public int RecoveryCodesLeft { get; set; }
    }
}