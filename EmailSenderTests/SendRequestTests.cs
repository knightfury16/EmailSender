using EmailSenderLib.Models;

namespace EmailSenderTests;

public class SendRequestTests
{
    private static EmailAddress CreateAddress(int i) => new($"user{i}@example.com", $"User{i}");

    [Fact]
    public void AddRecipients_DuplicatesIgnored()
    {
        var request = new EmailRequest(new List<EmailAddress>());
        request.AddTo(CreateAddress(1));
        request.AddTo(CreateAddress(1));
        Assert.Single(request.To);
    }

    [Fact]
    public void AddTo_ExceedLimit_Throws()
    {
        var request = new EmailRequest(new List<EmailAddress>());
        for (int i = 0; i < 100; i++)
        {
            request.AddTo(CreateAddress(i));
        }
        Assert.Throws<InvalidOperationException>(() => request.AddTo(CreateAddress(101)));
    }

    [Fact]
    public void Validate_DuplicateAcrossCollections_Throws()
    {
        var addr = CreateAddress(1);
        var request = new EmailRequest(new List<EmailAddress> { addr });
        request.AddCc(addr);
        request.Subject = "Test";
        request.TextContent = "text";
        Assert.Throws<InvalidOperationException>(() => request.Validate());
    }

    [Fact]
    public void Validate_NoRecipients_Throws()
    {
        var request = new EmailRequest(new List<EmailAddress>());
        request.Subject = "Test";
        request.TextContent = "text";
        Assert.Throws<InvalidOperationException>(() => request.Validate());
    }

    [Fact]
    public void AddHeader_TrimsValues()
    {
        var request = new EmailRequest(new List<EmailAddress> { CreateAddress(1) });
        request.Subject = "Test";
        request.TextContent = "content";
        request.AddHeader(" X ", " val ");
        Assert.True(request.Headers.ContainsKey("X"));
        Assert.Equal("val", request.Headers["X"]);
    }
}
