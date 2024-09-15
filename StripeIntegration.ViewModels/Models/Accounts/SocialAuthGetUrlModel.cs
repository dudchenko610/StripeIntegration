namespace StripeIntegration.ViewModels.Models.Accounts;

public class SocialAuthGetUrlModel
{
    public required string Provider { get; set; }
    public required string ReturnUrl { get; set; }
}