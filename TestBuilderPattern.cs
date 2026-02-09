using System;
using System.Threading.Tasks;
using MapNests.Public;
using MapNests.Public.Clients;
using MapNests.Public.Exceptions;
using MapNests.Public.Models;
using Microsoft.Extensions.Logging;

namespace MapNests.DotNet.SampleApp
{
    /// <summary>
    /// Test class for builder pattern examples.
    /// </summary>
    public static class TestBuilderPattern
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
        /// Tests the builder pattern with various configurations.
        /// </summary>
        public static async Task RunTest()
        {
            try
            {
                // Basic builder pattern
                _logger?.LogInformation("Testing basic builder pattern...");
                using var client = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .Build();

                // Health client removed - using GeoMap as example instead
                // bool isHealthy = await client.Health.IsHealthyAsync();
                // _logger?.LogInformation("  Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("  Client built successfully with builder pattern");

                // Builder with retry policy (using action)
                _logger?.LogInformation("\nTesting builder with retry policy (action)...");
                using var clientWithRetry = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithTimeout(TimeSpan.FromSeconds(60))
                    .WithRetryPolicy(policy =>
                    {
                        policy.MaxRetries = 3;
                        policy.InitialDelay = TimeSpan.FromSeconds(1);
                        policy.BackoffMultiplier = 2.0;
                        policy.MaxDelay = TimeSpan.FromSeconds(30);
                    })
                    .Build();

                // Health client removed
                // bool healthWithRetry = await clientWithRetry.Health.IsHealthyAsync();
                // _logger?.LogInformation("  Health Status: {Status}", healthWithRetry ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("  Client with retry policy built successfully");

                // Builder with explicit retry policy object
                _logger?.LogInformation("\nTesting builder with explicit retry policy object...");
                var retryPolicy = new RetryPolicyOptions
                {
                    MaxRetries = 5,
                    InitialDelay = TimeSpan.FromSeconds(2),
                    BackoffMultiplier = 1.5,
                    MaxDelay = TimeSpan.FromSeconds(60)
                };//mthd

                using var clientWithExplicitRetry = new MapNestsClientBuilder()
                    .WithApiKey(TestConfig.ApiKey)
                    .WithOrigin(TestConfig.Origin)
                    .WithRetryPolicy(retryPolicy)
                    .Build();

                // Health client removed
                // bool healthExplicitRetry = await clientWithExplicitRetry.Health.IsHealthyAsync();
                // _logger?.LogInformation("  Health Status: {Status}", healthExplicitRetry ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("  Client with explicit retry policy built successfully");
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

