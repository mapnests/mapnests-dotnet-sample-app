//In the name of Allah 

using Microsoft.Extensions.Logging;

namespace MapNests.DotNet.SampleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Setup logging
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug);
            });
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("MapNests SDK Test Application");
            logger.LogInformation("============================\n");

            // Initialize all test classes with logger
            TestRouteMap.SetLogger(logger);
            TestGeoMap.SetLogger(logger);
            TestEta.SetLogger(logger);
            TestBuilderPattern.SetLogger(logger);
            TestManualInstantiation.SetLogger(logger);
            TestDependencyInjection.SetLogger(logger);
            TestRetry.SetLogger(logger);
            TestCircuitBreaker.SetLogger(logger);

            // Run all API tests
            logger.LogInformation("\n=== Running All API Tests ===");

            logger.LogInformation("\n=== Test: Retry Policy ===");
            await TestRetry.RunTest();

            logger.LogInformation("\n=== Test: Circuit Breaker ===");
            await TestCircuitBreaker.RunTest();

            logger.LogInformation("\n=== Test: Dependency Injection ===");
            await TestDependencyInjection.RunTest();

            logger.LogInformation("\n=== Test: RouteMap API ===");
            await TestRouteMap.RunTest();

            logger.LogInformation("\n=== Test: GeoMap API ===");
            await TestGeoMap.RunTest();

            logger.LogInformation("\n=== Test: Eta API ===");
            await TestEta.RunTest();

            logger.LogInformation("\nTest completed!");
        }//mthd
    }//cls
}//ns
