using System;
using System.Collections.Generic;
using FormSubmissionSystem.Domain.Entities;
using FormSubmissionSystem.Domain.ValueObjects;
using Xunit;

namespace FormSubmissionSystem.Tests.Entities;

public class FormSubmissionTests
{
    [Fact]
    public void Constructor_ValidParameters_CreatesInstance()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object> { { "key", "value" } });
        
        var submission = new FormSubmission(formType, formData);
        
        Assert.NotNull(submission);
        Assert.NotEqual(Guid.Empty, submission.Id);
        Assert.Equal(formType, submission.FormType);
        Assert.Equal(formData, submission.Data);
        Assert.True(submission.SubmittedAt <= DateTime.UtcNow);
        Assert.True(submission.SubmittedAt > DateTime.UtcNow.AddSeconds(-1));
    }

    [Fact]
    public void Constructor_NullFormType_ThrowsArgumentNullException()
    {
        var formData = new FormData(new Dictionary<string, object>());
        
        Assert.Throws<ArgumentNullException>(() => new FormSubmission(null!, formData));
    }

    [Fact]
    public void Constructor_NullFormData_ThrowsArgumentNullException()
    {
        var formType = new FormType("order");
        
        Assert.Throws<ArgumentNullException>(() => new FormSubmission(formType, null!));
    }

    [Fact]
    public void Constructor_GeneratesUniqueId()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object>());
        
        var submission1 = new FormSubmission(formType, formData);
        var submission2 = new FormSubmission(formType, formData);
        
        Assert.NotEqual(submission1.Id, submission2.Id);
    }

    [Fact]
    public void Constructor_SetsSubmittedAtToUtcNow()
    {
        var before = DateTime.UtcNow;
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object>());
        
        var submission = new FormSubmission(formType, formData);
        
        var after = DateTime.UtcNow;
        Assert.True(submission.SubmittedAt >= before);
        Assert.True(submission.SubmittedAt <= after);
    }

    [Fact]
    public void FromPersistence_ValidParameters_RestoresInstance()
    {
        var id = Guid.NewGuid();
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object> { { "key", "value" } });
        var submittedAt = DateTime.UtcNow.AddHours(-1);
        
        var submission = FormSubmission.FromPersistence(id, formType, formData, submittedAt);
        
        Assert.Equal(id, submission.Id);
        Assert.Equal(formType, submission.FormType);
        Assert.Equal(formData, submission.Data);
        Assert.Equal(submittedAt, submission.SubmittedAt);
    }

    [Fact]
    public void FromPersistence_PreservesOriginalSubmittedAt()
    {
        var id = Guid.NewGuid();
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object>());
        var originalDate = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        
        var submission = FormSubmission.FromPersistence(id, formType, formData, originalDate);
        
        Assert.Equal(originalDate, submission.SubmittedAt);
    }

    [Fact]
    public void FromPersistence_DifferentIds_CreatesDifferentInstances()
    {
        var formType = new FormType("order");
        var formData = new FormData(new Dictionary<string, object>());
        var submittedAt = DateTime.UtcNow;
        
        var submission1 = FormSubmission.FromPersistence(Guid.NewGuid(), formType, formData, submittedAt);
        var submission2 = FormSubmission.FromPersistence(Guid.NewGuid(), formType, formData, submittedAt);
        
        Assert.NotEqual(submission1.Id, submission2.Id);
    }
}

