using System;
using System.Threading.Tasks;
using MapNests.Public;
using MapNests.Public.Clients;
using MapNests.Public.Exceptions;
using MapNests.Public.Requests;
using Microsoft.Extensions.Logging;

namespace MapNests.DotNet.SampleApp
{
    /// <summary>
    /// Test class for GeoMap V1 API operations.
    /// </summary>
    public static class TestGeoMapV1
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
        /// Tests all GeoMap V1 API methods.
        /// </summary>
        public static async Task RunTest()
        {
            try
            {
                _logger?.LogInformation("Testing GeoMap V1 API calls...");

                using var client = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithTimeout(TimeSpan.FromSeconds(30))
                    .Build();

                // Test DetailsAsync
                await TestDetailsAsync(client);

                // Test SearchAsync
                await TestSearchAsync(client);

                // Test ReverseAsync
                await TestReverseAsync(client);

                // Test ReverseGeocodeAsync
                await TestReverseGeocodeAsync(client);

                // Test GeocodeAsync
                await TestGeocodeAsync(client);

                // Test AutocompleteAllAsync
                await TestAutocompleteAllAsync(client);
            }//try
            catch (ApiException ex)
            {
                _logger?.LogError(ex, "API Error: {Message}", ex.Message);
                if (ex.StatusCode.HasValue)
                {
                    _logger?.LogError("Status Code: {StatusCode}", ex.StatusCode.Value);
                }//if
            }//catch
            catch (AntiDebuggingException ex)
            {
                _logger?.LogWarning(ex, "Anti-debugging protection triggered: {Message}", ex.Message);
            }//catch
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Unexpected Error: {Message}", ex.Message);
                _logger?.LogDebug("Stack Trace: {StackTrace}", ex.StackTrace);
            }//catch
        }//mthd

        private static async Task TestDetailsAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing DetailsAsync ===");
            var request = new DetailsRequest
            {
                PlaceId = "f7c9eac365c0e634d9387d55761455c365c66413c5a78143c9d583187d27b1f8"
            };//obj

            _logger?.LogInformation("Calling V1.DetailsAsync...");
            _logger?.LogInformation("  Place ID: {PlaceId}", request.PlaceId);
            _logger?.LogDebug("Initiating API call to GeoMap.V1.DetailsAsync");

            string response = await client.GeoMap.V1.DetailsAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestSearchAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing SearchAsync ===");
            var request = new SearchV1Request
            {
                Q = "us bangla airlines"
            };//obj

            _logger?.LogInformation("Calling V1.SearchAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogDebug("Initiating API call to GeoMap.V1.SearchAsync");

            string response = await client.GeoMap.V1.SearchAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestReverseAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing ReverseAsync ===");
            var request = new ReverseV1Request
            {
                Lat = 23.806950647749254,
                Lon = 90.42133700843839,
                Language = "en"
            };//obj

            _logger?.LogInformation("Calling V1.ReverseAsync...");
            _logger?.LogInformation("  Lat: {Lat}, Lon: {Lon}", request.Lat, request.Lon);
            _logger?.LogInformation("  Language: {Language}", request.Language);
            _logger?.LogDebug("Initiating API call to GeoMap.V1.ReverseAsync");

            string response = await client.GeoMap.V1.ReverseAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestReverseGeocodeAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing ReverseGeocodeAsync ===");
            var request = new ReverseV1Request
            {
                Lat = 23.80631,
                Lon = 90.41889,
                Language = "en"
            };//obj

            _logger?.LogInformation("Calling V1.ReverseGeocodeAsync...");
            _logger?.LogInformation("  Lat: {Lat}, Lon: {Lon}", request.Lat, request.Lon);
            _logger?.LogInformation("  Language: {Language}", request.Language);
            _logger?.LogDebug("Initiating API call to GeoMap.V1.ReverseGeocodeAsync");

            string response = await client.GeoMap.V1.ReverseGeocodeAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestGeocodeAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing GeocodeAsync ===");
            var request = new GeocodeRequest
            {
                Q = "uttara",
                AcceptLanguage = "en",
                Limit = 5
            };//obj

            _logger?.LogInformation("Calling V1.GeocodeAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogInformation("  AcceptLanguage: {AcceptLanguage}", request.AcceptLanguage);
            _logger?.LogInformation("  Limit: {Limit}", request.Limit);
            _logger?.LogDebug("Initiating API call to GeoMap.V1.GeocodeAsync");

            string response = await client.GeoMap.V1.GeocodeAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestAutocompleteAllAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing AutocompleteAllAsync (GeoMap V1) ===");
            var request = new AutocompleteAllRequest
            {
                Query = "gulshan road",
                Lat = 11.1234,
                Lon = 34.1234,
                Radius = 10,
                AcceptLanguage = "en",
                Limit = 10
            };//obj

            _logger?.LogInformation("Calling GeoMap.V1.AutocompleteAllAsync...");
            _logger?.LogDebug("Initiating API call to GeoMap.V1.AutocompleteAllAsync");

            string response = await client.GeoMap.V1.AutocompleteAllAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd
    }//cls
}//ns

