using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var apiKey = _configuration["SendGridSettings:ApiKey"];
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("gkblackfyre@gmail.com", "leave-master");
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);

        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            throw new InvalidOperationException($"Failed to send email: {response.StatusCode}");
        }
    }

    public async Task SendEmail2Async(string toEmail, string subject, string link)
    {
        var apiKey = _configuration["SendGridSettings:ApiKey"];
        var client = new SendGridClient(apiKey);
        var msg = new SendGridMessage
        {
            From = new EmailAddress("gkblackfyre@gmail.com", "leave-master"),
            Subject = subject,
            HtmlContent = "Please reset your password by clicking the following link: " + link
        };
        msg.AddTo(new EmailAddress(toEmail));

        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            throw new InvalidOperationException($"Failed to send email: {response.StatusCode}");
        }
    }
}
