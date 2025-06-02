using EmailSenderLib.Models;

namespace EmailSenderTests;

public class TemplatedEmailRequestTests
{
    [Fact]
    public void TemplateContent_CannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(
            () =>
                new TemplatedEmailRequest(new EmailAddress("test@gmail.com"))
                {
                    TemplateContent = null!,
                }
        );
    }

    [Fact]
    public void SetTemplate_SetsProperties()
    {
        var request = new TemplatedEmailRequest(new EmailAddress("a@example.com"))
        {
            TemplateContent = "testContent",
        };
        request.SetTemplate(new { Name = "Alice" }, "template1");
        Assert.Equal("template1", request.TemplateId);
        Assert.NotNull(request.TemplateContent);
    }
}
