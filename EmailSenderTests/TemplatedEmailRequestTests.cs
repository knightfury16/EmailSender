using EmailSenderLib.Models;

namespace EmailSenderTests;

public class TemplatedEmailRequestTests
{
    [Fact]
    public void TemplateId_CannotBeNull()
    {
        var request = new TemplatedEmailRequest(new List<EmailAddress> { new("a@example.com") });
        Assert.Throws<ArgumentNullException>(() => request.TemplateId = null!);
    }

    [Fact]
    public void SetTemplate_SetsProperties()
    {
        var request = new TemplatedEmailRequest(new List<EmailAddress> { new("a@example.com") });
        request.SetTemplate("template1", new { Name = "Alice" });
        Assert.Equal("template1", request.TemplateId);
        Assert.NotNull(request.TemplateContent);
    }
}
