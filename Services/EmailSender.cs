using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Threading.Tasks;
using test123.Settings;
namespace test123.Services;

public class EmailSender : IEmailSender
{
    private readonly IOptions<EmailSettings> _emailSettings;

    public EmailSender(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings;
    }
   

public async Task<bool> SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = message;
                mail.From = new MailAddress(_emailSettings.Value.Name);
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient(_emailSettings.Value.Host, _emailSettings.Value.Port);
                smtp.Credentials = new System.Net.NetworkCredential(_emailSettings.Value.Name, _emailSettings.Value.Password);
                smtp.EnableSsl = _emailSettings.Value.SSL;
                smtp.Send(mail);
                return  true;
            }
        }
        catch (Exception ae)
        {
            return false;
        }
    }
   
}
