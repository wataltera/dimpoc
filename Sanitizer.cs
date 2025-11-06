using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dimpoc;

public static class AnnotationProcessor
{
    private static readonly Regex DigitsOnly = new(@"\D", RegexOptions.Compiled);
    private static readonly Regex UsnumKeep = new(@"[^0-9+\-\.eE]", RegexOptions.Compiled);
    private static readonly Regex TinKeep = new(@"[^0-9\-]", RegexOptions.Compiled);

    public static FieldResults Sanitize(object obj)
    {
        var results = new FieldResults();
        if (obj == null) return results;

        var t = obj.GetType();
        var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var f in fields)
        {
            if (f.FieldType != typeof(string)) continue;

            var attrs = f.GetCustomAttributes(false);
            if (!attrs.Any(a => a is TINAttribute || a is PANAttribute || a is TelAttribute || a is USNUMAttribute)) continue;

            var original = (string?)f.GetValue(obj);
            var value = original ?? string.Empty;
            var changed = false;

            if (original == null)
            {
                changed = true; // null->empty
            }

            if (attrs.Any(a => a is PANAttribute))
            {
                var sanitized = DigitsOnly.Replace(value, "");
                if (sanitized != value) changed = true;
                value = sanitized;
            }
            else if (attrs.Any(a => a is TINAttribute))
            {
                var hasP = false;
                if (!string.IsNullOrEmpty(value) && (value[0] == 'P' || value[0] == 'p'))
                {
                    hasP = true;
                    value = value[1..];
                }
                var sanitized = TinKeep.Replace(value, "");
                if (hasP) sanitized = "P" + sanitized;
                if (sanitized != original) changed = true;
                value = sanitized;
            }
            else if (attrs.Any(a => a is TelAttribute))
            {
                var sanitized = DigitsOnly.Replace(value, "");
                if (sanitized != value) changed = true;
                value = sanitized;
            }
            else if (attrs.Any(a => a is USNUMAttribute))
            {
                var sanitized = UsnumKeep.Replace(value, "");
                if (sanitized != value) changed = true;
                value = sanitized;
            }

            // set back to field
            f.SetValue(obj, value);

            var item = new FieldResultItem
            {
                FieldName = f.Name,
                Empty = string.IsNullOrEmpty(value),
                Valid = !changed // per spec: valid=true if no changes, false if changed
            };
            results.Items.Add(item);
        }
        
    return results;
    }

    public static FieldResults Validate(object obj)
    {
        var results = new FieldResults();
        if (obj == null) return results;

        var t = obj.GetType();
        var fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (var f in fields)
        {
            if (f.FieldType != typeof(string)) continue;

            var attrs = f.GetCustomAttributes(false);
            if (!attrs.Any(a => a is TINAttribute || a is PANAttribute || a is TelAttribute || a is USNUMAttribute)) continue;

            var value = (string?)f.GetValue(obj) ?? string.Empty;
            var empty = string.IsNullOrEmpty(value);
            var valid = false;

            if (attrs.Any(a => a is PANAttribute))
            {
                // digits only expected
                var digits = DigitsOnly.Replace(value, "");
                valid = digits.Length >= 13 && digits.Length <= 19 && LuhnCheck(digits);
            }
            else if (attrs.Any(a => a is TINAttribute))
            {
                var s = value;
                if (!string.IsNullOrEmpty(s) && (s[0] == 'P' || s[0] == 'p')) s = s[1..];
                var digits = DigitsOnly.Replace(s, "");
                valid = digits.Length == 9; // basic check for SSN/TIN length
            }
            else if (attrs.Any(a => a is TelAttribute))
            {
                var digits = DigitsOnly.Replace(value, "");
                if (digits.Length == 11 && digits.StartsWith("1"))
                {
                    valid = true;
                }
                else if (digits.Length == 10)
                {
                    // US 10-digit: add country code 1 in front as per spec
                    digits = "1" + digits;
                    f.SetValue(obj, digits);
                    valid = true;
                }
                else
                {
                    valid = false;
                }
            }
            else if (attrs.Any(a => a is USNUMAttribute))
            {
                // try parse using invariant culture, accept floats, scientific notation and signs
                valid = double.TryParse(value, NumberStyles.Float | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _);
            }
            
            results.Items.Add(new FieldResultItem { FieldName = f.Name, Empty = empty, Valid = valid });
        }

        return results;
    }

    private static bool LuhnCheck(string digits)
    {
        if (string.IsNullOrEmpty(digits)) return false;
        int sum = 0;
        bool alt = false;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            if (!char.IsDigit(digits[i])) return false;
            int n = digits[i] - '0';
            if (alt)
            {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alt = !alt;
        }
        return (sum % 10) == 0;
    }
}
