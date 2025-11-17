using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FormSubmissionSystem.Domain.Entities;
using FormSubmissionSystem.Domain.Repositories;
using FormSubmissionSystem.Domain.ValueObjects;
using FormSubmissionSystem.Infrastructure.Data;

namespace FormSubmissionSystem.Infrastructure.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubmissionRepository> _logger;

    public SubmissionRepository(ApplicationDbContext context, ILogger<SubmissionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task<IEnumerable<FormSubmission>> GetAllAsync(string? searchTerm = null)
    {
        try
        {
            var query = _context.Submissions.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(s =>
                {
                    if (s.FormType.ToLower().Contains(searchLower))
                        return true;

                    try
                    {
                        var dataDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(s.Data);
                        if (dataDict != null)
                        {
                            return dataDict.Values.Any(v =>
                                v?.ToString()?.ToLower().Contains(searchLower) ?? false);
                        }
                    }
                    catch
                    {
                    }

                    return s.Data.ToLower().Contains(searchLower);
                });
            }

            var entities = query.OrderByDescending(s => s.SubmittedAt).ToList();
            return Task.FromResult<IEnumerable<FormSubmission>>(entities.Select(MapToDomain));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving submissions with search term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<FormSubmission?> GetByIdAsync(Guid id)
    {
        try
        {
            var entity = await _context.Submissions.FindAsync(id);
            return entity != null ? MapToDomain(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving submission with ID: {SubmissionId}", id);
            throw;
        }
    }

    public async Task<FormSubmission> CreateAsync(FormSubmission submission)
    {
        try
        {
            var entity = MapToEntity(submission);
            _context.Submissions.Add(entity);
            await _context.SaveChangesAsync();
            return submission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating submission of type: {FormType}", submission.FormType.Value);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Submissions.AnyAsync(s => s.Id == id);
    }

    private static FormSubmission MapToDomain(FormSubmissionEntity entity)
    {
        var formType = new FormType(entity.FormType);
        var formData = FormData.FromJsonString(entity.Data);
        
        return FormSubmission.FromPersistence(entity.Id, formType, formData, entity.SubmittedAt);
    }

    private static FormSubmissionEntity MapToEntity(FormSubmission submission)
    {
        return new FormSubmissionEntity
        {
            Id = submission.Id,
            FormType = submission.FormType.Value,
            Data = submission.Data.ToJsonString(),
            SubmittedAt = submission.SubmittedAt
        };
    }
}

