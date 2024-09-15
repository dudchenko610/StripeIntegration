namespace StripeIntegration.ViewModels.Models.Accounts;

public class SocialSignInModel
{
    public string Email { get; set; } = string.Empty;
    public TokenModel TokenModel { get; set; } = null!;
}