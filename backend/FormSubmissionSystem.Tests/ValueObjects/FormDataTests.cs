using System;
using System.Collections.Generic;
using System.Text.Json;
using FormSubmissionSystem.Domain.ValueObjects;
using Xunit;

namespace FormSubmissionSystem.Tests.ValueObjects;

public class FormDataTests
{
    [Fact]
    public void Constructor_ValidData_CreatesInstance()
    {
        var data = new Dictionary<string, object> { { "key", "value" } };
        var formData = new FormData(data);
        
        Assert.NotNull(formData);
    }

    [Fact]
    public void Constructor_NullData_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FormData(null!));
    }

    [Fact]
    public void ToJsonString_ValidData_ReturnsJson()
    {
        var data = new Dictionary<string, object>
        {
            { "fullName", "John Doe" },
            { "quantity", 5 }
        };
        var formData = new FormData(data);
        
        var json = formData.ToJsonString();
        
        Assert.Contains("fullName", json);
        Assert.Contains("quantity", json);
        Assert.Contains("5", json);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        Assert.NotNull(deserialized);
        Assert.Equal("John Doe", deserialized["fullName"].ToString());
    }

    [Fact]
    public void ToJsonString_EmptyData_ReturnsEmptyObjectJson()
    {
        var data = new Dictionary<string, object>();
        var formData = new FormData(data);
        
        var json = formData.ToJsonString();
        
        Assert.Equal("{}", json);
    }

    [Fact]
    public void FromJsonString_ValidJson_CreatesInstance()
    {
        var json = "{\"fullName\":\"John Doe\",\"quantity\":5}";
        
        var formData = FormData.FromJsonString(json);
        
        Assert.NotNull(formData);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData(null)]
    public void FromJsonString_EmptyOrWhitespace_ThrowsArgumentException(string? json)
    {
        Assert.Throws<ArgumentException>(() => FormData.FromJsonString(json!));
    }

    [Fact]
    public void FromJsonString_InvalidJson_ThrowsException()
    {
        var invalidJson = "{invalid json}";
        
        Assert.ThrowsAny<Exception>(() => FormData.FromJsonString(invalidJson));
    }

    [Fact]
    public void FromJsonString_EmptyObject_Works()
    {
        var json = "{}";
        
        var formData = FormData.FromJsonString(json);
        
        Assert.NotNull(formData);
        var dict = formData.ToDictionary();
        Assert.Empty(dict);
    }

    [Fact]
    public void ToDictionary_ReturnsCopy()
    {
        var originalData = new Dictionary<string, object> { { "key", "value" } };
        var formData = new FormData(originalData);
        
        var dict = formData.ToDictionary();
        dict["newKey"] = "newValue";
        
        var dict2 = formData.ToDictionary();
        Assert.DoesNotContain("newKey", dict2.Keys);
    }

    [Fact]
    public void Equals_SameData_ReturnsTrue()
    {
        var data1 = new Dictionary<string, object> { { "key", "value" } };
        var data2 = new Dictionary<string, object> { { "key", "value" } };
        var formData1 = new FormData(data1);
        var formData2 = new FormData(data2);
        
        Assert.True(formData1.Equals(formData2));
    }

    [Fact]
    public void Equals_DifferentData_ReturnsFalse()
    {
        var data1 = new Dictionary<string, object> { { "key", "value1" } };
        var data2 = new Dictionary<string, object> { { "key", "value2" } };
        var formData1 = new FormData(data1);
        var formData2 = new FormData(data2);
        
        Assert.False(formData1.Equals(formData2));
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var formData = new FormData(new Dictionary<string, object>());
        
        Assert.False(formData.Equals(null));
    }

    [Fact]
    public void GetHashCode_SameData_ReturnsSameHashCode()
    {
        var data1 = new Dictionary<string, object> { { "key", "value" } };
        var data2 = new Dictionary<string, object> { { "key", "value" } };
        var formData1 = new FormData(data1);
        var formData2 = new FormData(data2);
        
        Assert.Equal(formData1.GetHashCode(), formData2.GetHashCode());
    }

    [Fact]
    public void RoundTrip_JsonSerialization_PreservesData()
    {
        var originalData = new Dictionary<string, object>
        {
            { "string", "value" },
            { "number", 42 },
            { "boolean", true },
            { "array", new[] { 1, 2, 3 } }
        };
        var formData1 = new FormData(originalData);
        
        var json = formData1.ToJsonString();
        var formData2 = FormData.FromJsonString(json);
        
        Assert.True(formData1.Equals(formData2));
    }

    [Fact]
    public void FromJsonString_ComplexNestedData_Works()
    {
        var json = "{\"user\":{\"name\":\"John\",\"age\":30},\"items\":[1,2,3]}";
        
        var formData = FormData.FromJsonString(json);
        
        Assert.NotNull(formData);
        var jsonResult = formData.ToJsonString();
        Assert.Contains("user", jsonResult);
        Assert.Contains("items", jsonResult);
    }
}

