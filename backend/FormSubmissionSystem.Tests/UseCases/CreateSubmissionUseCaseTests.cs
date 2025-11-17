using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using FormSubmissionSystem.Application.DTOs;
using FormSubmissionSystem.Application.UseCases;
using FormSubmissionSystem.Domain.Repositories;
using Xunit;

namespace FormSubmissionSystem.Tests.UseCases;

public class CreateSubmissionUseCaseTests
{
    private readonly Mock<ISubmissionRepository> _repositoryMock;
    private readonly Mock<ILogger<CreateSubmissionUseCase>> _loggerMock;
    private readonly CreateSubmissionUseCase _useCase;

    public CreateSubmissionUseCaseTests()
    {
        _repositoryMock = new Mock<ISubmissionRepository>();
        _loggerMock = new Mock<ILogger<CreateSubmissionUseCase>>();
        _useCase = new CreateSubmissionUseCase(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ReturnsSubmissionResponse()
    {
        var request = new CreateSubmissionRequest
        {
            FormType = "order",
            Data = new Dictionary<string, object>
            {
                { "fullName", "John Doe" },
                { "product", "laptop" }
            }
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.FormSubmission>()))
            .ReturnsAsync((Domain.Entities.FormSubmission s) => s);

        var result = await _useCase.ExecuteAsync(request);

        Assert.NotNull(result);
        Assert.Equal("order", result.FormType);
        Assert.NotEqual(Guid.Empty, result.Id);
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Domain.Entities.FormSubmission>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyFormType_ThrowsArgumentException()
    {
        var request = new CreateSubmissionRequest
        {
            FormType = "",
            Data = new Dictionary<string, object>()
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_NullData_ThrowsArgumentNullException()
    {
        var request = new CreateSubmissionRequest
        {
            FormType = "order",
            Data = null!
        };

        await Assert.ThrowsAsync<ArgumentNullException>(() => _useCase.ExecuteAsync(request));
    }

    [Theory]
    [InlineData("contact")]
    [InlineData("feedback")]
    [InlineData("registration")]
    [InlineData("custom-form-type-123")]
    public async Task ExecuteAsync_DifferentFormTypes_Works(string formType)
    {
        var request = new CreateSubmissionRequest
        {
            FormType = formType,
            Data = new Dictionary<string, object> { { "field", "value" } }
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.FormSubmission>()))
            .ReturnsAsync((Domain.Entities.FormSubmission s) => s);

        var result = await _useCase.ExecuteAsync(request);

        Assert.Equal(formType, result.FormType);
    }

    [Fact]
    public async Task ExecuteAsync_ComplexData_Works()
    {
        var request = new CreateSubmissionRequest
        {
            FormType = "order",
            Data = new Dictionary<string, object>
            {
                { "string", "value" },
                { "number", 42 },
                { "boolean", true },
                { "array", new[] { 1, 2, 3 } },
                { "nested", new Dictionary<string, object> { { "key", "value" } } }
            }
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.FormSubmission>()))
            .ReturnsAsync((Domain.Entities.FormSubmission s) => s);

        var result = await _useCase.ExecuteAsync(request);

        Assert.NotNull(result);
        Assert.Contains("string", result.Data);
        Assert.Contains("number", result.Data);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyData_Works()
    {
        var request = new CreateSubmissionRequest
        {
            FormType = "order",
            Data = new Dictionary<string, object>()
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.FormSubmission>()))
            .ReturnsAsync((Domain.Entities.FormSubmission s) => s);

        var result = await _useCase.ExecuteAsync(request);

        Assert.NotNull(result);
        Assert.Equal("{}", result.Data);
    }

    [Fact]
    public async Task ExecuteAsync_WhitespaceFormType_ThrowsArgumentException()
    {
        var request = new CreateSubmissionRequest
        {
            FormType = "   ",
            Data = new Dictionary<string, object>()
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _useCase.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_GeneratesUniqueIds()
    {
        var request = new CreateSubmissionRequest
        {
            FormType = "order",
            Data = new Dictionary<string, object>()
        };

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Domain.Entities.FormSubmission>()))
            .ReturnsAsync((Domain.Entities.FormSubmission s) => s);

        var result1 = await _useCase.ExecuteAsync(request);
        var result2 = await _useCase.ExecuteAsync(request);

        Assert.NotEqual(result1.Id, result2.Id);
    }
}

