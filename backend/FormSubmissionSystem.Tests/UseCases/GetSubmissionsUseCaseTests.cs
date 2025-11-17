using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using FormSubmissionSystem.Application.UseCases;
using FormSubmissionSystem.Domain.Entities;
using FormSubmissionSystem.Domain.Repositories;
using FormSubmissionSystem.Domain.ValueObjects;
using Xunit;

namespace FormSubmissionSystem.Tests.UseCases;

public class GetSubmissionsUseCaseTests
{
    private readonly Mock<ISubmissionRepository> _repositoryMock;
    private readonly Mock<ILogger<GetSubmissionsUseCase>> _loggerMock;
    private readonly GetSubmissionsUseCase _useCase;

    public GetSubmissionsUseCaseTests()
    {
        _repositoryMock = new Mock<ISubmissionRepository>();
        _loggerMock = new Mock<ILogger<GetSubmissionsUseCase>>();
        _useCase = new GetSubmissionsUseCase(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithSearchTerm_ReturnsFilteredResults()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object> { { "fullName", "John Doe" } });
        var submission = new FormSubmission(formType, formData);

        _repositoryMock
            .Setup(r => r.GetAllAsync("John"))
            .ReturnsAsync(new[] { submission });

        var result = await _useCase.ExecuteAsync("John");

        Assert.Single(result);
        Assert.Equal("order", result.First().FormType);
    }

    [Fact]
    public async Task ExecuteAsync_WithoutSearchTerm_ReturnsAllResults()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object>());
        var submission = new FormSubmission(formType, formData);

        _repositoryMock
            .Setup(r => r.GetAllAsync(null))
            .ReturnsAsync(new[] { submission });

        var result = await _useCase.ExecuteAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task ExecuteAsync_EmptySearchTerm_ReturnsAllResults()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object>());
        var submission = new FormSubmission(formType, formData);

        _repositoryMock
            .Setup(r => r.GetAllAsync(""))
            .ReturnsAsync(new[] { submission });

        var result = await _useCase.ExecuteAsync("");

        Assert.Single(result);
    }

    [Fact]
    public async Task ExecuteAsync_NoResults_ReturnsEmpty()
    {
        _repositoryMock
            .Setup(r => r.GetAllAsync("nonexistent"))
            .ReturnsAsync(Array.Empty<FormSubmission>());

        var result = await _useCase.ExecuteAsync("nonexistent");

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_MultipleResults_ReturnsAll()
    {
        var submissions = new[]
        {
            new FormSubmission(new FormType("order"), new FormData(new Dictionary<string, object>())),
            new FormSubmission(new FormType("contact"), new FormData(new Dictionary<string, object>())),
            new FormSubmission(new FormType("order"), new FormData(new Dictionary<string, object>()))
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync(null))
            .ReturnsAsync(submissions);

        var result = await _useCase.ExecuteAsync();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task ExecuteAsync_CaseInsensitiveSearch_Works()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object> { { "fullName", "John Doe" } });
        var submission = new FormSubmission(formType, formData);

        _repositoryMock
            .Setup(r => r.GetAllAsync("john"))
            .ReturnsAsync(new[] { submission });

        var result = await _useCase.ExecuteAsync("john");

        Assert.Single(result);
    }

    [Fact]
    public async Task ExecuteAsync_SpecialCharactersInSearch_Works()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object> { { "email", "test@example.com" } });
        var submission = new FormSubmission(formType, formData);

        _repositoryMock
            .Setup(r => r.GetAllAsync("test@example.com"))
            .ReturnsAsync(new[] { submission });

        var result = await _useCase.ExecuteAsync("test@example.com");

        Assert.Single(result);
    }
}

