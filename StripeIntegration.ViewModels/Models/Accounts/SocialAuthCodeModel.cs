namespace StripeIntegration.ViewModels.Models.Accounts;

public class SocialAuthCodeModel
{
    // public AuthProvider Provider { get; set; }
    public string Code { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}