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
    /// Test class for GeoMap V2 API operations.
    /// </summary>
    public static class TestGeoMapV2
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
        /// Tests all GeoMap V2 API methods.
        /// </summary>
        public static async Task RunTest()
        {
            try
            {
                _logger?.LogInformation("Testing GeoMap V2 API calls...");

                using var client = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithTimeout(TimeSpan.FromSeconds(30))
                    .Build();
                
                //Test SearchBboxAsync
               await TestSearchBboxAsync(client);//ok//under maintaince

                // Test SearchAsync
                await TestSearchAsync(client); //ok

                // Test SearchRadiusAsync
                await TestSearchRadiusAsync(client);//ok

                // Test ReverseAsync
                await TestReverseAsync(client);//ok
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

        private static async Task TestSearchBboxAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing SearchBboxAsync ===");
            var request = new SearchBboxRequest
            {
                Q = "uttora",
                AcceptLanguage = "en",
                Page = 1,
                Limit = 10,
                TopLeftLat = 23.80159507531368,
                TopLeftLon = 90.43603219601864,
                BottomRightLat = 23.796686816815658,
                BottomRightLon = 90.43757714829312
            };//obj

            _logger?.LogInformation("Calling V2.SearchBboxAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogInformation("  BBox: ({TopLeftLat}, {TopLeftLon}) to ({BottomRightLat}, {BottomRightLon})",
                request.TopLeftLat, request.TopLeftLon,
                request.BottomRightLat, request.BottomRightLon);
            _logger?.LogDebug("Initiating API call to GeoMap.V2.SearchBboxAsync");

            string response = await client.GeoMap.V2.SearchBboxAsync(request);

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
                Q = "uttora",
                AcceptLanguage = "en",
                Page = 1,
                Limit = 10
            };//mthd

            _logger?.LogInformation("Calling V2.SearchAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogInformation("  Accept Language: {AcceptLanguage}", request.AcceptLanguage);
            _logger?.LogInformation("  Page: {Page}, Limit: {Limit}", request.Page, request.Limit);
            _logger?.LogDebug("Initiating API call to GeoMap.V2.SearchAsync");

            string response = await client.GeoMap.V2.SearchAsync(request);

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
                AcceptLanguage = "en",
                Page = 1,
                Limit = 10,
                ActiveLocations = false
            };//mthd

            _logger?.LogInformation("Calling V2.SearchRadiusAsync...");
            _logger?.LogInformation("  Query: {Query}", request.Q);
            _logger?.LogInformation("  Center: Lat={Lat}, Lon={Lon}", request.Lat, request.Lon);
            _logger?.LogInformation("  Radius: {Radius}", request.Radius);
            _logger?.LogInformation("  ActiveLocations: {ActiveLocations}", request.ActiveLocations);
            _logger?.LogDebug("Initiating API call to GeoMap.V2.SearchRadiusAsync");

            string response = await client.GeoMap.V2.SearchRadiusAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestReverseAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing ReverseAsync ===");
            var request = new ReverseRequest
            {
                Lat = 23.806950647749254,
                Lon = 90.42133700843839,
                AcceptLanguage = "en"
            };//mthd

            _logger?.LogInformation("Calling V2.ReverseAsync...");
            _logger?.LogInformation("  Lat: {Lat}, Lon: {Lon}", request.Lat, request.Lon);
            _logger?.LogInformation("  Accept Language: {AcceptLanguage}", request.AcceptLanguage);
            _logger?.LogDebug("Initiating API call to GeoMap.V2.ReverseAsync");

            string response = await client.GeoMap.V2.ReverseAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd
    }//cls
}//ns

