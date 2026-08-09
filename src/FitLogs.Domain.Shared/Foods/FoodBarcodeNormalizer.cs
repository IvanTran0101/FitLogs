using System;

namespace FitLogs.Foods;

/// <summary>Normalizes scanner punctuation and validates supported GTIN barcode lengths and check digits.</summary>
public static class FoodBarcodeNormalizer
{
    private static readonly int[] SupportedLengths = [8, 12, 13, 14];

    public static bool TryNormalize(string? value, out string normalized)
    {
        normalized = string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var digits = new System.Text.StringBuilder(value.Length);
        foreach (var character in value)
        {
            if (char.IsWhiteSpace(character) || character == '-')
            {
                continue;
            }

            if (!char.IsDigit(character))
            {
                return false;
            }

            digits.Append(character);
        }

        normalized = digits.ToString();
        if (Array.IndexOf(SupportedLengths, normalized.Length) < 0 || !HasValidCheckDigit(normalized))
        {
            normalized = string.Empty;
            return false;
        }

        return true;
    }

    private static bool HasValidCheckDigit(string barcode)
    {
        var sum = 0;
        var weight = 3;
        for (var index = barcode.Length - 2; index >= 0; index--)
        {
            sum += (barcode[index] - '0') * weight;
            weight = weight == 3 ? 1 : 3;
        }

        var checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit == barcode[^1] - '0';
    }
}
