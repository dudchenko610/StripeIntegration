namespace StripeIntegration.ViewModels.Models.Email;

public class MailDataModel
{
    public required string EmailToId { get; set; }
    public required string EmailToName { get; set; }
    public required string EmailSubject { get; set; }
    public required string EmailBody { get; set; }
}