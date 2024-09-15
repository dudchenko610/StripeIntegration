using System.ComponentModel.DataAnnotations;

namespace StripeIntegration.ViewModels.Models.Accounts;

public class ForgotPasswordModel
{
    [Required] public string? Email { get; set; }
}
