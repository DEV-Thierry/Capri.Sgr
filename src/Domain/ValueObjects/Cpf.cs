namespace Capri.Sgr.Domain.ValueObjects;

/// <summary>Normalized Brazilian CPF with local check-digit validation.</summary>
public sealed record Cpf
{
    public Cpf(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var digits = new string(value.Where(char.IsDigit).ToArray());
        if (digits.Length != 11 || digits.Distinct().Count() == 1 || !HasValidCheckDigits(digits))
            throw new ArgumentException("CPF must contain 11 digits with valid check digits.", nameof(value));

        Value = digits;
    }

    public string Value { get; }
    public override string ToString() => Value;

    private static bool HasValidCheckDigits(string digits) =>
        CalculateDigit(digits[..9], 10) == digits[9] - '0' && CalculateDigit(digits[..10], 11) == digits[10] - '0';

    private static int CalculateDigit(string digits, int initialWeight)
    {
        var sum = digits.Select((digit, index) => (digit - '0') * (initialWeight - index)).Sum();
        var remainder = (sum * 10) % 11;
        return remainder == 10 ? 0 : remainder;
    }
}