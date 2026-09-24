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
    /// Test class for the new Eta API introduced in SDK 2.0.0.
    /// </summary>
    public static class TestEta
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
        /// Tests all Eta API methods. Each sub-test is independently caught so one
        /// failure doesn't prevent the remaining methods from being demonstrated.
        /// </summary>
        public static async Task RunTest()
        {
            _logger?.LogInformation("Testing Eta API calls...");

            using var client = new MapNestsClientBuilder()
                .WithApiKey(TestConfig.ApiKey)
                .WithOrigin(TestConfig.Origin)
                .WithTimeout(TimeSpan.FromSeconds(30))
                .Build();

            await RunSubTest("EtaWithoutGeometryByModeAsync", () => TestEtaWithoutGeometryByModeAsync(client));
            await RunSubTest("EtaWithGeometryByModeAsync", () => TestEtaWithGeometryByModeAsync(client));
            await RunSubTest("EtaWithoutStoppageAsync", () => TestEtaWithoutStoppageAsync(client));
            await RunSubTest("EtaMultiStoppageAsync", () => TestEtaMultiStoppageAsync(client));
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

        private static async Task TestEtaWithoutGeometryByModeAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing EtaWithoutGeometryByModeAsync ===");
            var request = new EtaWithoutGeometryByModeRequest
            {
                FromLat = 23.8100337,
                FromLon = 90.4164449,
                ToLat = 23.8029085,
                ToLon = 90.422608,
                Mode = EtaTravelMode.Car,
                RouteCount = 3
            };//obj

            _logger?.LogInformation("Calling Eta.EtaWithoutGeometryByModeAsync...");
            _logger?.LogInformation("  From: Lat={FromLat}, Lon={FromLon}", request.FromLat, request.FromLon);
            _logger?.LogInformation("  To: Lat={ToLat}, Lon={ToLon}", request.ToLat, request.ToLon);
            _logger?.LogInformation("  Mode: {Mode}, RouteCount: {RouteCount}", request.Mode, request.RouteCount);
            _logger?.LogDebug("Initiating API call to Eta.EtaWithoutGeometryByModeAsync");

            string response = await client.Eta.EtaWithoutGeometryByModeAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestEtaWithGeometryByModeAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing EtaWithGeometryByModeAsync ===");
            var request = new EtaWithGeometryByModeRequest
            {
                FromLat = 23.8100337,
                FromLon = 90.4164449,
                ToLat = 23.8029085,
                ToLon = 90.422608,
                Mode = EtaTravelMode.Car,
                TripId = "sample-trip-1"
            };//obj

            _logger?.LogInformation("Calling Eta.EtaWithGeometryByModeAsync...");
            _logger?.LogInformation("  From: Lat={FromLat}, Lon={FromLon}", request.FromLat, request.FromLon);
            _logger?.LogInformation("  To: Lat={ToLat}, Lon={ToLon}", request.ToLat, request.ToLon);
            _logger?.LogInformation("  Mode: {Mode}, TripId: {TripId}", request.Mode, request.TripId);
            _logger?.LogDebug("Initiating API call to Eta.EtaWithGeometryByModeAsync");

            string response = await client.Eta.EtaWithGeometryByModeAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestEtaWithoutStoppageAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing EtaWithoutStoppageAsync ===");
            var request = new EtaWithoutStoppageRequest
            {
                FromLat = 23.8100337,
                FromLon = 90.4164449,
                ToLat = 23.8029085,
                ToLon = 90.422608
            };//obj

            _logger?.LogInformation("Calling Eta.EtaWithoutStoppageAsync...");
            _logger?.LogInformation("  From: Lat={FromLat}, Lon={FromLon}", request.FromLat, request.FromLon);
            _logger?.LogInformation("  To: Lat={ToLat}, Lon={ToLon}", request.ToLat, request.ToLon);
            _logger?.LogDebug("Initiating API call to Eta.EtaWithoutStoppageAsync");

            string response = await client.Eta.EtaWithoutStoppageAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestEtaMultiStoppageAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing EtaMultiStoppageAsync ===");
            var request = new EtaMultiStoppageRequest
            {
                Source = new EtaMultiStoppageRequest.EtaPoint { Lat = 23.809973415982903, Lon = 90.35697149649764 },
                Dropoff = new EtaMultiStoppageRequest.EtaPoint { Lat = 23.800886516288884, Lon = 90.4484577784628 },
                Stoppages =
                [
                    new EtaMultiStoppageRequest.EtaStoppage { Lat = 23.81038738311683, Lon = 90.36203008256733, StoppageOrder = 1 },
                    new EtaMultiStoppageRequest.EtaStoppage { Lat = 23.798308134287165, Lon = 90.43522641639149, StoppageOrder = 2 }
                ],
                Modes = [EtaTravelMode.Car, EtaTravelMode.Motorcycle],
                TripId = "sample-trip-multi"
            };//obj

            _logger?.LogInformation("Calling Eta.EtaMultiStoppageAsync...");
            _logger?.LogInformation("  Stoppages: {StoppageCount}, Modes: {ModeCount}, TripId: {TripId}",
                request.Stoppages?.Length ?? 0, request.Modes?.Length ?? 0, request.TripId);
            _logger?.LogDebug("Initiating API call to Eta.EtaMultiStoppageAsync");

            string response = await client.Eta.EtaMultiStoppageAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd
    }//cls
}//ns
