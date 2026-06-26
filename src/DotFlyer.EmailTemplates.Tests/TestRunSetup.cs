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
            if (!Directory.Exists(dir))
            {
                continue;
            }

            try
            {
                Directory.Delete(dir, recursive: true);
            }
            catch (IOException)
            {
                // Best-effort cleanup; ignore file locks.
            }
            catch (UnauthorizedAccessException)
            {
                // Best-effort cleanup; ignore permission issues.
            }
        }
    }
}
