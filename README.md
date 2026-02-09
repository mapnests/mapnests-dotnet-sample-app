# MapNests SDK

A .NET SDK for MapNests map data and functionality with modern configuration patterns, validation, retry logic, circuit breaker, and dependency injection support.

## Installation

Install the package via NuGet:

```bash
dotnet add package MapNests
```

Or via Package Manager Console:

```powershell
Install-Package MapNests
```

## Getting Started

### API Registration & Authentication

**Step 1: Register for API Access**

- Endpoint: https://mapnests.com/sign-up
- Create an account and register for API access.

**Step 2: Login to Dashboard**

- Endpoint: https://mapnests.com/sign-in
- Use your credentials to log in and manage your projects.

### Project Setup

**Step 1: Create a Project**

1. Log in to the MapNests Dashboard.
2. Navigate to the dashboard.
3. Go to the Projects section.
4. Click on "Create Project".
5. Provide a Project Name and Project Description.
6. After project creation, go to the details section.
7. In the Android Section, add your Package Name and SHA256 fingerprint.
8. Once successfully created, you will receive an API Key.

### Demo Values for Testing

For local development and testing, you can use these demo values:

| Parameter | Demo Value | Description |
|-----------|------------|-------------|
| **ApiKey** | `your-api-key-here` | Placeholder for development; replace with your API key for production |
| **Origin** | `https://myapp.com` | Web app origin (used in Origin header) |
| **Origin** | `com.technonext.sdk` | Android package name (when calling from mobile SDK) |

```csharp
// Quick test with demo values
using var client = new MapNestsClientBuilder()
    .WithApiKey("your-api-key-here")
    .WithOrigin("https://myapp.com")
    .Build();
```

> **Note**: Replace with your actual API key and origin for production. Obtain credentials from your MapNests dashboard.

### Basic Usage

```csharp
using MapNests.Public.Clients;
using MapNests.Public.Requests;

// Using builder pattern (recommended)
using var client = new MapNestsClientBuilder()
    .WithApiKey("your-api-key")
    .WithOrigin("https://myapp.com")
    .Build();

// Health check
if (await client.Health.IsHealthyAsync())
{
    Console.WriteLine("API is healthy!");
}

// Demo operations
var demo = await client.Demo.CreateAsync(new CreateDemoRequest 
{ 
    Name = "Test", 
    Description = "Test description" 
});

var result = await client.Demo.GetAsync(demo.Id);
await client.Demo.UpdateAsync(demo.Id, new UpdateDemoRequest { Name = "Updated" });
await client.Demo.DeleteAsync(demo.Id);

// Route map operations (V1, V2, V3)
var routeResult = await client.RouteMap.V1.SearchAsync(new SearchRequest { /* ... */ });

// Geo map operations (V1, V2)
var geoResult = await client.GeoMap.V1.GeocodeAsync(new GeocodeRequest { /* ... */ });
```

### Configuration Options

#### Builder Pattern

```csharp
var client = new MapNestsClientBuilder()
    .WithApiKey("your-api-key")
    .WithOrigin("https://myapp.com")
    .WithTimeout(TimeSpan.FromSeconds(60))
    .WithRetryPolicy(policy =>
    {
        policy.MaxRetries = 3;
        policy.InitialDelay = TimeSpan.FromSeconds(1);
        policy.BackoffMultiplier = 2.0;
        policy.MaxDelay = TimeSpan.FromSeconds(30);
    })
    .WithCircuitBreaker(circuitBreaker =>
    {
        circuitBreaker.FailureThreshold = 5;
        circuitBreaker.DurationOfBreak = TimeSpan.FromSeconds(30);
        circuitBreaker.SamplingDuration = TimeSpan.FromMinutes(1);
    })
    .Build();
```

#### Environment Variables

```csharp
// Set environment variables:
// MAPNESTS_API_KEY=your-api-key
// MAPNESTS_ORIGIN=https://myapp.com

var client = new MapNestsClientBuilder()
    .WithApiKey(Environment.GetEnvironmentVariable("MAPNESTS_API_KEY") ?? "")
    .WithOrigin(Environment.GetEnvironmentVariable("MAPNESTS_ORIGIN") ?? "")
    .Build();
```

#### Dependency Injection

```csharp
using MapNests.Public;
using Microsoft.Extensions.DependencyInjection;

// Option 1: Direct configuration
services.AddMapNests(options =>
{
    options.ApiKey = "your-api-key";
    options.Origin = "https://myapp.com";
    options.Timeout = TimeSpan.FromSeconds(60);
});

// Option 2: From configuration (appsettings.json)
services.AddMapNests(configuration);

// Option 3: Using IHttpClientFactory (Recommended for Production)
services.AddMapNestsHttpClient(options =>
{
    options.ApiKey = "your-api-key";
    options.Origin = "https://myapp.com";
    options.RetryPolicy = new RetryPolicyOptions { MaxRetries = 3 };
    options.CircuitBreaker = new CircuitBreakerOptions { FailureThreshold = 5 };
});

// Option 4: IHttpClientFactory with IConfiguration
services.AddMapNestsHttpClient(configuration);

// Option 5: Using IOptions pattern
services.Configure<ApiClientOptions>(configuration.GetSection("MapNests"));
services.AddMapNestsWithOptions();
```

#### Configuration Binding (appsettings.json)

```json
{
  "MapNests": {
    "ApiKey": "your-api-key",
    "Origin": "https://myapp.com",
    "Timeout": "00:01:00",
    "RetryPolicy": {
      "MaxRetries": 3,
      "InitialDelay": "00:00:01",
      "BackoffMultiplier": 2.0,
      "MaxDelay": "00:00:30"
    },
    "CircuitBreaker": {
      "FailureThreshold": 5,
      "DurationOfBreak": "00:00:30",
      "SamplingDuration": "00:01:00",
      "HalfOpenMaxAttempts": 1
    }
  }
}
```

```csharp
// In Startup.cs or Program.cs
services.AddMapNests(configuration);
```

### API Versioning

The SDK automatically uses the API version that matches the SDK version (e.g., SDK 1.0.0 → API v1). You can also access specific API versions:

```csharp
// Demo: default or explicit V1/V2
var result = await client.Demo.GetAsync("id");
var resultV1 = await client.Demo.V1.GetAsync("id");
var resultV2 = await client.Demo.V2.GetAsync("id");

// RouteMap: V1, V2, or V3
var routeV1 = await client.RouteMap.V1.SearchAsync(request);
var routeV3 = await client.RouteMap.V3.MultiSourceSummaryAsync(request);

// GeoMap: V1 or V2
var geoV1 = await client.GeoMap.V1.GeocodeAsync(request);
var geoV2 = await client.GeoMap.V2.ReverseAsync(request);
```

### Validation

The SDK uses data annotations for automatic validation:

- `ApiKey` and `Origin` are required (`[Required]`)
- `Timeout` must be between 1 second and 5 minutes (`[Range]`)
- `RetryPolicyOptions` properties have range validations

Validation occurs automatically when using DI extensions or when calling `Build()` on the builder.

### Exceptions

The SDK uses a clear exception hierarchy for error handling:

| Exception | When Thrown |
|-----------|-------------|
| **ApiException** | General API errors (4xx, 5xx status codes) |
| **DeprecatedApiException** | Using deprecated APIs (RFC 8594 headers) |
| **CircuitBreakerOpenException** | Circuit breaker is open (too many failures) |
| **AntiDebuggingException** | SDK internal protection triggered |

```csharp
using MapNests.Public.Exceptions;

try
{
    var result = await client.RouteMap.V1.MultiSourceSummaryAsync(request);
}
catch (ApiException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
    Console.WriteLine($"Status Code: {ex.StatusCode}");
    Console.WriteLine($"Response: {ex.ResponseBody}");
}
catch (CircuitBreakerOpenException ex)
{
    Console.WriteLine($"Circuit breaker open: {ex.Message}");
}
catch (DeprecatedApiException ex)
{
    Console.WriteLine($"Deprecated API: {ex.Message}");
    Console.WriteLine($"Sunset: {ex.SunsetDate}");
}
```

All API exceptions expose `StatusCode` and `ResponseBody` for debugging. See [Error Handling](./docs/ERROR_HANDLING.md) for details.

## Requirements

This package targets both:
- **.NET Standard 2.1** - For maximum compatibility
- **.NET 8.0** - For modern .NET applications

### Compatibility

The .NET Standard 2.1 target is compatible with:
- .NET Core 3.0+
- .NET Framework 4.8+
- .NET 5+
- .NET 6+
- .NET 7+
- .NET 8+

The .NET 8.0 target provides optimized performance for .NET 8+ applications.

### Dependencies

- `System.ComponentModel.Annotations` (v5.0.0) - For validation attributes
- `System.Text.Json` (v10.0.2) - For JSON serialization
- `Microsoft.Extensions.DependencyInjection.Abstractions` (v10.0.2) - For DI support (optional)
- `Microsoft.Extensions.Options` (v10.0.2) - For configuration binding (optional)
- `Microsoft.Extensions.Configuration.Binder` (v10.0.2) - For configuration binding (optional)
- `Microsoft.Extensions.Logging.Abstractions` (v10.0.2) - For logging integration (optional)
- `Microsoft.Extensions.Http` (v10.0.2) - For `AddMapNestsHttpClient` / IHttpClientFactory (optional)

## Features

- ✅ **Builder Pattern** - Fluent API for configuration (`MapNestsClientBuilder`)
- ✅ **Options Pattern** - Strongly-typed configuration
- ✅ **Environment Variables** - Configuration via builder with env vars
- ✅ **Configuration Binding** - Bind from `appsettings.json`
- ✅ **Validation Attributes** - Automatic validation with data annotations
- ✅ **Retry Logic** - Configurable exponential backoff
- ✅ **Circuit Breaker** - Prevents cascading failures with automatic recovery
- ✅ **Dependency Injection** - Full DI support with validation
- ✅ **IHttpClientFactory** - Connection pooling via `AddMapNestsHttpClient`
- ✅ **Structured Logging** - Integration with `Microsoft.Extensions.Logging`
- ✅ **Type Safety** - Strongly typed APIs throughout
- ✅ **API Versioning** - Demo (V1/V2), RouteMap (V1/V2/V3), GeoMap (V1/V2)


For more information, visit the [project repository](https://github.com/yourusername/mapnests).

## License

This project is licensed under the MIT License.

## Versioning

This project follows [Semantic Versioning](https://semver.org/) (SemVer).
