using EmailSenderLib.Models;

namespace EmailSenderTests;

public class EmailAddressTests
{
    [Fact]
    public void Constructor_ValidArguments_SetsProperties()
    {
        var email = new EmailAddress("user@example.com", "User");
        Assert.Equal("user@example.com", email.Address);
        Assert.Equal("User", email.DisplayName);
        Assert.True(email.HasDisplayName);
        Assert.Equal("example.com", email.Domain);
        Assert.Equal("user", email.LocalPart);
    }

    [Fact]
    public void MaxDisplayNameLengthTest_throwsArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () =>
                new EmailAddress(
                    "user@example.com",
                    "JohdfjakdddhhdksjfhfhhhfhfhfhfjdafnnathanAlexanderMaximillianTheThirdOfTheGreatNorthernEmpire"
                )
        );
    }

    [Fact]
    public void TryCreate_InvalidAddress_ReturnsFalse()
    {
        var result = EmailAddress.TryCreate("invalid@", out var email);
        Assert.False(result);
        Assert.Null(email);
    }

    [Fact]
    public void ToString_WithDisplayName_ReturnsFormattedString()
    {
        var email = new EmailAddress("user@example.com", "User");
        Assert.Equal("User <user@example.com>", email.ToString());
    }
}
