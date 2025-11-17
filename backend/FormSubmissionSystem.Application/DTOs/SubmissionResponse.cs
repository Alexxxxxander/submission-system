namespace FormSubmissionSystem.Application.DTOs;

public record SubmissionResponse
{
    public required Guid Id { get; init; }
    public required string FormType { get; init; }
    public required string Data { get; init; }
    public required DateTime SubmittedAt { get; init; }
}

