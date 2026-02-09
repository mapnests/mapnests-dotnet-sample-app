using System.Threading.Tasks;
using MapNests.Public;
using MapNests.Public.Clients;
using MapNests.Public.Exceptions;
using Microsoft.Extensions.Logging;

namespace MapNests.DotNet.SampleApp
{
    /// <summary>
    /// Test class for manual client instantiation.
    /// </summary>
    public static class TestManualInstantiation
    {
        private static ILogger? _logger;

        /// <summary>
        /// Sets the logger instance for this test class.
        /// </summary>
        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }//mthd

        /// <summary>
        /// Tests manual client instantiation.
        /// </summary>
        public static async Task RunTest()
        {
            // Create client with API key and origin (client's origin, not API base URL)
            // Origin is where the SDK is being called from (e.g., your application's domain)
            // Using 'using' statement ensures proper disposal of HttpClient if SDK created one
            using var client = new MapNestsClientBuilder()
                .WithApiKey(TestConfig.ApiKey)
                .WithOrigin(TestConfig.Origin)
                .Build();

            try
            {
                // Test Health client - REMOVED: Health client no longer exists
                // _logger?.LogInformation("Testing Health Client...");
                // bool isHealthy = await client.Health.IsHealthyAsync();
                // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("Health Client removed - testing GeoMap instead");
            }//try
            catch (ApiException ex)
            {
                _logger?.LogError("API Error: {Message}", ex.Message);
                if (ex.StatusCode.HasValue)
                {
                    _logger?.LogError("Status Code: {StatusCode}", ex.StatusCode.Value);
                }//if
            }//catch
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd
    }//cls
}//ns

