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
    /// Test class for RouteMap API operations (SDK 2.0.0 — flat, no V1/V2 split).
    /// </summary>
    public static class TestRouteMap
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
        /// Tests all RouteMap API methods. Each sub-test is independently caught so one
        /// failure doesn't prevent the remaining methods from being demonstrated.
        /// </summary>
        public static async Task RunTest()
        {
            _logger?.LogInformation("Testing RouteMap API calls...");

            using var client = new MapNestsClientBuilder()
                .WithApiKey(TestConfig.ApiKey)
                .WithOrigin(TestConfig.Origin)
                .WithTimeout(TimeSpan.FromSeconds(30))
                .Build();

            await RunSubTest("MultiSourceSummaryAsync", () => TestMultiSourceSummaryAsync(client));
            await RunSubTest("MultiSourceSummaryWithoutGeometryAsync", () => TestMultiSourceSummaryWithoutGeometryAsync(client));
            await RunSubTest("PairwiseSummaryAsync", () => TestPairwiseSummaryAsync(client));
            await RunSubTest("PairwiseDistanceMatrixAsync", () => TestPairwiseDistanceMatrixAsync(client));
            await RunSubTest("MultiStopPointsAsync", () => TestMultiStopPointsAsync(client));
            await RunSubTest("DistanceMatrixDetailsAsync", () => TestDistanceMatrixDetailsAsync(client));
            await RunSubTest("DistanceMatrixAsync", () => TestDistanceMatrixAsync(client));
            await RunSubTest("SnapToRoadAsync", () => TestSnapToRoadAsync(client));
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

            _logger?.LogInformation("Calling RouteMap.MultiSourceSummaryAsync...");
            _logger?.LogInformation("  Sources: {SourceCount}", request.Sources?.Length ?? 0);
            _logger?.LogInformation("  Destination: Lat={Lat}, Lon={Lon}", request.Destination?.Lat, request.Destination?.Lon);
            _logger?.LogDebug("Initiating API call to RouteMap.MultiSourceSummaryAsync");

            string response = await client.RouteMap.MultiSourceSummaryAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestMultiSourceSummaryWithoutGeometryAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing MultiSourceSummaryWithoutGeometryAsync ===");
            var request = new MultiSourceSummaryWithoutGeometryRequest
            {
                Sources =
                [
                    new MultiSourceSummaryWithoutGeometryRequest.SourcePoint
                    {
                        Id = 1,
                        Lat = 23.79899704146328,
                        Lon = 90.4373400551932,
                        Mode = RouteMode.Car
                    },
                    new MultiSourceSummaryWithoutGeometryRequest.SourcePoint
                    {
                        Id = 2,
                        Lat = 23.79955611490628,
                        Lon = 90.42467389787947,
                        Mode = RouteMode.Car
                    }
                ],
                Destination = new MultiSourceSummaryWithoutGeometryRequest.DestinationPoint
                {
                    Lat = 23.800886516288884,
                    Lon = 90.4484577784628
                }
            };//obj

            _logger?.LogInformation("Calling RouteMap.MultiSourceSummaryWithoutGeometryAsync...");
            _logger?.LogInformation("  Sources: {SourceCount}", request.Sources?.Length ?? 0);
            _logger?.LogInformation("  Destination: Lat={Lat}, Lon={Lon}", request.Destination?.Lat, request.Destination?.Lon);
            _logger?.LogDebug("Initiating API call to RouteMap.MultiSourceSummaryWithoutGeometryAsync");

            string response = await client.RouteMap.MultiSourceSummaryWithoutGeometryAsync(request);

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
            };//obj

            _logger?.LogInformation("Calling RouteMap.PairwiseSummaryAsync...");
            _logger?.LogInformation("  Pairs: {PairCount}", request.Pairs?.Length ?? 0);
            _logger?.LogDebug("Initiating API call to RouteMap.PairwiseSummaryAsync");

            string response = await client.RouteMap.PairwiseSummaryAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestPairwiseDistanceMatrixAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing PairwiseDistanceMatrixAsync ===");
            var request = new PairwiseDistanceMatrixRequest
            {
                Mode = RouteMode.Car,
                Sort = SortOption.DistanceInMetresAsc,
                Routes =
                [
                    new PairwiseDistanceMatrixRequest.Route
                    {
                        Id = 1,
                        Origin = new PairwiseDistanceMatrixRequest.RoutePoint { Lat = 23.8100337, Lon = 90.4164449 },
                        Destination = new PairwiseDistanceMatrixRequest.RoutePoint { Lat = 23.8029085, Lon = 90.422608 }
                    },
                    new PairwiseDistanceMatrixRequest.Route
                    {
                        Id = 2,
                        Origin = new PairwiseDistanceMatrixRequest.RoutePoint { Lat = 23.79899704146328, Lon = 90.4373400551932 },
                        Destination = new PairwiseDistanceMatrixRequest.RoutePoint { Lat = 23.800886516288884, Lon = 90.4484577784628 }
                    }
                ]
            };//obj

            _logger?.LogInformation("Calling RouteMap.PairwiseDistanceMatrixAsync...");
            _logger?.LogInformation("  Routes: {RouteCount}, Mode: {Mode}, Sort: {Sort}", request.Routes?.Length ?? 0, request.Mode, request.Sort);
            _logger?.LogDebug("Initiating API call to RouteMap.PairwiseDistanceMatrixAsync");

            string response = await client.RouteMap.PairwiseDistanceMatrixAsync(request);

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
            };//obj

            _logger?.LogInformation("Calling RouteMap.MultiStopPointsAsync...");
            _logger?.LogInformation("  Stop Points: {StopPointCount}", request.StopPoints?.Length ?? 0);
            _logger?.LogDebug("Initiating API call to RouteMap.MultiStopPointsAsync");

            string response = await client.RouteMap.MultiStopPointsAsync(request);

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
            };//obj

            _logger?.LogInformation("Calling RouteMap.DistanceMatrixDetailsAsync...");
            _logger?.LogInformation("  From: Lat={FromLat}, Long={FromLong}", request.FromLat, request.FromLong);
            _logger?.LogInformation("  To: Lat={ToLat}, Long={ToLong}", request.ToLat, request.ToLong);
            _logger?.LogInformation("  Mode: {Mode}", request.Mode);
            _logger?.LogDebug("Initiating API call to RouteMap.DistanceMatrixDetailsAsync");

            string response = await client.RouteMap.DistanceMatrixDetailsAsync(request);

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
            };//obj

            _logger?.LogInformation("Calling RouteMap.DistanceMatrixAsync...");
            _logger?.LogInformation("  From: Lat={FromLat}, Long={FromLong}", request.FromLat, request.FromLong);
            _logger?.LogInformation("  To: Lat={ToLat}, Long={ToLong}", request.ToLat, request.ToLong);
            _logger?.LogInformation("  Mode: {Mode}", request.Mode);
            _logger?.LogDebug("Initiating API call to RouteMap.DistanceMatrixAsync");

            string response = await client.RouteMap.DistanceMatrixAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd

        private static async Task TestSnapToRoadAsync(IMapNestsClient client)
        {
            _logger?.LogInformation("\n=== Testing SnapToRoadAsync ===");
            var request = new SnapToRoadRequest
            {
                Mode = RouteMode.Car,
                Lat = 23.8100337,
                Lon = 90.4164449
            };//obj

            _logger?.LogInformation("Calling RouteMap.SnapToRoadAsync...");
            _logger?.LogInformation("  Lat: {Lat}, Lon: {Lon}, Mode: {Mode}", request.Lat, request.Lon, request.Mode);
            _logger?.LogDebug("Initiating API call to RouteMap.SnapToRoadAsync");

            string response = await client.RouteMap.SnapToRoadAsync(request);

            _logger?.LogInformation("Response received:");
            _logger?.LogInformation("==================");
            _logger?.LogInformation("{Response}", response);
            _logger?.LogInformation("==================");
        }//mthd
    }//cls
}//ns
