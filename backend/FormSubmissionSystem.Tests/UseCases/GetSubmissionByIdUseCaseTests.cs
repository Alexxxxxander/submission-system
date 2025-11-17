using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using FormSubmissionSystem.Application.UseCases;
using FormSubmissionSystem.Domain.Entities;
using FormSubmissionSystem.Domain.Repositories;
using FormSubmissionSystem.Domain.ValueObjects;
using Xunit;

namespace FormSubmissionSystem.Tests.UseCases;

public class GetSubmissionByIdUseCaseTests
{
    private readonly Mock<ISubmissionRepository> _repositoryMock;
    private readonly Mock<ILogger<GetSubmissionByIdUseCase>> _loggerMock;
    private readonly GetSubmissionByIdUseCase _useCase;

    public GetSubmissionByIdUseCaseTests()
    {
        _repositoryMock = new Mock<ISubmissionRepository>();
        _loggerMock = new Mock<ILogger<GetSubmissionByIdUseCase>>();
        _useCase = new GetSubmissionByIdUseCase(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ExistingId_ReturnsSubmission()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object> { { "fullName", "John Doe" } });
        var submission = new FormSubmission(formType, formData);
        var id = submission.Id;

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(submission);

        var result = await _useCase.ExecuteAsync(id);

        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal("order", result.FormType);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistingId_ReturnsNull()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((FormSubmission?)null);

        var result = await _useCase.ExecuteAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyGuid_ReturnsNull()
    {
        _repositoryMock
            .Setup(r => r.GetByIdAsync(Guid.Empty))
            .ReturnsAsync((FormSubmission?)null);

        var result = await _useCase.ExecuteAsync(Guid.Empty);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExecuteAsync_CallsRepositoryOnce()
    {
        var id = Guid.NewGuid();
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object>());
        var submission = new FormSubmission(formType, formData);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(submission);

        await _useCase.ExecuteAsync(id);

        _repositoryMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }
}

