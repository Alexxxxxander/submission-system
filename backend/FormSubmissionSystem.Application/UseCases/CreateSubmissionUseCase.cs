using Microsoft.Extensions.Logging;
using FormSubmissionSystem.Application.DTOs;
using FormSubmissionSystem.Domain.Entities;
using FormSubmissionSystem.Domain.Repositories;
using FormSubmissionSystem.Domain.ValueObjects;

namespace FormSubmissionSystem.Application.UseCases;

public class CreateSubmissionUseCase
{
    private readonly ISubmissionRepository _repository;
    private readonly ILogger<CreateSubmissionUseCase> _logger;

    public CreateSubmissionUseCase(
        ISubmissionRepository repository,
        ILogger<CreateSubmissionUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<SubmissionResponse> ExecuteAsync(CreateSubmissionRequest request)
    {
        _logger.LogInformation("Creating new submission of type: {FormType}", request.FormType);

        var formType = new FormType(request.FormType);
        var formData = new FormData(request.Data);

        var submission = new FormSubmission(formType, formData);

        var created = await _repository.CreateAsync(submission);

        _logger.LogInformation("Successfully created submission with ID: {SubmissionId}", created.Id);

        return MapToResponse(created);
    }

    private static SubmissionResponse MapToResponse(FormSubmission submission)
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

