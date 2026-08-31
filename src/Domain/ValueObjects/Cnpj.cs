namespace Capri.Sgr.Domain.ValueObjects;

/// <summary>Normalized Brazilian CNPJ with local check-digit validation.</summary>
public sealed record Cnpj
{
    public Cnpj(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var digits = new string(value.Where(char.IsDigit).ToArray());
        if (digits.Length != 14 || digits.Distinct().Count() == 1 || !HasValidCheckDigits(digits))
            throw new ArgumentException("CNPJ must contain 14 digits with valid check digits.", nameof(value));

        Value = digits;
    }

    public string Value { get; }
    public override string ToString() => Value;

    private static bool HasValidCheckDigits(string digits) =>
        CalculateDigit(digits[..12], [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]) == digits[12] - '0' &&
        CalculateDigit(digits[..13], [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]) == digits[13] - '0';

    private static int CalculateDigit(string digits, IReadOnlyList<int> weights)
    {
        var sum = digits.Select((digit, index) => (digit - '0') * weights[index]).Sum();
        var remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }
}