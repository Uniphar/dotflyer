namespace DotFlyer.EmailTemplates.Tests;

[TestClass]
public static class TestRunSetup
{
    [AssemblyInitialize]
    public static void CleanOutputDirectories(TestContext _)
    {
        var outputDirs = new[]
        {
            Path.Combine(Environment.CurrentDirectory, "TestOutputs"),
            Path.Combine(Environment.CurrentDirectory, "PlaywrightTestOutputs")
        };

        foreach (var dir in outputDirs)
        {
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, recursive: true);
            }
        }
    }
}
