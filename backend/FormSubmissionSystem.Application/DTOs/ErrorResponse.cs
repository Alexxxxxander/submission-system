namespace FormSubmissionSystem.Application.DTOs;

public record ErrorResponse
{
    public required string Error { get; init; }
    public string? Details { get; init; }
}

