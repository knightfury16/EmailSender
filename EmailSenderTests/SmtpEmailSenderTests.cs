using EmailSenderLib.Models;
using EmailSenderLib.SmtpEmailSender;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace EmailSenderTests;

public class SmtpEmailSenderTests
{
    [Fact]
    public async Task MissingSmtpServer_ReturnsFailure()
    {
        var settings = new SmtpEmailSettings
        {
            UserName = "user",
            Password = "pass",
            SenderEmail = "sender@example.com"
        };
        var options = Options.Create(settings);
        var sender = new SmtpEmailSender(options, NullLogger<SmtpEmailSettings>.Instance);
        var request = new EmailSendRequest(new EmailAddress("to@example.com"))
        {
            Subject = "Sub",
            TextContent = "body"
        };

        var response = await sender.SendEmailAsync(request);
        Assert.True(response.IsFailure);
    }
}
