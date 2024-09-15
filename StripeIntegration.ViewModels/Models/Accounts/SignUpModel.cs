namespace StripeIntegration.ViewModels.Models.Accounts;

public class SignUpModel
{
    public string? Nickname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool SocialSignUp { get; set; } // TODO: remove
}