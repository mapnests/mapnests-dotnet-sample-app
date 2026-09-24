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
    /// Test class for GeoMap API operations (SDK 2.0.0 — flat, no V1/V2 split).
    /// </summary>
    public static class TestGeoMap
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
        /// Tests all GeoMap API methods. Each sub-test is independently caught so one
        /// failure doesn't prevent the remaining methods from being demonstrated.
        /// </summary>
        public static async Task RunTest()
        {
            _logger?.LogInformation("Testing GeoMap API calls...");

            using var client = new MapNestsClientBuilder()
                .WithApiKey(TestConfig.ApiKey)
                .WithOrigin(TestConfig.Origin)
                .WithTimeout(TimeSpan.FromSeconds(30))
                .Build();

            await RunSubTest("DetailsAsync", () => TestDetailsAsync(client));
            await RunSubTest("SearchAsync", () => TestSearchAsync(client));
            await RunSubTest("ReverseAsync", () => TestReverseAsync(client));
            await RunSubTest("GeocodeAsync", () => TestGeocodeAsync(client));
            await RunSubTest("SearchBboxAsync", () => TestSearchBboxAsync(client));
            await RunSubTest("SearchRadiusAsync", () => TestSearchRadiusAsync(client));
            await RunSubTest("AutocompleteAsync", () => TestAutocompleteAsync(client));
        }//mthd

        private static async Task RunSubTest(string name, Func<Task> test)
        {
            try
            {
                await test();
            }//try
            catch (RequestValidationException ex)
            {
                _logger?.LogError(ex, "{Name}: Request Validation Error: {Message} (Parameter: {Parameter})",
                    name, ex.Message, ex.ParameterName);
            }//catch
            catch (ApiException ex)
            {
                _logger?.LogError(ex, "{Name}: API Error: {Message}", name, ex.Message);
                if (ex.StatusCode.HasValue)
                {
                    _logger?.LogError("Status Code: {StatusCode}", ex.StatusCode.Value);
                }//if
            }//catch
            catch (AntiDebuggingException ex)
            {
                _logger?.LogWarning(ex, "{Name}: Anti-debugging protection triggered: {Message}", name, ex.Message);
            }//catch
            catch (Exception ex)
            {
                _logger?.LogError(ex, "{Name}: Unexpected Error: {Message}", name, ex.Message);
                _logger?.LogDebug("Stack Trace: {StackTrace}", ex.StackTrace);
            }//catch
        }//mthd

        private static async Task TestDetailsAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing DetailsAsync ===");
            var request = new DetailsRequest
            {
                PlaceId = "ef3b457964a67698ea5ff86bb2456d6efd1875e9e1548c654b067e00660fd067"
            };//obj

            _logger?.LogInformation("Calling GeoMap.DetailsAsync...");
            _logger?.LogInformation("  Place ID: {PlaceId}", request.PlaceId);
            _logger?.LogDebug("Initiating API call to GeoMap.DetailsAsync");

            string response = await client.GeoMap.DetailsAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestSearchAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing SearchAsync ===");
            var request = new SearchRequest
            {
                Q = "us bangla airlines",
                AcceptLanguage = AcceptLanguages.En.GetValue(),
                Page = 1,
                Limit = 10
            };//obj

            _logger?.LogInformation("Calling GeoMap.SearchAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogDebug("Initiating API call to GeoMap.SearchAsync");

            string response = await client.GeoMap.SearchAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestReverseAsync(IMapNestsClient client)
        {
            // Also covers what used to be the separate ReverseGeocodeAsync method, which
            // was removed in 2.0.0 in favor of this single ReverseAsync(ReverseRequest).
            _logger?.LogInformation("\n=== Testing ReverseAsync ===");
            var request = new ReverseRequest
            {
                Lat = 23.806950647749254,
                Lon = 90.42133700843839,
                AcceptLanguage = AcceptLanguages.En.GetValue()
            };//obj

            _logger?.LogInformation("Calling GeoMap.ReverseAsync...");
            _logger?.LogInformation("  Lat: {Lat}, Lon: {Lon}", request.Lat, request.Lon);
            _logger?.LogInformation("  Accept Language: {AcceptLanguage}", request.AcceptLanguage);
            _logger?.LogDebug("Initiating API call to GeoMap.ReverseAsync");

            string response = await client.GeoMap.ReverseAsync(request);

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
                AcceptLanguage = AcceptLanguages.En.GetValue(),
                Limit = 5
            };//obj

            _logger?.LogInformation("Calling GeoMap.GeocodeAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogInformation("  AcceptLanguage: {AcceptLanguage}", request.AcceptLanguage);
            _logger?.LogInformation("  Limit: {Limit}", request.Limit);
            _logger?.LogDebug("Initiating API call to GeoMap.GeocodeAsync");

            string response = await client.GeoMap.GeocodeAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestSearchBboxAsync(IMapNestsClient client)
        {
            // Note: as of SDK 2.0.0, this endpoint is under maintenance server-side and
            // always returns a canned "under maintenance" response — that's expected.
            _logger?.LogInformation("\n=== Testing SearchBboxAsync (currently under maintenance server-side) ===");
            var request = new SearchBboxRequest
            {
                Q = "uttora",
                AcceptLanguage = AcceptLanguages.En.GetValue(),
                Page = 1,
                Limit = 10,
                TopLeftLat = 23.80159507531368,
                TopLeftLon = 90.43603219601864,
                BottomRightLat = 23.796686816815658,
                BottomRightLon = 90.43757714829312
            };//obj

            _logger?.LogInformation("Calling GeoMap.SearchBboxAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogInformation("  BBox: ({TopLeftLat}, {TopLeftLon}) to ({BottomRightLat}, {BottomRightLon})",
                request.TopLeftLat, request.TopLeftLon,
                request.BottomRightLat, request.BottomRightLon);
            _logger?.LogDebug("Initiating API call to GeoMap.SearchBboxAsync");

            string response = await client.GeoMap.SearchBboxAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestSearchRadiusAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing SearchRadiusAsync ===");
            var request = new SearchRadiusRequest
            {
                Q = "restaurant",
                Lat = 23.79899704146328,
                Lon = 90.4373400551932,
                Radius = "5000",
                AcceptLanguage = AcceptLanguages.En.GetValue(),
                Page = 1,
                Limit = 10,
                ActiveLocations = false
            };//obj

            _logger?.LogInformation("Calling GeoMap.SearchRadiusAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogInformation("  Center: Lat={Lat}, Lon={Lon}", request.Lat, request.Lon);
            _logger?.LogInformation("  Radius: {Radius}", request.Radius);
            _logger?.LogDebug("Initiating API call to GeoMap.SearchRadiusAsync");

            string response = await client.GeoMap.SearchRadiusAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestAutocompleteAsync(IMapNestsClient client)
        {
            // Replaces the removed AutocompleteAllAsync/AutocompleteAllRequest. Also uses
            // real Dhaka coordinates — the old test used Lat=11.1234/Lon=34.1234, which is
            // far outside Bangladesh and would now throw RequestValidationException under
            // SDK 2.0.0's client-side bounding-box validation.
            _logger?.LogInformation("\n=== Testing AutocompleteAsync ===");
            var request = new AutocompleteRequest
            {
                Q = "gulshan road",
                Lat = 23.7925,
                Lon = 90.4078,
                Radius = "2000",
                ZoneActiveOnly = false,
                AcceptLanguage = AcceptLanguages.En.GetValue(),
                Limit = 10
            };//obj

            _logger?.LogInformation("Calling GeoMap.AutocompleteAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogInformation("  Center: Lat={Lat}, Lon={Lon}, Radius={Radius}", request.Lat, request.Lon, request.Radius);
            _logger?.LogDebug("Initiating API call to GeoMap.AutocompleteAsync");

            string response = await client.GeoMap.AutocompleteAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd
    }//cls
}//ns
