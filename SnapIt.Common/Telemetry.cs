﻿//using Microsoft.ApplicationInsights;
//using Microsoft.ApplicationInsights.Extensibility;

namespace SnapIt.Common;

public static class Telemetry
{
    //private const string TelemetryKey = "040b9d3e-6d04-4090-89df-e6da11a66d20";

    //private static TelemetryClient _telemetry = GetAppInsightsClient();

    public static bool Enabled { get; set; } = true;

    //        private static TelemetryClient GetAppInsightsClient()
    //        {
    //            var config = new TelemetryConfiguration();
    //            config.InstrumentationKey = TelemetryKey;
    //            config.TelemetryChannel = new Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.ServerTelemetryChannel();
    //            //config.TelemetryChannel = new Microsoft.ApplicationInsights.Channel.InMemoryChannel(); // Default channel
    //            config.TelemetryChannel.DeveloperMode = Debugger.IsAttached;
    //#if DEBUG
    //            config.TelemetryChannel.DeveloperMode = true;
    //#endif
    //            TelemetryClient client = new TelemetryClient(config);
    //            client.Context.Component.Version = Constants.AppVersion;// Assembly.GetEntryAssembly().GetName().Version.ToString();
    //            client.Context.Session.Id = Guid.NewGuid().ToString();
    //            client.Context.User.Id = (Environment.UserName + Environment.MachineName).GetHashCode().ToString();
    //            client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
    //            return client;
    //        }

    public static void SetUser(string user)
    {
        //_telemetry.Context.User.AuthenticatedUserId = user;
    }

    public static void TrackEvent(string key, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
    {
        //if (Enabled)
        //{
        //    _telemetry.TrackEvent(key, properties, metrics);
        //}
    }

    public static void TrackException(Exception ex)
    {
        //if (ex != null && Enabled)
        //{
        //    var telex = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(ex);
        //    _telemetry.TrackException(telex);
        //    Flush();
        //}
    }

    public static void TrackException(Exception ex, string note = null)
    {
        //if (ex != null && Enabled)
        //{
        //    if (!string.IsNullOrEmpty(note))
        //    {
        //        var properties = new Dictionary<string, string>();
        //        properties.Add("dev note", note);

        //        _telemetry.TrackException(ex, properties);
        //    }
        //    else
        //    {
        //        var telex = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(ex);

        //        _telemetry.TrackException(telex);
        //    }

        //    Flush();
        //}
    }

    internal static void Flush()
    {
        //_telemetry.Flush();
    }
}