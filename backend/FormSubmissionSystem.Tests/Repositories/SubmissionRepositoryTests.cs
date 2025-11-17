using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using FormSubmissionSystem.Domain.Entities;
using FormSubmissionSystem.Domain.ValueObjects;
using FormSubmissionSystem.Infrastructure.Data;
using FormSubmissionSystem.Infrastructure.Repositories;
using Xunit;

namespace FormSubmissionSystem.Tests.Repositories;

public class SubmissionRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly SubmissionRepository _repository;

    public SubmissionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        var loggerMock = new Mock<ILogger<SubmissionRepository>>();
        _repository = new SubmissionRepository(_context, loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ValidSubmission_ReturnsCreatedSubmission()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object> { { "fullName", "John Doe" } });
        var submission = new FormSubmission(formType, formData);

        var result = await _repository.CreateAsync(submission);

        Assert.NotNull(result);
        Assert.Equal(submission.Id, result.Id);
        Assert.Equal(submission.FormType.Value, result.FormType.Value);

        var saved = await _context.Submissions.FindAsync(submission.Id);
        Assert.NotNull(saved);
    }

    [Fact]
    public async Task GetAllAsync_WithoutSearchTerm_ReturnsAllSubmissions()
    {
        var submission1 = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object> { { "fullName", "John Doe" } })
        );

        var submission2 = new FormSubmission(
            new FormType("contact"),
            new FormData(new Dictionary<string, object> { { "email", "test@test.com" } })
        );

        await _repository.CreateAsync(submission1);
        await _repository.CreateAsync(submission2);

        var result = await _repository.GetAllAsync();

        Assert.Equal(2, result.Count());
        var ids = result.Select(s => s.Id).ToList();
        Assert.Contains(submission1.Id, ids);
        Assert.Contains(submission2.Id, ids);
    }

    [Fact]
    public async Task GetAllAsync_WithSearchTerm_ReturnsFilteredSubmissions()
    {
        var submission1 = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>
            {
                { "fullName", "John Doe" },
                { "product", "laptop" }
            })
        );

        var submission2 = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>
            {
                { "fullName", "Jane Smith" },
                { "product", "phone" }
            })
        );

        await _repository.CreateAsync(submission1);
        await _repository.CreateAsync(submission2);

        var result = await _repository.GetAllAsync("laptop");

        Assert.Single(result);
        Assert.Equal(submission1.Id, result.First().Id);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsSubmission()
    {
        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object> { { "fullName", "John Doe" } })
        );

        await _repository.CreateAsync(submission);

        var result = await _repository.GetByIdAsync(submission.Id);

        Assert.NotNull(result);
        Assert.Equal(submission.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_ExistingId_ReturnsTrue()
    {
        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object> { { "fullName", "John Doe" } })
        );

        await _repository.CreateAsync(submission);

        var result = await _repository.ExistsAsync(submission.Id);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_NonExistingId_ReturnsFalse()
    {
        var result = await _repository.ExistsAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_SearchByFormType_ReturnsMatching()
    {
        var submission1 = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>())
        );
        var submission2 = new FormSubmission(
            new FormType("contact"),
            new FormData(new Dictionary<string, object>())
        );

        await _repository.CreateAsync(submission1);
        await _repository.CreateAsync(submission2);

        var result = await _repository.GetAllAsync("order");

        Assert.Single(result);
        Assert.Equal(submission1.Id, result.First().Id);
    }

    [Fact]
    public async Task GetAllAsync_SearchByNestedData_Works()
    {
        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>
            {
                { "userName", "John" }
            })
        );

        await _repository.CreateAsync(submission);

        var result = await _repository.GetAllAsync("John");

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_SearchWithSpecialCharacters_Works()
    {
        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>
            {
                { "email", "user@example.com" },
                { "phone", "+7 (999) 123-45-67" }
            })
        );

        await _repository.CreateAsync(submission);

        var result1 = await _repository.GetAllAsync("@example");
        var result2 = await _repository.GetAllAsync("+7");

        Assert.Single(result1);
        Assert.Single(result2);
    }

    [Fact]
    public async Task GetAllAsync_SearchEmptyString_ReturnsAll()
    {
        var submission1 = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>())
        );
        var submission2 = new FormSubmission(
            new FormType("contact"),
            new FormData(new Dictionary<string, object>())
        );

        await _repository.CreateAsync(submission1);
        await _repository.CreateAsync(submission2);

        var result = await _repository.GetAllAsync("");

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_SearchWhitespace_ReturnsAll()
    {
        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>())
        );

        await _repository.CreateAsync(submission);

        var result = await _repository.GetAllAsync("   ");

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_OrderedBySubmittedAtDescending()
    {
        var submission1 = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>())
        );
        await _repository.CreateAsync(submission1);
        await Task.Delay(10);

        var submission2 = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>())
        );
        await _repository.CreateAsync(submission2);

        var result = await _repository.GetAllAsync();

        Assert.Equal(submission2.Id, result.First().Id);
        Assert.Equal(submission1.Id, result.Skip(1).First().Id);
    }

    [Fact]
    public async Task CreateAsync_MultipleSubmissions_AllPersisted()
    {
        var submissions = Enumerable.Range(0, 10)
            .Select(i => new FormSubmission(
                new FormType($"type{i}"),
                new FormData(new Dictionary<string, object> { { "index", i } })
            ))
            .ToList();

        foreach (var submission in submissions)
        {
            await _repository.CreateAsync(submission);
        }

        var result = await _repository.GetAllAsync();

        Assert.Equal(10, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_EmptyGuid_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.Empty);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_EmptyGuid_ReturnsFalse()
    {
        var result = await _repository.ExistsAsync(Guid.Empty);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_LargeData_Works()
    {
        var largeData = new Dictionary<string, object>();
        for (int i = 0; i < 100; i++)
        {
            largeData[$"field{i}"] = $"value{i}";
        }

        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(largeData)
        );

        await _repository.CreateAsync(submission);

        var result = await _repository.GetAllAsync("value50");

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_SearchInArrayValues_Works()
    {
        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>
            {
                { "tags", new[] { "urgent", "important", "review" } }
            })
        );

        await _repository.CreateAsync(submission);

        var result = await _repository.GetAllAsync("urgent");

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_SearchPartialMatch_Works()
    {
        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>
            {
                { "fullName", "John Doe Smith" }
            })
        );

        await _repository.CreateAsync(submission);

        var result = await _repository.GetAllAsync("Doe");

        Assert.Single(result);
    }

    [Fact]
    public async Task GetAllAsync_NoMatches_ReturnsEmpty()
    {
        var submission = new FormSubmission(
            new FormType("order"),
            new FormData(new Dictionary<string, object>
            {
                { "fullName", "John Doe" }
            })
        );

        await _repository.CreateAsync(submission);

        var result = await _repository.GetAllAsync("nonexistent");

        Assert.Empty(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
