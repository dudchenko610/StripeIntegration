namespace StripeIntegration.ViewModels.Models.Users;

public class UserModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string StripeCustomerId { get; set; } = string.Empty;
    public DateTime RegistrationTime { get; set; }
    public string TrialLicenseKey = string.Empty;
}