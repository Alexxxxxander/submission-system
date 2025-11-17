using System;
using FormSubmissionSystem.Domain.ValueObjects;
using Xunit;

namespace FormSubmissionSystem.Tests.ValueObjects;

public class FormTypeTests
{
    [Fact]
    public void Constructor_ValidValue_SetsValue()
    {
        var formType = new FormType("order");
        
        Assert.Equal("order", formType.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_EmptyOrWhitespace_ThrowsArgumentException(string? value)
    {
        Assert.Throws<ArgumentException>(() => new FormType(value!));
    }

    [Fact]
    public void Constructor_LongValue_Accepts()
    {
        var longValue = new string('a', 100);
        var formType = new FormType(longValue);
        
        Assert.Equal(longValue, formType.Value);
    }

    [Fact]
    public void Equals_SameValue_ReturnsTrue()
    {
        var formType1 = new FormType("order");
        var formType2 = new FormType("order");
        
        Assert.True(formType1.Equals(formType2));
        Assert.Equal(formType1, formType2);
    }

    [Fact]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        var formType1 = new FormType("order");
        var formType2 = new FormType("contact");
        
        Assert.False(formType1.Equals(formType2));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var formType = new FormType("order");
        
        Assert.False(formType.Equals(null));
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHashCode()
    {
        var formType1 = new FormType("order");
        var formType2 = new FormType("order");
        
        Assert.Equal(formType1.GetHashCode(), formType2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValue_ReturnsDifferentHashCode()
    {
        var formType1 = new FormType("order");
        var formType2 = new FormType("contact");
        
        Assert.NotEqual(formType1.GetHashCode(), formType2.GetHashCode());
    }

    [Fact]
    public void ImplicitOperator_StringToFormType_Works()
    {
        FormType formType = "order";
        
        Assert.Equal("order", formType.Value);
    }

    [Fact]
    public void ImplicitOperator_FormTypeToString_Works()
    {
        var formType = new FormType("order");
        string value = formType;
        
        Assert.Equal("order", value);
    }

    [Fact]
    public void ToString_ReturnsValue()
    {
        var formType = new FormType("order");
        
        Assert.Equal("order", formType.ToString());
    }

    [Fact]
    public void Equals_CaseSensitive_IsCaseSensitive()
    {
        var formType1 = new FormType("Order");
        var formType2 = new FormType("order");
        
        Assert.False(formType1.Equals(formType2));
    }
}

