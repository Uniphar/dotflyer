namespace DotFlyer.Service.Extensions;

public static class TelemetryClientExtensions
{
    private static Dictionary<string, string> GetEventDictionary(string message)
    {
        return new()
        {
            { "Message", message }
        };
    }

    public static void TrackInvalidEmailPayload(this TelemetryClient telemetryClient, string message)
    {
        telemetryClient.TrackEvent("InvalidEmailPayload", GetEventDictionary(message));
    }

    public static void TrackInvalidSMSPayload(this TelemetryClient telemetryClient, string message)
    {
        telemetryClient.TrackEvent("InvalidSMSPayload", GetEventDictionary(message));
    }
}
