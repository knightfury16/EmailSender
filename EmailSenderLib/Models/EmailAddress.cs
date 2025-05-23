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

    private static string ValidateEmail(string address)
    {
        throw new NotImplementedException();
    }

    private static string? ValidateDisplayName(string? displayName)
    {
        throw new NotImplementedException();
    }
}
