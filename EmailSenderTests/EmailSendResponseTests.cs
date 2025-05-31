using EmailSenderLib.Models;

namespace EmailSenderTests;

public class EmailSendResponseTests
{
    [Fact]
    public void Success_CreatesSuccessfulResponse()
    {
        var res = EmailSendResponse.Success("id123");
        Assert.True(res.IsSuccess);
        Assert.Equal("id123", res.MessageId);
        Assert.Null(res.ErrorMessage);
    }

    [Fact]
    public void Failure_CreatesFailedResponse()
    {
        var res = EmailSendResponse.Failure("err", "id1");
        Assert.False(res.IsSuccess);
        Assert.Equal("err", res.ErrorMessage);
        Assert.Equal("id1", res.MessageId);
    }
}
