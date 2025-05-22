namespace EmailSenderLib.Models;

/// <summary>
/// Represents an email address with an optional display name.
/// </summary>
public class EmailAddress
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the display name associated with the email address.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Initializes a new instance of the EmailAddress class.
    /// </summary>
    public EmailAddress() { }

    /// <summary>
    /// Initializes a new instance of the EmailAddress class with an email address.
    /// </summary>
    /// <param name="address">The email address.</param>
    public EmailAddress(string address)
    {
        Address = address;
    }

    /// <summary>
    /// Initializes a new instance of the EmailAddress class with an email address and display name.
    /// </summary>
    /// <param name="address">The email address.</param>
    /// <param name="displayName">The display name associated with the email address.</param>
    public EmailAddress(string address, string displayName)
    {
        Address = address;
        DisplayName = displayName;
    }

    /// <summary>
    /// Converts this EmailAddress instance to a System.Net.Mail.MailAddress instance.
    /// </summary>
    /// <returns>A System.Net.Mail.MailAddress object representing the same email address and display name.</returns>
    /// <exception cref="ArgumentNullException">Thrown when Address is null.</exception>
    public System.Net.Mail.MailAddress ToSystemMailAddress()
    {
        if (string.IsNullOrEmpty(Address))
        {
            throw new ArgumentNullException(
                nameof(Address),
                "Email address cannot be null or empty"
            );
        }

        return string.IsNullOrEmpty(DisplayName)
            ? new System.Net.Mail.MailAddress(Address)
            : new System.Net.Mail.MailAddress(Address, DisplayName);
    }
}
