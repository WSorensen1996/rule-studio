using System.Globalization;
using System.Text.RegularExpressions;

namespace DataverseLogic.RuleStudio.Functions;

/// <summary>
/// Standard custom functions that check a value: no side effects, safe to retry. Each returns
/// Success = true with Value = whether the value is valid and a Message saying why; an empty value
/// counts as valid, so making a column required stays a separate condition.
/// </summary>
public static class ValidationFunctions
{
    private const string EmptyMessage = "Empty: nothing to check.";
    private const int MaxEmailLength = 254;
    private const int MaxLocalPartLength = 64;
    private const int MinE164Digits = 8;
    private const int MaxE164Digits = 15;

    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(200);

    private static readonly Regex LocalPart = new(@"^[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+(\.[A-Za-z0-9!#$%&'*+/=?^_`{|}~-]+)*$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture, RegexTimeout);
    private static readonly Regex DomainLabel = new("^[A-Za-z0-9]([A-Za-z0-9-]{0,61}[A-Za-z0-9])?$", RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture, RegexTimeout);
    private static readonly Regex TopLevelDomain = new("^[A-Za-z]{2,63}$", RegexOptions.CultureInvariant, RegexTimeout);

    // Calling code, national number lengths (without the trunk 0) and whether national numbers start with a trunk 0.
    private static readonly Dictionary<string, (string Code, int Min, int Max, bool Trunk)> PhonePlans = new(StringComparer.OrdinalIgnoreCase)
    {
        ["AT"] = ("43", 4, 13, true),
        ["BE"] = ("32", 8, 9, true),
        ["CA"] = ("1", 10, 10, false),
        ["CH"] = ("41", 9, 9, true),
        ["DE"] = ("49", 6, 11, true),
        ["DK"] = ("45", 8, 8, false),
        ["ES"] = ("34", 9, 9, false),
        ["FI"] = ("358", 5, 10, true),
        ["FR"] = ("33", 9, 9, true),
        ["GB"] = ("44", 9, 10, true),
        ["IE"] = ("353", 7, 9, true),
        ["IT"] = ("39", 6, 11, false),
        ["NL"] = ("31", 9, 9, true),
        ["NO"] = ("47", 8, 8, false),
        ["PL"] = ("48", 9, 9, false),
        ["SE"] = ("46", 7, 9, true),
        ["US"] = ("1", 10, 10, false),
    };

    // Checks the form of an e-mail address (not that the mailbox exists).
    public static (bool Success, bool Value, string Message) ValidateEmail(string? email)
    {
        var text = email?.Trim() ?? string.Empty;
        if (text.Length == 0)
            return (true, true, EmptyMessage);

        var problem = EmailProblem(text);
        return problem == null ? (true, true, "Valid e-mail address.") : (true, false, problem);
    }

    // Checks a phone number. With a + or 00 prefix it must be a valid international number; without
    // one, CountryCode (ISO, for example DK) says which country's numbering to use. The message gives
    // the number in international form (+4512345678).
    public static (bool Success, bool Value, string Message) ValidatePhone(string? phone, string? countryCode)
    {
        var text = phone?.Trim() ?? string.Empty;
        if (text.Length == 0)
            return (true, true, EmptyMessage);

        var compact = Regex.Replace(text, @"[\s\-.()/]", string.Empty, RegexOptions.None, RegexTimeout);
        if (compact.StartsWith("00", StringComparison.Ordinal))
        {
            compact = "+" + compact.Remove(0, 2);
        }

        var international = compact.Length > 0 && compact[0] == '+';
        var digits = international ? compact.Remove(0, 1) : compact;
        if (digits.Length == 0 || !digits.All(c => c >= '0' && c <= '9'))
            return (true, false, "A phone number may only contain digits, spaces, dashes, dots, brackets and a leading +.");

        var country = countryCode?.Trim() ?? string.Empty;
        var hasPlan = PhonePlans.TryGetValue(country, out var plan);
        if (country.Length > 0 && !hasPlan && !international)
            return (true, false, $"Country {country} is not known by this function; write the number with + and the country code.");

        if (!international)
        {
            if (!hasPlan)
                return (true, false, "Write the number with + and the country code, or give CountryCode.");

            var national = plan.Trunk && digits[0] == '0' ? digits.Remove(0, 1) : digits;
            return CheckNational(plan, national, country);
        }

        if (digits.Length < MinE164Digits || digits.Length > MaxE164Digits || digits[0] == '0')
            return (true, false, $"An international number has {MinE164Digits}–{MaxE164Digits} digits after the + and does not start with 0.");

        if (hasPlan && digits.StartsWith(plan.Code, StringComparison.Ordinal))
            return CheckNational(plan, digits.Remove(0, plan.Code.Length), country);

        if (hasPlan)
            return (true, false, $"The number does not start with +{plan.Code}, the country code of {country.ToUpperInvariant()}.");

        return (true, true, $"Valid: +{digits}");
    }

    // Checks the format of an EU, Norwegian, Swiss or UK VAT number, and its check digits where the
    // country has a public algorithm. It does not ask VIES whether the number is registered.
    public static (bool Success, bool Value, string Message) ValidateVatNumber(string? vatNumber, string? countryCode)
    {
        var compact = Regex.Replace(vatNumber?.Trim() ?? string.Empty, @"[\s\-.]", string.Empty, RegexOptions.None, RegexTimeout).ToUpperInvariant();
        if (compact.Length == 0)
            return (true, true, EmptyMessage);

        var given = countryCode?.Trim().ToUpperInvariant() ?? string.Empty;
        if (given == "GR")
        {
            given = "EL";
        }

        string country;
        string body;
        if (compact.Length > 2 && char.IsLetter(compact[0]) && char.IsLetter(compact[1]) && VatNumbers.Knows(compact.Remove(2)))
        {
            country = compact.Remove(2);
            body = compact.Remove(0, 2);
            if (given.Length > 0 && given != country)
                return (true, false, $"The number starts with {country}, but the country is {given}.");
        }
        else if (given.Length > 0)
        {
            country = given;
            body = compact;
        }
        else
        {
            return (true, false, "Start the number with its country prefix (for example DK) or give CountryCode.");
        }

        if (!VatNumbers.Knows(country))
            return (true, false, $"Country {country} is not known by this function.");

        var (valid, checkedDigits) = VatNumbers.Check(country, body);
        if (!valid)
            return (true, false, $"Not a valid {country} VAT number.");

        return (true, true, checkedDigits
            ? $"Valid {country} VAT number (format and check digits)."
            : $"Valid {country} VAT number format (this country's check digits are not verified).");
    }

    private static (bool Success, bool Value, string Message) CheckNational((string Code, int Min, int Max, bool Trunk) plan, string national, string country)
    {
        if (national.Length < plan.Min || national.Length > plan.Max)
        {
            var length = plan.Min == plan.Max
                ? plan.Min.ToString(CultureInfo.InvariantCulture)
                : $"{plan.Min.ToString(CultureInfo.InvariantCulture)}–{plan.Max.ToString(CultureInfo.InvariantCulture)}";
            return (true, false, $"A {country.ToUpperInvariant()} number has {length} digits after +{plan.Code}.");
        }

        return (true, true, $"Valid: +{plan.Code}{national}");
    }

    private static string? EmailProblem(string email)
    {
        if (email.Length > MaxEmailLength)
            return $"An e-mail address has at most {MaxEmailLength} characters.";

        var at = email.LastIndexOf('@');
        if (at <= 0 || at == email.Length - 1 || email.IndexOf('@') != at)
            return "An e-mail address has one @ with text on both sides.";

        var local = email.Substring(0, at);
        var domain = email.Substring(at + 1);
        if (local.Length > MaxLocalPartLength || !LocalPart.IsMatch(local))
            return "The part before the @ contains characters or dots that are not allowed.";

        var labels = domain.Split('.');
        if (labels.Length < 2 || !Array.TrueForAll(labels, l => DomainLabel.IsMatch(l)) || !TopLevelDomain.IsMatch(labels[labels.Length - 1]))
            return "The part after the @ is not a valid domain, like example.com.";

        return null;
    }
}
