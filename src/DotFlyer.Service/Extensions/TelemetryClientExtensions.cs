using Uniphar.Platform.Telemetry;

namespace DotFlyer.Service.Extensions;

public static class CustomEventTelemetryClientExtensions
{
    private static Dictionary<string, object> GetEventDictionary(string message)
    {
        return new()
        {
            { "Message", message }
        };
    }

    public static void TrackInvalidEmailPayload(this ICustomEventTelemetryClient telemetryClient, string message)
    {
        telemetryClient.TrackEvent("InvalidEmailPayload", GetEventDictionary(message));
    }

    public static void TrackInvalidSMSPayload(this ICustomEventTelemetryClient telemetryClient, string message)
    {
        telemetryClient.TrackEvent("InvalidSMSPayload", GetEventDictionary(message));
    }
}
