using DotFlyer.Common.EmailTemplates;
using System.Text.Json;

namespace DotFlyer.EmailTemplates.Tests;

[TestClass, TestCategory("Unit")]
public class ManualSecretRotationModelTests
{
    [TestMethod]
    public void ManualSecretRotationModel_ShouldRequire_ResourceName()
    {
        var json = """
        {
            "SecretName": "TestSecret",
            "KeyVaults": ["https://kv1.vault.local"]
        }
        """;

        try
        {
            JsonSerializer.Deserialize<ManualSecretRotationModel>(json);
            Assert.Fail("Expected JsonException but no exception was thrown");
        }
        catch (JsonException ex)
        {
            Assert.IsTrue(ex.Message.Contains("ResourceName") || ex.Message.Contains("required"),
                $"Expected deserialization to fail for missing ResourceName. Actual message: {ex.Message}");
        }
    }

    [TestMethod]
    public void ManualSecretRotationModel_ShouldRequire_KeyVaults()
    {
        var json = """
        {
            "ResourceName": "TestDatabase",
            "SecretName": "TestSecret"
        }
        """;

        try
        {
            JsonSerializer.Deserialize<ManualSecretRotationModel>(json);
            Assert.Fail("Expected JsonException but no exception was thrown");
        }
        catch (JsonException ex)
        {
            Assert.IsTrue(ex.Message.Contains("KeyVaults") || ex.Message.Contains("required"),
                $"Expected deserialization to fail for missing KeyVaults. Actual message: {ex.Message}");
        }
    }

    [TestMethod]
    public void ManualSecretRotationModel_ShouldRequire_SecretName()
    {
        var json = """
        {
            "ResourceName": "TestDatabase",
            "KeyVaults": ["https://kv1.vault.local"]
        }
        """;

        try
        {
            JsonSerializer.Deserialize<ManualSecretRotationModel>(json);
            Assert.Fail("Expected JsonException but no exception was thrown");
        }
        catch (JsonException ex)
        {
            Assert.IsTrue(ex.Message.Contains("SecretName") || ex.Message.Contains("required"),
                $"Expected deserialization to fail for missing SecretName. Actual message: {ex.Message}");
        }
    }

    [TestMethod]
    public void ManualSecretRotationModel_ShouldDeserialize_WhenAllRequiredFieldsPresent()
    {
        var json = """
        {
            "ResourceName": "TestDatabase",
            "KeyVaults": ["https://kv1.vault.local", "https://kv2.vault.local"],
            "SecretName": "TestSecret"
        }
        """;

        var model = JsonSerializer.Deserialize<ManualSecretRotationModel>(json);

        Assert.IsNotNull(model);
        Assert.AreEqual("TestDatabase", model.ResourceName);
        Assert.AreEqual("TestSecret", model.SecretName);
        Assert.AreEqual(2, model.KeyVaults.Count());
    }

    [TestMethod]
    public void ManualSecretRotationModel_OptionalFields_ShouldBeNullable()
    {
        var json = """
        {
            "ResourceName": "TestDatabase",
            "KeyVaults": ["https://kv1.vault.local"],
            "SecretName": "TestSecret"
        }
        """;

        var model = JsonSerializer.Deserialize<ManualSecretRotationModel>(json);

        Assert.IsNotNull(model);
        Assert.IsNull(model.TenantId);
        Assert.IsNull(model.AppId);
        Assert.IsNull(model.PwPushUrl);
        Assert.AreEqual(0, model.PwPushExpiresInDays);
        Assert.AreEqual(0, model.PwPushExpiresAfterViews);
        Assert.AreEqual(default(DateTime), model.OldSecretDeletionDateUtc);
    }
}
