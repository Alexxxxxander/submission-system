using System.Text.Json;

namespace FormSubmissionSystem.Domain.ValueObjects;

public class FormData : IEquatable<FormData>
{
    private readonly Dictionary<string, object> _data;

    public FormData(Dictionary<string, object> data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public string ToJsonString() => JsonSerializer.Serialize(_data);

    public static FormData FromJsonString(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON string cannot be empty", nameof(json));

        var data = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        if (data == null)
            throw new ArgumentException("Invalid JSON format", nameof(json));

        return new FormData(data);
    }

    public Dictionary<string, object> ToDictionary() => new(_data);

    public bool Equals(FormData? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return ToJsonString() == other.ToJsonString();
    }

    public override bool Equals(object? obj) => obj is FormData other && Equals(other);

    public override int GetHashCode() => ToJsonString().GetHashCode();
}

