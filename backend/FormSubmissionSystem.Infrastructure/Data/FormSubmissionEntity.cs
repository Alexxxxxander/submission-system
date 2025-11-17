namespace FormSubmissionSystem.Infrastructure.Data;

public class FormSubmissionEntity
{
    public Guid Id { get; set; }
    public string FormType { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
}

