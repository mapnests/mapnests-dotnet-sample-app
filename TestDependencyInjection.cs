using System;
using System.Threading.Tasks;
using MapNests.Public;
using MapNests.Public.Clients;
using MapNests.Public.Exceptions;
using MapNests.Public.Models;
using MapNests.Public.Requests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MapNests.DotNet.SampleApp
{
    /// <summary>
    /// Test class for dependency injection examples.
    /// </summary>
    public static class TestDependencyInjection
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
        /// Tests all dependency injection scenarios.
        /// </summary>
        public static async Task RunTest()
        {
            // Test 1: Basic AddMapNests with API key and origin
            await TestBasicConfiguration();

            // Test 2: With timeout
            await TestWithTimeout();

            // Test 3: With retry policy
            await TestWithRetryPolicy();

            // Test 4: With circuit breaker
            await TestWithCircuitBreaker();

            // Test 5: With both retry policy and circuit breaker
            await TestWithRetryAndCircuitBreaker();

            // Test 6: Singleton service verification
            await TestSingletonService();

            // Test 7: Exception handling tests
            await TestExceptionHandling();
        }//mthd

        /// <summary>
        /// Test 1: Basic AddMapNests with API key and origin.
        /// </summary>
        private static async Task TestBasicConfiguration()
        {
            _logger?.LogInformation("\n=== Test 1: Basic Configuration (API Key + Origin) ===");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(TestConfig.ApiKey, TestConfig.Origin);

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing Health Client via DI...");
                // Health client removed
                // bool isHealthy = await client.Health.IsHealthyAsync();
                // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("Health Client removed - DI test skipped");
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

        /// <summary>
        /// Test 2: AddMapNests with timeout configuration.
        /// </summary>
        private static async Task TestWithTimeout()
        {
            _logger?.LogInformation("\n=== Test 2: Configuration with Timeout ===");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(options =>
                {
                    options.ApiKey = TestConfig.ApiKey;
                    options.Origin = TestConfig.Origin;
                    options.Timeout = TimeSpan.FromSeconds(60);
                });//lambda

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing Health Client via DI with timeout (60s)...");
                // Health client removed
                // bool isHealthy = await client.Health.IsHealthyAsync();
                // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("Health Client removed - DI test skipped");
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

        /// <summary>
        /// Test 3: AddMapNests with retry policy configuration.
        /// </summary>
        private static async Task TestWithRetryPolicy()
        {
            _logger?.LogInformation("\n=== Test 3: Configuration with Retry Policy ===");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(options =>
                {
                    options.ApiKey = TestConfig.ApiKey;
                    options.Origin = TestConfig.Origin;
                    options.Timeout = TimeSpan.FromSeconds(60);
                    options.RetryPolicy = new RetryPolicyOptions
                    {
                        MaxRetries = 3,
                        InitialDelay = TimeSpan.FromSeconds(1),
                        BackoffMultiplier = 2.0,
                        MaxDelay = TimeSpan.FromSeconds(30)
                    };//mthd
                });//lambda

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing Health Client via DI with retry policy...");
                _logger?.LogInformation("  Retry Policy: MaxRetries=3, InitialDelay=1s, BackoffMultiplier=2.0, MaxDelay=30s");
                // Health client removed
                // bool isHealthy = await client.Health.IsHealthyAsync();
                // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("Health Client removed - DI test skipped");
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

        /// <summary>
        /// Test 4: AddMapNests with circuit breaker configuration.
        /// </summary>
        private static async Task TestWithCircuitBreaker()
        {
            _logger?.LogInformation("\n=== Test 4: Configuration with Circuit Breaker ===");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(options =>
                {
                    options.ApiKey = TestConfig.ApiKey;
                    options.Origin = TestConfig.Origin;
                    options.Timeout = TimeSpan.FromSeconds(60);
                    options.CircuitBreaker = new CircuitBreakerOptions
                    {
                        FailureThreshold = 5,
                        DurationOfBreak = TimeSpan.FromSeconds(30),
                        SamplingDuration = TimeSpan.FromMinutes(1),
                        HalfOpenMaxAttempts = 1
                    };//obj
                });//lambda

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing Health Client via DI with circuit breaker...");
                _logger?.LogInformation("  Circuit Breaker: FailureThreshold=5, DurationOfBreak=30s, SamplingDuration=1m, HalfOpenMaxAttempts=1");
                // Health client removed
                // bool isHealthy = await client.Health.IsHealthyAsync();
                // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("Health Client removed - DI test skipped");
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

        /// <summary>
        /// Test 5: AddMapNests with both retry policy and circuit breaker configuration.
        /// </summary>
        private static async Task TestWithRetryAndCircuitBreaker()
        {
            _logger?.LogInformation("\n=== Test 5: Configuration with Retry Policy AND Circuit Breaker ===");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(options =>
                {
                    options.ApiKey = TestConfig.ApiKey;
                    options.Origin = TestConfig.Origin;
                    options.Timeout = TimeSpan.FromSeconds(60);
                    options.RetryPolicy = new RetryPolicyOptions
                    {
                        MaxRetries = 3,
                        InitialDelay = TimeSpan.FromSeconds(1),
                        BackoffMultiplier = 2.0,
                        MaxDelay = TimeSpan.FromSeconds(30)
                    };//mthd
                    options.CircuitBreaker = new CircuitBreakerOptions
                    {
                        FailureThreshold = 5,
                        DurationOfBreak = TimeSpan.FromSeconds(30),
                        SamplingDuration = TimeSpan.FromMinutes(1),
                        HalfOpenMaxAttempts = 1
                    };//mthd
                });//lambda

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing Health Client via DI with retry policy AND circuit breaker...");
                _logger?.LogInformation("  Retry Policy: MaxRetries=3, InitialDelay=1s, BackoffMultiplier=2.0, MaxDelay=30s");
                _logger?.LogInformation("  Circuit Breaker: FailureThreshold=5, DurationOfBreak=30s, SamplingDuration=1m, HalfOpenMaxAttempts=1");
                // Health client removed
                // bool isHealthy = await client.Health.IsHealthyAsync();
                // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("Health Client removed - DI test skipped");
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

        /// <summary>
        /// Test 6: Verify singleton service behavior - multiple resolutions return the same instance.
        /// </summary>
        private static async Task TestSingletonService()
        {
            _logger?.LogInformation("\n=== Test 6: Singleton Service Verification ===");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(options =>
                {
                    options.ApiKey = TestConfig.ApiKey;
                    options.Origin = TestConfig.Origin;
                    options.Timeout = TimeSpan.FromSeconds(60);
                });//lambda

                var serviceProvider = services.BuildServiceProvider();

                // Resolve client multiple times
                var client1 = serviceProvider.GetRequiredService<IMapNestsClient>();
                var client2 = serviceProvider.GetRequiredService<IMapNestsClient>();
                var client3 = serviceProvider.GetRequiredService<IMapNestsClient>();

                // Verify they are the same instance (singleton)
                bool isSingleton = ReferenceEquals(client1, client2) && ReferenceEquals(client2, client3);
                _logger?.LogInformation("Resolved client 3 times from service provider");
                _logger?.LogInformation("Singleton verification: {IsSingleton}", isSingleton ? "✓ Same instance (Singleton)" : "✗ Different instances");

                if (isSingleton)
                {
                    _logger?.LogInformation("  ✓ IMapNestsClient is registered as singleton");
                    _logger?.LogInformation("  ✓ All resolutions return the same instance");
                    _logger?.LogInformation("  ✓ HttpClient is reused efficiently");
                }//if
                else
                {
                    _logger?.LogWarning("  ✗ Client instances are different - not singleton!");
                }//else

                // Test that all instances work correctly
                _logger?.LogInformation("\nTesting all resolved instances...");
                // Health client removed
                // bool health1 = await client1.Health.IsHealthyAsync();
                // bool health2 = await client2.Health.IsHealthyAsync();
                // bool health3 = await client3.Health.IsHealthyAsync();
                // _logger?.LogInformation("Client 1 Health Status: {Status}", health1 ? "Healthy" : "Unhealthy");
                // _logger?.LogInformation("Client 2 Health Status: {Status}", health2 ? "Healthy" : "Unhealthy");
                // _logger?.LogInformation("Client 3 Health Status: {Status}", health3 ? "Healthy" : "Unhealthy");
                _logger?.LogInformation("All client instances created successfully (Health client removed)");
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

        /// <summary>
        /// Test 7: Exception handling with dependency injection.
        /// Tests various exception types and how they propagate through DI.
        /// </summary>
        private static async Task TestExceptionHandling()
        {
            _logger?.LogInformation("\n=== Test 7: Exception Handling with DI ===");

            // Test 7.1: ApiException with different status codes
            await TestApiExceptionHandling();

            // Test 7.2: DeprecatedApiException
            await TestDeprecatedApiExceptionHandling();

            // Test 7.3: CircuitBreakerOpenException
            await TestCircuitBreakerExceptionHandling();

            // Test 7.4: AntiDebuggingException
            await TestAntiDebuggingExceptionHandling();

            // Test 7.5: Exception propagation through DI
            await TestExceptionPropagation();
        }//mthd

        /// <summary>
        /// Test 7.1: ApiException handling with different status codes.
        /// </summary>
        private static async Task TestApiExceptionHandling()
        {
            _logger?.LogInformation("\n--- Test 7.1: ApiException Handling ---");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests("invalid-api-key", "https://myapp.com");

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing with invalid API key (expecting 401 Unauthorized)...");
                try
                {
                    // Health client removed
                    // bool isHealthy = await client.Health.IsHealthyAsync();
                    // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                    _logger?.LogInformation("Health Client removed - test skipped");
                }//try
                catch (ApiException ex)
                {
                    _logger?.LogInformation("✓ ApiException caught successfully");
                    _logger?.LogInformation("  Exception Type: {Type}", ex.GetType().Name);
                    _logger?.LogInformation("  Message: {Message}", ex.Message);
                    if (ex.StatusCode.HasValue)
                    {
                        _logger?.LogInformation("  Status Code: {StatusCode}", ex.StatusCode.Value);
                        _logger?.LogInformation("  Status Code Category: {Category}", GetStatusCodeCategory(ex.StatusCode.Value));
                    }//if
                    if (!string.IsNullOrEmpty(ex.ResponseBody))
                    {
                        _logger?.LogInformation("  Response Body: {ResponseBody}", ex.ResponseBody);
                    }//if
                }//catch
            }//try
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd

        /// <summary>
        /// Test 7.2: DeprecatedApiException handling.
        /// </summary>
        private static async Task TestDeprecatedApiExceptionHandling()
        {
            _logger?.LogInformation("\n--- Test 7.2: DeprecatedApiException Handling ---");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(TestConfig.ApiKey, TestConfig.Origin);

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing deprecated API endpoint (if available)...");
                try
                {
                    // Health client removed
                    // This would throw DeprecatedApiException if the endpoint is deprecated
                    // bool isHealthy = await client.Health.IsHealthyAsync();
                    // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                    _logger?.LogInformation("Health Client removed - test skipped");
                }//try
                catch (DeprecatedApiException ex)
                {
                    _logger?.LogInformation("✓ DeprecatedApiException caught successfully");
                    _logger?.LogInformation("  Exception Type: {Type}", ex.GetType().Name);
                    _logger?.LogInformation("  Message: {Message}", ex.Message);
                    if (ex.StatusCode.HasValue)
                    {
                        _logger?.LogInformation("  Status Code: {StatusCode}", ex.StatusCode.Value);
                    }//if
                    if (ex.DeprecationDate.HasValue)
                    {
                        _logger?.LogInformation("  Deprecation Date: {Date}", ex.DeprecationDate.Value);
                    }//if
                    if (ex.SunsetDate.HasValue)
                    {
                        _logger?.LogInformation("  Sunset Date: {Date}", ex.SunsetDate.Value);
                    }//if
                    if (!string.IsNullOrEmpty(ex.MigrationPath))
                    {
                        _logger?.LogInformation("  Migration Path: {Path}", ex.MigrationPath);
                    }//if
                    if (!string.IsNullOrEmpty(ex.DeprecationWarning))
                    {
                        _logger?.LogInformation("  Deprecation Warning: {Warning}", ex.DeprecationWarning);
                    }//if
                }//catch
                catch (ApiException ex)
                {
                    _logger?.LogInformation("ApiException caught (not deprecated): {Message}", ex.Message);
                }//catch
            }//try
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd

        /// <summary>
        /// Test 7.3: CircuitBreakerOpenException handling.
        /// </summary>
        private static async Task TestCircuitBreakerExceptionHandling()
        {
            _logger?.LogInformation("\n--- Test 7.3: CircuitBreakerOpenException Handling ---");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(options =>
                {
                    options.ApiKey = TestConfig.ApiKey;
                    options.Origin = TestConfig.Origin;
                    options.CircuitBreaker = new CircuitBreakerOptions
                    {
                        FailureThreshold = 2, // Low threshold for testing
                        DurationOfBreak = TimeSpan.FromSeconds(10),
                        SamplingDuration = TimeSpan.FromSeconds(30),
                        HalfOpenMaxAttempts = 1
                    };//mthd
                });//lambda

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing circuit breaker behavior...");
                _logger?.LogInformation("  Note: CircuitBreakerOpenException is thrown when circuit is open");
                _logger?.LogInformation("  This requires multiple failures to trigger");

                try
                {
                    // Health client removed
                    // bool isHealthy = await client.Health.IsHealthyAsync();
                    // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                    _logger?.LogInformation("Health Client removed - test skipped");
                }//try
                catch (CircuitBreakerOpenException ex)
                {
                    _logger?.LogInformation("✓ CircuitBreakerOpenException caught successfully");
                    _logger?.LogInformation("  Exception Type: {Type}", ex.GetType().Name);
                    _logger?.LogInformation("  Message: {Message}", ex.Message);
                    _logger?.LogInformation("  Circuit breaker is open - requests are blocked");
                }//catch
                catch (ApiException ex)
                {
                    _logger?.LogInformation("ApiException caught: {Message}", ex.Message);
                }//catch
            }//try
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd

        /// <summary>
        /// Test 7.4: AntiDebuggingException handling.
        /// </summary>
        private static async Task TestAntiDebuggingExceptionHandling()
        {
            _logger?.LogInformation("\n--- Test 7.4: AntiDebuggingException Handling ---");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(TestConfig.ApiKey, TestConfig.Origin);

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing anti-debugging protection...");
                _logger?.LogInformation("  Note: AntiDebuggingException is thrown when debugger is detected");

                try
                {
                    // Health client removed
                    // bool isHealthy = await client.Health.IsHealthyAsync();
                    // _logger?.LogInformation("Health Status: {Status}", isHealthy ? "Healthy" : "Unhealthy");
                    _logger?.LogInformation("Health Client removed - test skipped");
                }//try
                catch (AntiDebuggingException ex)
                {
                    _logger?.LogInformation("✓ AntiDebuggingException caught successfully");
                    _logger?.LogInformation("  Exception Type: {Type}", ex.GetType().Name);
                    _logger?.LogInformation("  Message: {Message}", ex.Message);
                    _logger?.LogInformation("  Anti-debugging protection triggered");
                }//catch
                catch (ApiException ex)
                {
                    _logger?.LogInformation("ApiException caught: {Message}", ex.Message);
                }//catch
            }//try
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd

        /// <summary>
        /// Test 7.5: Exception propagation through dependency injection.
        /// </summary>
        private static async Task TestExceptionPropagation()
        {
            _logger?.LogInformation("\n--- Test 7.5: Exception Propagation Through DI ---");
            try
            {
                var services = new ServiceCollection();
                services.AddMapNests(TestConfig.ApiKey, TestConfig.Origin);

                var serviceProvider = services.BuildServiceProvider();
                var client = serviceProvider.GetRequiredService<IMapNestsClient>();

                _logger?.LogInformation("Testing exception propagation from DI-resolved client...");

                // Test that exceptions propagate correctly
                try
                {
                    // Try to get a non-existent resource (would throw 404)
                    var demoRequest = new CreateDemoRequest
                    {
                        Name = "Test",
                        Description = "Test",
                        Data = "Test"
                    };//mthd

                    // Demo client removed
                    // var result = await client.Demo.CreateAsync(demoRequest);
                    // _logger?.LogInformation("Demo created: {Id}", result.Id);
                    _logger?.LogInformation("Demo client removed - test skipped");
                }//try
                catch (ApiException ex)
                {
                    _logger?.LogInformation("✓ Exception propagated correctly through DI");
                    _logger?.LogInformation("  Exception Type: {Type}", ex.GetType().Name);
                    _logger?.LogInformation("  Message: {Message}", ex.Message);
                    if (ex.StatusCode.HasValue)
                    {
                        _logger?.LogInformation("  Status Code: {StatusCode}", ex.StatusCode.Value);
                    }//if
                    _logger?.LogInformation("  ✓ Exception handling works correctly with DI");
                }//catch

                // Test exception handling in a service class pattern
                _logger?.LogInformation("\nTesting exception handling in service pattern...");
                var testService = new TestService(client, _logger);
                await testService.TestServiceMethod();
            }//try
            catch (Exception ex)
            {
                _logger?.LogError("Unexpected Error: {Message}", ex.Message);
            }//catch
        }//mthd

        /// <summary>
        /// Gets the category of HTTP status code.
        /// </summary>
        private static string GetStatusCodeCategory(int statusCode)
        {
            if (statusCode >= 400 && statusCode < 500)
            {
                return "Client Error (4xx)";
            }//if
            if (statusCode >= 500)
            {
                return "Server Error (5xx)";
            }//if
            return "Other";
        }//mthd

        /// <summary>
        /// Test service class to demonstrate exception handling in DI pattern.
        /// </summary>
        private class TestService
        {
            private readonly IMapNestsClient _client;
            private readonly ILogger? _serviceLogger;

            public TestService(IMapNestsClient client, ILogger? logger = null)
            {
                _client = client;
                _serviceLogger = logger;
            }//ctor

            public async Task TestServiceMethod()
            {
                try
                {
                    // Health client removed
                    // bool isHealthy = await _client.Health.IsHealthyAsync();
                    // _serviceLogger?.LogInformation("  Service method executed successfully");
                    _serviceLogger?.LogInformation("  Health Client removed - service method skipped");
                }//try
                catch (ApiException ex)
                {
                    _serviceLogger?.LogInformation("  ✓ Service method caught ApiException: {Message}", ex.Message);
                    _serviceLogger?.LogInformation("  ✓ Exception handling works in service classes with DI");
                }//catch
            }//mthd
        }//cls
    }//cls
}//ns

