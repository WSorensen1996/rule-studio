using System.Globalization;
using System.Text.RegularExpressions;

namespace DataverseLogic.RuleStudio.Functions;

/// <summary>
/// VAT number formats per country prefix (Greece uses EL, Northern Ireland XI), with the public
/// check-digit algorithms for the countries that have one.
/// </summary>
internal static class VatNumbers
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(200);

    private static readonly Dictionary<string, Regex> Formats = new(StringComparer.Ordinal)
    {
        ["AT"] = Format(@"U\d{8}"),
        ["BE"] = Format(@"[01]?\d{9}"),
        ["BG"] = Format(@"\d{9,10}"),
        ["CH"] = Format(@"E\d{9}(MWST|TVA|IVA)?"),
        ["CY"] = Format(@"\d{8}[A-Z]"),
        ["CZ"] = Format(@"\d{8,10}"),
        ["DE"] = Format(@"\d{9}"),
        ["DK"] = Format(@"\d{8}"),
        ["EE"] = Format(@"\d{9}"),
        ["EL"] = Format(@"\d{9}"),
        ["ES"] = Format(@"[0-9A-Z]\d{7}[0-9A-Z]"),
        ["FI"] = Format(@"\d{8}"),
        ["FR"] = Format(@"[0-9A-HJ-NP-Z]{2}\d{9}"),
        ["GB"] = Format(@"\d{9}|\d{12}|GD\d{3}|HA\d{3}"),
        ["HR"] = Format(@"\d{11}"),
        ["HU"] = Format(@"\d{8}"),
        ["IE"] = Format(@"\d{7}[A-W][A-I]?|\d[A-Z+*]\d{5}[A-W]"),
        ["IT"] = Format(@"\d{11}"),
        ["LT"] = Format(@"\d{9}|\d{12}"),
        ["LU"] = Format(@"\d{8}"),
        ["LV"] = Format(@"\d{11}"),
        ["MT"] = Format(@"\d{8}"),
        ["NL"] = Format(@"\d{9}B\d{2}"),
        ["NO"] = Format(@"\d{9}(MVA)?"),
        ["PL"] = Format(@"\d{10}"),
        ["PT"] = Format(@"\d{9}"),
        ["RO"] = Format(@"\d{2,10}"),
        ["SE"] = Format(@"\d{10}01"),
        ["SI"] = Format(@"\d{8}"),
        ["SK"] = Format(@"\d{10}"),
        ["XI"] = Format(@"\d{9}|\d{12}|GD\d{3}|HA\d{3}"),
    };

    private static readonly Dictionary<string, Func<string, bool>> CheckDigits = new(StringComparer.Ordinal)
    {
        ["AT"] = Austria,
        ["BE"] = Belgium,
        ["CH"] = Switzerland,
        ["DE"] = Germany,
        ["DK"] = Denmark,
        ["FI"] = Finland,
        ["FR"] = France,
        ["IT"] = b => Luhn(b),
        ["NL"] = Netherlands,
        ["NO"] = Norway,
        ["PL"] = Poland,
        ["SE"] = b => Luhn(b.Remove(10)),
    };

    internal static bool Knows(string country) => Formats.ContainsKey(country);

    // Whether the number fits the country's format and check digits, and whether check digits were verified.
    internal static (bool Valid, bool CheckedDigits) Check(string country, string body)
    {
        if (!Formats[country].IsMatch(body))
            return (false, false);

        if (!CheckDigits.TryGetValue(country, out var check))
            return (true, false);

        // France: only numbers whose key is two digits have a public algorithm.
        if (country == "FR" && !(char.IsDigit(body[0]) && char.IsDigit(body[1])))
            return (true, false);

        return (check(body), true);
    }

    private static Regex Format(string pattern) => new($"^(?:{pattern})$", RegexOptions.CultureInvariant, RegexTimeout);

    private static int Digit(string text, int index) => text[index] - '0';

    private static int Weighted(string text, params int[] weights)
    {
        var sum = 0;
        for (var i = 0; i < weights.Length; i++)
        {
            sum += Digit(text, i) * weights[i];
        }

        return sum;
    }

    // Mod 11 check digit: 11 - remainder, 0 when the remainder is 0; a remainder of 1 has no valid digit.
    private static bool Mod11(string text, int checkIndex, params int[] weights)
    {
        var remainder = Weighted(text, weights) % 11;
        if (remainder == 1)
            return false;

        return (remainder == 0 ? 0 : 11 - remainder) == Digit(text, checkIndex);
    }

    private static bool Austria(string body)
    {
        var digits = body.Remove(0, 1);
        var sum = 0;
        for (var i = 0; i < 7; i++)
        {
            var d = Digit(digits, i);
            sum += i % 2 == 0 ? d : ((d * 2) / 10) + ((d * 2) % 10);
        }

        return (10 - ((sum + 4) % 10)) % 10 == Digit(digits, 7);
    }

    private static bool Belgium(string body)
    {
        var digits = body.Length == 9 ? "0" + body : body;
        var number = long.Parse(digits.Remove(8), CultureInfo.InvariantCulture);
        return 97 - (number % 97) == long.Parse(digits.Remove(0, 8), CultureInfo.InvariantCulture);
    }

    private static bool Denmark(string body) => body[0] != '0' && Weighted(body, 2, 7, 6, 5, 4, 3, 2, 1) % 11 == 0;

    private static bool Finland(string body) => Mod11(body, 7, 7, 9, 10, 5, 8, 4, 2);

    private static bool France(string body)
    {
        var key = int.Parse(body.Remove(2), CultureInfo.InvariantCulture);
        var siren = long.Parse(body.Remove(0, 2), CultureInfo.InvariantCulture);
        return (12 + (3 * (siren % 97))) % 97 == key;
    }

    // ISO 7064 MOD 11,10.
    private static bool Germany(string body)
    {
        var product = 10;
        for (var i = 0; i < 8; i++)
        {
            var sum = (Digit(body, i) + product) % 10;
            if (sum == 0)
            {
                sum = 10;
            }

            product = (2 * sum) % 11;
        }

        var check = 11 - product;
        return (check == 10 ? 0 : check) == Digit(body, 8);
    }

    private static bool Luhn(string digits)
    {
        var sum = 0;
        for (var i = 0; i < digits.Length; i++)
        {
            var d = Digit(digits, digits.Length - 1 - i);
            if (i % 2 == 1)
            {
                d *= 2;
                if (d > 9)
                {
                    d -= 9;
                }
            }

            sum += d;
        }

        return sum % 10 == 0;
    }

    // The old mod 11 check on the first nine digits, or (numbers since 2020) mod 97 over the whole number.
    private static bool Netherlands(string body)
    {
        var sum = Weighted(body, 9, 8, 7, 6, 5, 4, 3, 2) - Digit(body, 8);
        if (sum % 11 == 0)
            return true;

        var numeric = string.Concat(("NL" + body).Select(c => char.IsLetter(c) ? (c - 'A' + 10).ToString(CultureInfo.InvariantCulture) : c.ToString()));
        return numeric.Aggregate(0, (remainder, c) => ((remainder * 10) + (c - '0')) % 97) == 1;
    }

    private static bool Norway(string body) => Mod11(body, 8, 3, 2, 7, 6, 5, 4, 3, 2);

    private static bool Poland(string body)
    {
        var check = Weighted(body, 6, 5, 7, 2, 3, 4, 5, 6, 7) % 11;
        return check != 10 && check == Digit(body, 9);
    }

    private static bool Switzerland(string body) => Mod11(body.Remove(0, 1), 8, 5, 4, 3, 2, 7, 6, 5, 4);
}
