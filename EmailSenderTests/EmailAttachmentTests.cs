using EmailSenderLib.Models;

namespace EmailSenderTests;

public class EmailAttachmentTests
{
    [Fact]
    public void FromByte_CreatesAttachment()
    {
        var data = new byte[] { 1, 2, 3 };
        var attachment = EmailAttachment.FromByte(data, "test.txt", "text/plain");
        Assert.Equal("test.txt", attachment.FileName);
        Assert.Equal("text/plain", attachment.MimeType);
        Assert.False(attachment.IsInline);

        //check content
        using var memoryStream = new MemoryStream();
        attachment.Content.CopyTo(memoryStream);
        var contentBytes = memoryStream.ToArray();
        Assert.Equal(data, contentBytes);
    }

    [Fact]
    public void InlineAttachmentWithoutContentId_Throws()
    {
        var data = new byte[] { 1 };
        Assert.Throws<ArgumentException>(
            () => EmailAttachment.FromByte(data, "a.txt", isInline: true)
        );
    }

    [Fact]
    public void FromByte_TooLarge_Throws()
    {
        var big = new byte[25 * 1024 * 1024 + 1];
        Assert.Throws<InvalidOperationException>(() => EmailAttachment.FromByte(big, "big.txt"));
    }
}
