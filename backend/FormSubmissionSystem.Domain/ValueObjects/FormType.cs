namespace FormSubmissionSystem.Domain.ValueObjects;

public class FormType : IEquatable<FormType>
{
    public string Value { get; private set; }

    private FormType()
    {
        Value = null!;
    }

    public FormType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("FormType cannot be empty", nameof(value));

        Value = value;
    }

    public bool Equals(FormType? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj) => obj is FormType other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(FormType formType) => formType.Value;
    public static implicit operator FormType(string value) => new(value);

    public override string ToString() => Value;
}

