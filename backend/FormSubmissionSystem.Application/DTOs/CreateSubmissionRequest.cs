namespace FormSubmissionSystem.Application.DTOs;

public record CreateSubmissionRequest
{
    public required string FormType { get; init; }
    public required Dictionary<string, object> Data { get; init; }
}

