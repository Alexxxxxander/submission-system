using Microsoft.Extensions.Logging;
using FormSubmissionSystem.Application.DTOs;
using FormSubmissionSystem.Domain.Repositories;

namespace FormSubmissionSystem.Application.UseCases;

public class GetSubmissionsUseCase
{
    private readonly ISubmissionRepository _repository;
    private readonly ILogger<GetSubmissionsUseCase> _logger;

    public GetSubmissionsUseCase(
        ISubmissionRepository repository,
        ILogger<GetSubmissionsUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<SubmissionResponse>> ExecuteAsync(string? searchTerm = null)
    {
        _logger.LogInformation("Retrieving submissions with search term: {SearchTerm}", searchTerm ?? "none");

        var submissions = await _repository.GetAllAsync(searchTerm);

        return submissions.Select(MapToResponse);
    }

    private static SubmissionResponse MapToResponse(Domain.Entities.FormSubmission submission)
    {
        return new SubmissionResponse
        {
            Id = submission.Id,
            FormType = submission.FormType.Value,
            Data = submission.Data.ToJsonString(),
            SubmittedAt = submission.SubmittedAt
        };
    }
}

