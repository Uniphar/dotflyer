using DotFlyer.Common.EmailTemplates;
using System.Text.Json;

namespace DotFlyer.EmailTemplates.Tests;

[TestClass, TestCategory("Unit")]
public class NewEmployeeOnboardingModelTests
{
    [TestMethod]
    public void NewEmployeeOnboardingModel_ShouldRequire_FirstName()
    {
        var json = """
        {
            "UserPrincipalName": "jane.doe@example.com"
        }
        """;

        try
        {
            JsonSerializer.Deserialize<NewEmployeeOnboardingModel>(json);
            Assert.Fail("Expected JsonException but no exception was thrown");
        }
        catch (JsonException ex)
        {
            Assert.IsTrue(ex.Message.Contains("FirstName") || ex.Message.Contains("required"),
                $"Expected deserialization to fail for missing FirstName. Actual message: {ex.Message}");
        }
    }

    [TestMethod]
    public void NewEmployeeOnboardingModel_ShouldRequire_UserPrincipalName()
    {
        var json = """
        {
            "FirstName": "Jane"
        }
        """;

        try
        {
            JsonSerializer.Deserialize<NewEmployeeOnboardingModel>(json);
            Assert.Fail("Expected JsonException but no exception was thrown");
        }
        catch (JsonException ex)
        {
            Assert.IsTrue(ex.Message.Contains("UserPrincipalName") || ex.Message.Contains("required"),
                $"Expected deserialization to fail for missing UserPrincipalName. Actual message: {ex.Message}");
        }
    }

    [TestMethod]
    public void NewEmployeeOnboardingModel_ShouldDeserialize_WhenAllRequiredFieldsPresent()
    {
        var json = """
        {
            "FirstName": "Jane",
            "UserPrincipalName": "jane.doe@example.com"
        }
        """;

        var model = JsonSerializer.Deserialize<NewEmployeeOnboardingModel>(json);

        Assert.IsNotNull(model);
        Assert.AreEqual("Jane", model.FirstName);
        Assert.AreEqual("jane.doe@example.com", model.UserPrincipalName);
    }

    [TestMethod]
    public void NewEmployeeOnboardingModel_OptionalFields_ShouldBeNullable()
    {
        var json = """
        {
            "FirstName": "Jane",
            "UserPrincipalName": "jane.doe@example.com"
        }
        """;

        var model = JsonSerializer.Deserialize<NewEmployeeOnboardingModel>(json);

        Assert.IsNotNull(model);
        Assert.IsNull(model.PwPushUrl);
        Assert.AreEqual(0, model.PwPushExpiresInDays);
        Assert.AreEqual(0, model.PwPushExpiresAfterViews);
        Assert.IsNull(model.Environment);
    }
}
