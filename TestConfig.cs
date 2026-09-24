using System;
using System.IO;
using System.Text.Json;

namespace MapNests.DotNet.SampleApp
{
    /// <summary>
    /// Simple configuration helper for test app.
    /// Loads API key and origin from appsettings.json.
    /// </summary>
    internal static class TestConfig
    {
        private const string DefaultApiKey = "3Lldiceu7dHeubYt9e8nV7rjK4S8cUYsubUP-Iq9uF5G1LebKPPxwX586WbA8JW4BOBu7RpaKDPUhIZAPokWdA";
        private const string DefaultOrigin = "https://cms.foodibd.com/";

        public static string ApiKey { get; } = Load("ApiKey", DefaultApiKey);
        public static string Origin { get; } = Load("Origin", DefaultOrigin);

        private static string Load(string name, string fallback)
        {
            try
            {
                var baseDir = AppContext.BaseDirectory;
                var path = Path.Combine(baseDir, "appsettings.json");

                if (!File.Exists(path))
                {
                    return fallback;
                }//if

                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                if (doc.RootElement.TryGetProperty("MapNests", out var mapNests) &&
                    mapNests.TryGetProperty(name, out var valueElement) &&
                    valueElement.ValueKind == JsonValueKind.String)
                {
                    var value = valueElement.GetString();
                    return string.IsNullOrWhiteSpace(value) ? fallback : value!;
                }//if
            }//try
            catch
            {
                // Ignore and fall back.
            }//catch

            return fallback;
        }//mthd
    }//cls
}//ns


