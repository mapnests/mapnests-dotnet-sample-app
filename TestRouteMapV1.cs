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
    /// Test class for RouteMap V1 API operations.
    /// </summary>
    public static class TestRouteMapV1
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
        /// Tests all RouteMap V1 API methods.
        /// </summary>
        public static async Task RunTest()
        {
            try
            {
                _logger?.LogInformation("Testing RouteMap V1 API calls...");

                using var client = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithTimeout(TimeSpan.FromSeconds(30))
                    .Build();


                // Test V1.MultiSourceSummaryAsync
                await TestMultiSourceSummaryAsync(client); 

                //Test PairwiseSummaryAsync
                await TestPairwiseSummaryAsync(client); 

                // Test MultiStopPointsAsync
                await TestMultiStopPointsAsync(client); 

                // Test DistanceMatrixDetailsAsync
                await TestDistanceMatrixDetailsAsync(client);

                // Test DistanceMatrixAsync
                await TestDistanceMatrixAsync(client);
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

        private static async Task TestMultiSourceSummaryAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing MultiSourceSummaryAsync ===");
            var request = new MultiSourceSummaryRequest
            {
                Sources =
                [
                    new SourcePoint
                    {
                        Id = 1,
                        Lat = 23.79899704146328,
                        Lon = 90.4373400551932,
                        Mode = RouteMode.Car
                    },
                    new SourcePoint
                    {
                        Id = 2,
                        Lat = 23.79955611490628,
                        Lon = 90.42467389787947,
                        Mode = RouteMode.Car
                    }
                ],
                Destination = new DestinationPoint
                {
                    Lat = 23.800886516288884,
                    Lon = 90.4484577784628
                }
            };//obj

            _logger?.LogInformation("Calling V1.MultiSourceSummaryAsync...");
            _logger?.LogInformation("  Sources: {SourceCount}", request.Sources?.Length ?? 0);
            _logger?.LogInformation("  Destination: Lat={Lat}, Lon={Lon}", request.Destination?.Lat, request.Destination?.Lon);
            _logger?.LogDebug("Initiating API call to RouteMap.V1.MultiSourceSummaryAsync");

            string response = await client.RouteMap.V1.MultiSourceSummaryAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestPairwiseSummaryAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing PairwiseSummaryAsync ===");
            var request = new PairwiseSummaryRequest
            {
                Pairs =
                [
                    new PairwisePair
                    {
                        Id = 1,
                        Src = new PairwisePoint { Lat = 23.8113, Lon = 90.4135 },
                        Dest = new PairwisePoint { Lat = 23.7815, Lon = 90.4123 },
                        Mode = RouteMode.Bicycle
                    },
                    new PairwisePair
                    {
                        Id = 2,
                        Src = new PairwisePoint { Lat = 23.8123, Lon = 90.4145 },
                        Dest = new PairwisePoint { Lat = 23.7825, Lon = 90.4133 },
                        Mode = RouteMode.Bicycle
                    },
                    new PairwisePair
                    {
                        Id = 3,
                        Src = new PairwisePoint { Lat = 23.8133, Lon = 90.4155 },
                        Dest = new PairwisePoint { Lat = 23.7835, Lon = 90.4143 },
                        Mode = RouteMode.Bicycle
                    }
                ]
            };//mthd

            _logger?.LogInformation("Calling V1.PairwiseSummaryAsync...");
            _logger?.LogInformation("  Pairs: {PairCount}", request.Pairs?.Length ?? 0);
            _logger?.LogDebug("Initiating API call to RouteMap.V1.PairwiseSummaryAsync");

            string response = await client.RouteMap.V1.PairwiseSummaryAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestMultiStopPointsAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing MultiStopPointsAsync ===");
            var request = new MultiStopPointsRequest
            {
                Src = new StopPointSource
                {
                    Lat = 23.809973415982903,
                    Lon = 90.35697149649764
                },
                StopPoints =
                [
                    new StopPoint { Id = 1, Lat = 23.81038738311683, Lon = 90.36203008256733 },
                    new StopPoint { Id = 2, Lat = 23.798308134287165, Lon = 90.43522641639149 }
                ],
                Mode = RouteMode.Bicycle
            };//mthd

            _logger?.LogInformation("Calling V1.MultiStopPointsAsync...");
            _logger?.LogInformation("  Stop Points: {StopPointCount}", request.StopPoints?.Length ?? 0);
            _logger?.LogDebug("Initiating API call to RouteMap.V1.MultiStopPointsAsync");

            string response = await client.RouteMap.V1.MultiStopPointsAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestDistanceMatrixDetailsAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing DistanceMatrixDetailsAsync ===");
            var request = new DistanceMatrixDetailsRequest
            {
                FromLat = 23.8100337,
                FromLong = 90.4164449,
                ToLat = 23.8029085,
                ToLong = 90.422608,
                Mode = RouteMode.Walking
            };//mthd

            _logger?.LogInformation("Calling V1.DistanceMatrixDetailsAsync...");
            _logger?.LogInformation("  From: Lat={FromLat}, Long={FromLong}", request.FromLat, request.FromLong);
            _logger?.LogInformation("  To: Lat={ToLat}, Long={ToLong}", request.ToLat, request.ToLong);
            _logger?.LogInformation("  Mode: {Mode}", request.Mode);
            _logger?.LogDebug("Initiating API call to RouteMap.V1.DistanceMatrixDetailsAsync");

            string response = await client.RouteMap.V1.DistanceMatrixDetailsAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestDistanceMatrixAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing DistanceMatrixAsync ===");
            var request = new DistanceMatrixRequest
            {
                FromLat = 23.8100337,
                FromLong = 90.4164449,
                ToLat = 23.8029085,
                ToLong = 90.422608,
                Mode = RouteMode.Walking
            };//mthd

            _logger?.LogInformation("Calling V1.DistanceMatrixAsync...");
            _logger?.LogInformation("  From: Lat={FromLat}, Long={FromLong}", request.FromLat, request.FromLong);
            _logger?.LogInformation("  To: Lat={ToLat}, Long={ToLong}", request.ToLat, request.ToLong);
            _logger?.LogInformation("  Mode: {Mode}", request.Mode);
            _logger?.LogDebug("Initiating API call to RouteMap.V1.DistanceMatrixAsync");

            string response = await client.RouteMap.V1.DistanceMatrixAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

    }//cls
}//ns

