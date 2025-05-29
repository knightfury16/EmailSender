using EmailSenderLib.Models;

namespace EmailSenderTests;

public class EmailSendRequestTests
{
    [Fact]
    public void Validate_NoContent_Throws()
    {
        var request = new EmailSendRequest(new List<EmailAddress> { new("a@example.com") });
        request.Subject = "Sub";
        Assert.Throws<InvalidOperationException>(() => request.Validate());
    }

    [Fact]
    public void Validate_WithContent_Succeeds()
    {
        var request = new EmailSendRequest(new List<EmailAddress> { new("a@example.com") });
        request.Subject = "Sub";
        request.TextContent = "hello";
        request.Validate();
    }
}
