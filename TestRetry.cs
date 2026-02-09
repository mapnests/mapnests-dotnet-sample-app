// In the name of Allah

using System;
using System.Net.Http;
using System.Threading.Tasks;
using MapNests.Public.Clients;
using MapNests.Public.Exceptions;
using MapNests.Public.Models;
using MapNests.Public.Requests;
using Microsoft.Extensions.Logging;

namespace MapNests.DotNet.SampleApp
{
    /// <summary>
    /// Test class for retry policy configuration and behavior.
    /// </summary>
    public static class TestRetry
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
        /// Tests retry policy configuration and behavior.
        /// </summary>
        public static async Task RunTest()
        {
            await TestRetryConfiguration();
            await TestRetryWithSimulatedFailures();
        }//mthd

        /// <summary>
        /// Tests that retry policy is correctly configured via builder.
        /// </summary>
        private static async Task TestRetryConfiguration()
        {
            _logger?.LogInformation("\n=== Test: Retry Policy Configuration ===");
            try
            {
                var retryPolicy = new RetryPolicyOptions
                {
                    MaxRetries = 3,
                    InitialDelay = TimeSpan.FromMilliseconds(100),
                    BackoffMultiplier = 2.0,
                    MaxDelay = TimeSpan.FromSeconds(5)
                };//obj

                using var client = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithRetryPolicy(retryPolicy)
                    .Build();

                _logger?.LogInformation("  Retry Policy: MaxRetries={MaxRetries}, InitialDelay={InitialDelay}ms, BackoffMultiplier={Multiplier}",
                    retryPolicy.MaxRetries, retryPolicy.InitialDelay.TotalMilliseconds, retryPolicy.BackoffMultiplier);

                var request = new MultiSourceSummaryRequest
                {
                    Sources = [new SourcePoint { Id = 1, Lat = 23.79, Lon = 90.43, Mode = RouteMode.Car }],
                    Destination = new DestinationPoint { Lat = 23.80, Lon = 90.44 }
                };
                var response = await client.RouteMap.V1.MultiSourceSummaryAsync(request);
                _logger?.LogInformation("  RouteMap call: {Status}", string.IsNullOrEmpty(response) ? "Empty" : "OK");
                _logger?.LogInformation("  ✓ Retry policy configured successfully");
            }//try
            catch (ApiException ex)
            {
                _logger?.LogError("API Error: {Message}", ex.Message);
            }//catch
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd

        /// <summary>
        /// Tests retry behavior with simulated transient failures.
        /// Uses a custom HttpClient that throws HttpRequestException for the first 2 requests, then succeeds.
        /// Retry logic triggers on thrown exceptions (network errors), not on 500 status codes.
        /// </summary>
        private static async Task TestRetryWithSimulatedFailures()
        {
            _logger?.LogInformation("\n=== Test: Retry with Simulated Transient Failures ===");
            try
            {
                var callCount = 0;
                var handler = new RetryTestHandler(() =>
                {
                    callCount++;
                    return callCount <= 2; // Throw first 2 times, succeed on 3rd
                });

                using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };

                using var client = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithHttpClient(httpClient)
                    .WithRetryPolicy(policy =>
                    {
                        policy.MaxRetries = 3;
                        policy.InitialDelay = TimeSpan.FromMilliseconds(50);
                        policy.BackoffMultiplier = 2.0;
                        policy.MaxDelay = TimeSpan.FromSeconds(2);
                    })
                    .Build();

                _logger?.LogInformation("  Simulating: 2 transient HttpRequestExceptions, then success");
                _logger?.LogInformation("  Retry Policy: MaxRetries=3 (retries on network errors)");

                var req = new MultiSourceSummaryRequest
                {
                    Sources = [new SourcePoint { Id = 1, Lat = 23.79, Lon = 90.43, Mode = RouteMode.Car }],
                    Destination = new DestinationPoint { Lat = 23.80, Lon = 90.44 }
                };
                var result = await client.RouteMap.V1.MultiSourceSummaryAsync(req);
                _logger?.LogInformation("  Total requests made (including retries): {Count}", callCount);
                _logger?.LogInformation("  Request result: {Status}", string.IsNullOrEmpty(result) ? "Empty" : "OK");

                if (callCount >= 3 && !string.IsNullOrEmpty(result))
                {
                    _logger?.LogInformation("  ✓ Retry behavior verified: failed twice, succeeded on retry");
                }//if
                else if (!string.IsNullOrEmpty(result))
                {
                    _logger?.LogInformation("  ✓ Request succeeded");
                }//else if
            }//try
            catch (ApiException ex)
            {
                _logger?.LogError("API Error: {Message} (StatusCode: {Code})", ex.Message, ex.StatusCode);
            }//catch
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd

        /// <summary>
        /// Custom HTTP handler that simulates transient failures (throws) for retry testing.
        /// Retry triggers on HttpRequestException, not on 500 status codes.
        /// </summary>
        private sealed class RetryTestHandler : HttpMessageHandler
        {
            private readonly Func<bool> _shouldThrow;

            public RetryTestHandler(Func<bool> shouldThrow)
            {
                _shouldThrow = shouldThrow ?? throw new ArgumentNullException(nameof(shouldThrow));
            }//ctor

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (_shouldThrow())
                {
                    throw new HttpRequestException("Simulated transient network error for retry test");
                }//if

                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });
            }//mthd
        }//cls
    }//cls
}//ns
