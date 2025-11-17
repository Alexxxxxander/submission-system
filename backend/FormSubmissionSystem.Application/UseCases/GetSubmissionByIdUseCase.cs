using Microsoft.Extensions.Logging;
using FormSubmissionSystem.Application.DTOs;
using FormSubmissionSystem.Domain.Repositories;

namespace FormSubmissionSystem.Application.UseCases;

public class GetSubmissionByIdUseCase
{
    private readonly ISubmissionRepository _repository;
    private readonly ILogger<GetSubmissionByIdUseCase> _logger;

    public GetSubmissionByIdUseCase(
        ISubmissionRepository repository,
        ILogger<GetSubmissionByIdUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SubmissionResponse?> ExecuteAsync(Guid id)
    {
        _logger.LogInformation("Retrieving submission with ID: {SubmissionId}", id);

        var submission = await _repository.GetByIdAsync(id);

        if (submission == null)
            return null;

        return MapToResponse(submission);
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

