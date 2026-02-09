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
    /// Test class for circuit breaker configuration and behavior.
    /// </summary>
    public static class TestCircuitBreaker
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
        /// Tests circuit breaker configuration and behavior.
        /// </summary>
        public static async Task RunTest()
        {
            await TestCircuitBreakerConfiguration();
            await TestCircuitBreakerOpenWithSimulatedFailures();
        }//mthd

        /// <summary>
        /// Tests that circuit breaker is correctly configured via builder.
        /// </summary>
        private static async Task TestCircuitBreakerConfiguration()
        {
            _logger?.LogInformation("\n=== Test: Circuit Breaker Configuration ===");
            try
            {
                var circuitBreakerOptions = new CircuitBreakerOptions
                {
                    FailureThreshold = 5,
                    DurationOfBreak = TimeSpan.FromSeconds(30),
                    SamplingDuration = TimeSpan.FromMinutes(1),
                    HalfOpenMaxAttempts = 1
                };//obj

                using var client = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithCircuitBreaker(circuitBreakerOptions)
                    .Build();

                _logger?.LogInformation("  Circuit Breaker: FailureThreshold={Threshold}, DurationOfBreak={Duration}s",
                    circuitBreakerOptions.FailureThreshold, circuitBreakerOptions.DurationOfBreak.TotalSeconds);

                var request = new MultiSourceSummaryRequest
                {
                    Sources = [new SourcePoint { Id = 1, Lat = 23.79, Lon = 90.43, Mode = RouteMode.Car }],
                    Destination = new DestinationPoint { Lat = 23.80, Lon = 90.44 }
                };
                var response = await client.RouteMap.V1.MultiSourceSummaryAsync(request);
                _logger?.LogInformation("  RouteMap call: {Status}", string.IsNullOrEmpty(response) ? "Empty" : "OK");
                _logger?.LogInformation("  ✓ Circuit breaker configured successfully");
            }//try
            catch (CircuitBreakerOpenException ex)
            {
                _logger?.LogInformation("  ✓ CircuitBreakerOpenException caught: {Message}", ex.Message);
            }//catch
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
        /// Tests circuit breaker open state with simulated persistent failures.
        /// Uses a custom HttpClient that always throws HttpRequestException.
        /// Circuit breaker records failures when operations throw. After FailureThreshold failures, circuit opens.
        /// </summary>
        private static async Task TestCircuitBreakerOpenWithSimulatedFailures()
        {
            _logger?.LogInformation("\n=== Test: Circuit Breaker Open with Simulated Failures ===");
            try
            {
                var handler = new CircuitBreakerTestHandler(); // Always throws

                using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };

                // Low threshold to trigger circuit open quickly
                var circuitBreakerOptions = new CircuitBreakerOptions
                {
                    FailureThreshold = 2,
                    DurationOfBreak = TimeSpan.FromSeconds(5),
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    HalfOpenMaxAttempts = 1
                };//obj

                using var client = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithHttpClient(httpClient)
                    .WithCircuitBreaker(circuitBreakerOptions)
                    .WithRetryPolicy(policy =>
                    {
                        policy.MaxRetries = 0; // No retries so each call = 1 failure
                        policy.InitialDelay = TimeSpan.FromMilliseconds(10);
                    })
                    .Build();

                _logger?.LogInformation("  Simulating: All requests throw HttpRequestException");
                _logger?.LogInformation("  Circuit Breaker: FailureThreshold=2 (circuit opens after 2 failures)");

                var req = new MultiSourceSummaryRequest
                {
                    Sources = [new SourcePoint { Id = 1, Lat = 23.79, Lon = 90.43, Mode = RouteMode.Car }],
                    Destination = new DestinationPoint { Lat = 23.80, Lon = 90.44 }
                };

                // First call - will throw
                try
                {
                    await client.RouteMap.V1.MultiSourceSummaryAsync(req);
                }//try
                catch (HttpRequestException)
                {
                    _logger?.LogInformation("  Call 1: Failed as expected (exception thrown)");
                }//catch

                // Second call - will throw, circuit opens
                try
                {
                    await client.RouteMap.V1.MultiSourceSummaryAsync(req);
                }//try
                catch (HttpRequestException)
                {
                    _logger?.LogInformation("  Call 2: Failed as expected - circuit should open");
                }//catch

                // Third call - circuit should be open, CircuitBreakerOpenException thrown
                try
                {
                    await client.RouteMap.V1.MultiSourceSummaryAsync(req);
                    _logger?.LogWarning("  Call 3: Unexpected success (circuit may not have opened)");
                }//try
                catch (CircuitBreakerOpenException ex)
                {
                    _logger?.LogInformation("  Call 3: ✓ CircuitBreakerOpenException caught as expected");
                    _logger?.LogInformation("  Message: {Message}", ex.Message);
                    _logger?.LogInformation("  ✓ Circuit breaker behavior verified: circuit opened after failures");
                }//catch
                catch (HttpRequestException)
                {
                    _logger?.LogInformation("  Call 3: HttpRequestException (circuit may have transitioned)");
                }//catch
            }//try
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd

        /// <summary>
        /// Custom HTTP handler that always throws for circuit breaker testing.
        /// Circuit breaker records failures when operations throw.
        /// </summary>
        private sealed class CircuitBreakerTestHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Simulated persistent failure for circuit breaker test");
            }//mthd
        }//cls
    }//cls
}//ns
