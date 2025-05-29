using System.Net.Mail;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace EmailSenderLib.Models;

/// <summary>
/// Represents an email address with an optional display name.
/// </summary>
public class EmailAddress : IEquatable<EmailAddress>
{
    private static readonly Regex EmailValidationRegex = new(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    private const int MaxEmailLength = 254; // RFC 5321 limit
    private const int MaxDisplayNameLength = 64; // RFC 5322 recommendation

    public string Address { get; }
    public string? DisplayName { get; }

    [JsonIgnore]
    public bool HasDisplayName => !string.IsNullOrEmpty(DisplayName);

    [JsonIgnore]
    public string Domain => Address.Substring(Address.LastIndexOf('@') + 1);

    [JsonIgnore]
    public string LocalPart => Address.Substring(0, Address.LastIndexOf('@'));

    public EmailAddress(string address, string? displayName)
    {
        // throws error if invalid
        ValidateEmail(address);
        ValidateDisplayName(displayName);

        Address = address.ToLowerInvariant(); // Normalize email addresses to lowercase
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
    }

    public EmailAddress(string address)
        : this(address, null) { }

    public static bool TryCreate(
        string? address,
        string? displayName,
        out EmailAddress? emailAddress
    )
    {
        emailAddress = null;

        try
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;

            emailAddress = new EmailAddress(address, displayName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryCreate(string? address, out EmailAddress? emailAddress)
    {
        return TryCreate(address, null, out emailAddress);
    }

    private void ValidateEmail(string address)
    {
        if (address == null)
        {
            throw new ArgumentNullException(nameof(address), "Email address cannot be null.");
        }

        address = address.Trim();

        if (string.IsNullOrEmpty(address))
        {
            throw new ArgumentException("Email address cannot be empty.", nameof(address));
        }

        if (address.Length > MaxEmailLength)
        {
            throw new ArgumentException(
                $"Email address cannot exceed {MaxEmailLength} characters.",
                nameof(address)
            );
        }

        if (!EmailValidationRegex.IsMatch(address))
        {
            throw new ArgumentException("Invalid email address format.", nameof(address));
        }

        // let the big boy validate email now
        try
        {
            var _ = new MailAddress(address);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("Invalid email address format.", nameof(address), ex);
        }
    }

    private void ValidateDisplayName(string? displayName)
    {
        if (string.IsNullOrEmpty(displayName))
            return;

        displayName = displayName.Trim();

        if (displayName.Length > MaxDisplayNameLength)
        {
            throw new ArgumentException(
                $"Display name cannot exceed {MaxDisplayNameLength} characters.",
                nameof(displayName)
            );
        }

        if (displayName.Any(char.IsControl))
        {
            throw new ArgumentException("Display name cannot contain control characters.");
        }
    }

    public override string ToString()
    {
        return HasDisplayName ? $"{DisplayName} <{Address}>" : Address;
    }

    public string ToAddressString()
    {
        return Address;
    }

    #region Equality and Comparision

    public bool Equals(EmailAddress? other)
    {
        if (other == null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return string.Equals(this.Address, other.Address, StringComparison.OrdinalIgnoreCase)
            && string.Equals(
                this.DisplayName,
                other.DisplayName,
                StringComparison.OrdinalIgnoreCase
            );
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as EmailAddress);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Address.ToLowerInvariant(), DisplayName);
    }

    public static bool operator ==(EmailAddress? left, EmailAddress? right)
    {
        return EqualityComparer<EmailAddress>.Default.Equals(left, right);
    }

    public static bool operator !=(EmailAddress? left, EmailAddress? right)
    {
        return !(left == right);
    }
    #endregion

    #region Internal Conversion (for framework use only)
    internal System.Net.Mail.MailAddress ToSystemMail()
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
    #endregion
}
