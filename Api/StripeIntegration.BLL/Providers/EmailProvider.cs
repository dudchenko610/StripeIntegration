using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using StripeIntegration.ViewModels.Models.Email;
using StripeIntegration.ViewModels.Options;

namespace StripeIntegration.BLL.Providers;

public class EmailProvider
{
    private readonly EmailConnectionOptions _mailSettings;

    public EmailProvider(IOptions<EmailConnectionOptions> connectionOptions)
    {
        _mailSettings = connectionOptions.Value;
    }
    
    public async Task<bool> SendMailAsync(MailDataModel mailData)
    {
        try
        {
            using (var emailMessage = new MimeMessage())
            {
                var emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);
                var emailTo = new MailboxAddress(mailData.EmailToName, mailData.EmailToId);
                emailMessage.To.Add(emailTo);

                // emailMessage.Cc.Add(new MailboxAddress("Cc Receiver", "cc@example.com"));
                // emailMessage.Bcc.Add(new MailboxAddress("Bcc Receiver", "bcc@example.com"));

                emailMessage.Subject = mailData.EmailSubject;

                var emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = mailData.EmailBody;

                emailMessage.Body = emailBodyBuilder.ToMessageBody();
                //this is the SmtpClient from the Mailkit.Net.Smtp namespace, not the System.Net.Mail one
                using (var mailClient = new SmtpClient())
                {
                    await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.Auto);
                    await mailClient.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                    await mailClient.SendAsync(emailMessage);
                    await mailClient.DisconnectAsync(true);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}