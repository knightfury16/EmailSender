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

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("@no-local-part.com")]
    [InlineData("username@")]
    [InlineData("username@.com.")]
    [InlineData("user name@example.com")]
    public void InvalidEmail_Throws(string invalidEmail)
    {
        Assert.Throws<ArgumentException>(() => new EmailAddress(invalidEmail));
    }

    [Theory]
    [InlineData("John\u0001Doe")] // Start of Heading (ASCII 1)
    [InlineData("Jane\u0009Doe")] // Horizontal tab (ASCII 9)
    [InlineData("\u001FMaxwell")] // Unit Separator (ASCII 31)
    [InlineData("Anna\nSmith")] // Line Feed (ASCII 10)
    [InlineData("Bob\rJones")] // Carriage Return (ASCII 13)
    public void DisplayName_WithControlCharacters_ThrowsArgumentException(string invalidDisplayName)
    {
        // Arrange
        var email = "user@example.com";

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => new EmailAddress(email, invalidDisplayName)
        );

        Assert.Equal("Display name cannot contain control characters.", ex.Message);
    }
}
