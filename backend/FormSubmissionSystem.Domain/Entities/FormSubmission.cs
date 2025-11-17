using System.Runtime.CompilerServices;
using FormSubmissionSystem.Domain.ValueObjects;

[assembly: InternalsVisibleTo("FormSubmissionSystem.Infrastructure")]

namespace FormSubmissionSystem.Domain.Entities;

public class FormSubmission
{
    public Guid Id { get; internal set; }
    public FormType FormType { get; private set; }
    public FormData Data { get; private set; }
    public DateTime SubmittedAt { get; internal set; }

    private FormSubmission()
    {
        FormType = null!;
        Data = null!;
    }

    public FormSubmission(FormType formType, FormData data)
    {
        Id = Guid.NewGuid();
        FormType = formType ?? throw new ArgumentNullException(nameof(formType));
        Data = data ?? throw new ArgumentNullException(nameof(data));
        SubmittedAt = DateTime.UtcNow;
    }

    public static FormSubmission FromPersistence(Guid id, FormType formType, FormData data, DateTime submittedAt)
    {
        var submission = new FormSubmission(formType, data);
        submission.Id = id;
        submission.SubmittedAt = submittedAt;
        return submission;
    }
}

