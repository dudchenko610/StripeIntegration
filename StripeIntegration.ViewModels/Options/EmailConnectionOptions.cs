namespace StripeIntegration.ViewModels.Options;

public class EmailConnectionOptions
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    
    public string EmailConfirmation { get; set; }
    public string PasswordRestore { get; set; }
}