namespace StripeIntegration.ViewModels.Models.Accounts;

public class ChangePasswordModel
{
    public required string Code { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}
