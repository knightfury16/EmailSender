using System.Net.Mail;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace EmailSenderLib.Models;

/// <summary>
/// Represents an email address with an optional display name.
/// </summary>
public class EmailAddress
{
    private static readonly Regex EmailValidationRegex = new(
        @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    private const int MaxEmailLength = 254; //RFC 5321 limit
    private const int MaxDisplayNameLength = 64; //RFC 5322 recommendation

    public string Adderess { get; }

    public string? DisplayName { get; }

    [JsonIgnore]
    public bool HasDisplayName => !string.IsNullOrEmpty(DisplayName);

    [JsonIgnore]
    public string Domain => Adderess.Substring(Adderess.LastIndexOf('@') + 1);

    [JsonIgnore]
    public string LocalPart => Adderess.Substring(0, Adderess.LastIndexOf('@'));

    public EmailAddress(string address, string? displayName)
    {
        // throws error if invalid
        ValidateEmail(address);
        ValidateDisplayName(displayName);

        Adderess = address;
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
            if (address == null)
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
            throw new ArgumentNullException(nameof(address), "Email Address can not be null.");
        }

        address = address.Trim();

        if (string.IsNullOrEmpty(address))
        {
            throw new ArgumentException("Email Address can not be empty");
        }

        if (address.Length > MaxEmailLength)
        {
            throw new ArgumentException(
                $"Email address can not exceed {MaxEmailLength} characters.",
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
        catch (System.FormatException ex)
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
                $"Display name can not excced {MaxDisplayNameLength} characters.",
                nameof(displayName)
            );
        }

        if (displayName.Any(char.IsControl))
        {
            throw new ArgumentException("Display name can not contain control characters.");
        }
    }

    public override string ToString()
    {
        return HasDisplayName ? $"{DisplayName} <{Adderess}>" : Adderess;
    }
}
