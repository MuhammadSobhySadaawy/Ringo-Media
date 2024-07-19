using SendGrid;
using System.Threading.Tasks;
namespace test123.Services;

public interface IEmailSender
{
    Task<bool> SendEmailAsync(string email, string subject, string message);
}
