using FormSubmissionSystem.Domain.Entities;

namespace FormSubmissionSystem.Domain.Repositories;

public interface ISubmissionRepository
{
    Task<IEnumerable<FormSubmission>> GetAllAsync(string? searchTerm = null);
    Task<FormSubmission?> GetByIdAsync(Guid id);
    Task<FormSubmission> CreateAsync(FormSubmission submission);
    Task<bool> ExistsAsync(Guid id);
}

