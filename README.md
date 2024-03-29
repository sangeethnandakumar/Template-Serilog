# Structured Logging - Simplified
This is a Serilog installation template that can work in tandem with Microsoft's `ILogger` interface and provides structured logging support

![alt text](https://code.4noobz.net/wp-content/uploads/2021/10/serilog-logo.png)

# IMPLEMENTING IN - WebAPIs
## Install Packages
```xml
    <!-- LOGGING PROVIDERS-->
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="Serilog.Extensions.Hosting" Version="8.0.0" />
    <PackageReference Include="Serilog.Settings.Configuration" Version="8.0.0" />

    <!-- LOGGING SINKS-->
    <PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
```

## Create An Extension Method `UseSerilogConfiguration()`
```csharp
using Serilog;
namespace Instaread.BestSellingScrapper.API.Extensions
{
    public static class SerilogExtensions
    {
        public static IHostBuilder UseSerilogConfiguration(this IHostBuilder builder)
        {
            builder.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration);
            });
            return builder;
        }
    }
}
```

## Call Extension Method From `Program.cs`
```csharp
using Instaread.BestSellingScrapper.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilogConfiguration();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

## Lastly Put This In `appsettings.json`
```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft.AspNetCore.Hosting": "Warning",
        "Microsoft.AspNetCore.Mvc": "Warning",
        "Microsoft.AspNetCore.Routing.EndpointMiddleware": "Warning",
        "Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "{Timestamp:dd/MM/yy hh:mm:ss tt} [{Level:u3}] {Message}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 5,
          "fileSizeLimitBytes": 20000000,
          "rollOnFileSizeLimit": true,
          "outputTemplate": "{Timestamp:dd/MM/yy hh:mm:ss tt} [{Level:u3}] {Message}{NewLine}{Exception}"
        }
      }
    ],
    "Properties": {
      "Application": "Instaread.BestSellingScrapper.API"
    }
  }
}
```

## Now Just Inject And Use `ILogger`
```csharp
[HttpGet(Name = "GetWeatherForecast")]
public IEnumerable<WeatherForecast> Get()
{
    //Simple logging
    _logger.LogDebug("This is a debug log");

    //Structured Logging
    _logger.LogInfo("It is very expensive at {Cost} rupees per piece", 120 );

    //Structured Logging With JSON serializing
    _logger.LogWarning("This data will get serialized on logs {@Data}", new { Name="Sangeeth" });

    return data;
}
```

# IMPLEMENTING IN - CONSOLE App
Your project need to install the following Serilog nuget modules to configure logging. Install the following packages from nuget

## Step 1: Install Required NuGet Libraries
Install the following NuGet packages
```xml
  <ItemGroup>
    <PackageReference Include="Ben.Demystifier" Version="0.4.1" />
    <PackageReference Include="Serilog" Version="2.11.0" />
    <PackageReference Include="Serilog.Sinks.Async" Version="1.5.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="4.0.1" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
  </ItemGroup>
```

## Step 2: Copy class 'Setup.cs'
Copy the Setup.cs file to your app
```csharp
using Serilog;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ConsoleApp
{
    public static class Setup
    {
        /// <summary>
        /// Sets the logger up and running
        /// </summary>
        public static void Serilog()
        {
            const string OUTPUT_TEMPLATE = "{Timestamp:MM/dd/yyyy hh:mm:ss tt} [{Level}] {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: OUTPUT_TEMPLATE
                )
            .WriteTo.Async(x =>
                x.File(@"D:\Logs\log.txt",
                outputTemplate: OUTPUT_TEMPLATE,
                rollingInterval: RollingInterval.Day, //Creates new file daily
                retainedFileCountLimit: 5, //Maintains 5 log files at a time. Auto delete older logs
                fileSizeLimitBytes: 20 * 1000000, //If log grows > 20MB, Splits into new log file
                rollOnFileSizeLimit: true)
            )
            .CreateLogger();
        }

        /// <summary>
        /// Enrich exceptions with hints for better troubleshooting
        /// </summary>
        /// <param name="message"></param>
        /// <param name="hints"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string AddHints(this string message, object hints, Exception ex = null)
        {
            var sw = new Stopwatch();
            sw.Start();
            var renderHints = JsonSerializer.Serialize(hints, new JsonSerializerOptions { WriteIndented = true });
            var builder = new StringBuilder(message);
            builder.Append("\nHints:");
            builder.Append(renderHints.Substring(1, renderHints.Length - 2));
            if (ex != null)
            {
                builder.Append("\nException:\n");
                builder.Append(ex.Demystify().StackTrace);
            }
            builder.Append("\n------------------------------------------------------------------\n");
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
            return builder.ToString();
        }
    }
}
```

## Step 3: Setup Serilog
Call Configure Serilog before anything
```csharp
    //Setup Logger
    Configure.Serilog();
```

## Step 4: Implement Logger
Implement the logger as follows
```csharp
	using Serilog;
	
	//Setup Logger
	Configure.Serilog();
	
	//Write some usefull logs
	Log.Information("Info error");
	Log.Warning("Warning error");
	
	//Write some exception logs
	try
	{
		Log.Information("Info error");
		throw new StackOverflowException();
	}
	catch (Exception ex)
	{
		//Catch exceptions and write hints
		Log.Error("An exception occured.".AddHints(
			new
			{
				A = "Value of A",
				B = "Value of B",
				C = new { Fname = "Sangee", LName = "Nandakumar" }
			}
		, ex));
	}
	Console.Read();
```

# IMPLEMENTING IN - ASP.NET WebApp
Your project need to install the following Serilog nuget modules to configure logging. Install the following packages from nuget

## Step 1: Install Required NuGet Libraries
Install the following NuGet packages
```xml
<ItemGroup>
    <PackageReference Include="Ben.Demystifier" Version="0.4.1" />
    <PackageReference Include="Serilog" Version="3.0.1" />
    <PackageReference Include="Serilog.Sinks.Async" Version="1.5.0" />
    <PackageReference Include="Serilog.Sinks.Console" Version="4.1.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
</ItemGroup>
```

## Step 2: Copy class 'Configure.cs'
Copy the Configure.cs file to your app

### If using Seq, Add this sink also
```csharp
 .WriteTo.Async(x =>
                x.Seq("https://seq.twileloop.com", apiKey: "*****")
            )
```

```csharp
using ConsoleApp;
using Serilog;

//Setup Logger
Setup.Serilog();

//Write some usefull logs
Log.Information("Info error");
Log.Warning("Warning error");

//Write some exception logs
try
{
    Log.Information("Info error");
    throw new StackOverflowException();
}
catch (Exception ex)
{
    //Catch exceptions and write hints
    Log.Error("An exception occured.".AddHints(
        new
        {
            A = "Value of A",
            B = "Value of B",
            C = new { Fname = "Sangee", LName = "Nandakumar" }
        }
        , ex));
}

Console.Read();
```

## Step 3: Setup Serilog
Wrap everything ina try-catch and add
```csharp
Configure.Serilog(builder);
```
after var builder = WebApplication.CreateBuilder(args);`

### General configuration

```csharp
using Serilog;
using WebApp;

try
{
    var builder = WebApplication.CreateBuilder(args);
    Configure.Serilog(builder);
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
```

## Step 4: Write logs

## Write structured logs
```
Log.Information("Processed {@SensorInput} in {TimeMS:000} ms", new { Latitude = 25, Longitude = 134 }, 34);
```

## Write logs anywhere
```csharp
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            Log.Debug("Some sample logs");
            Log.Information("Some sample logs");
            Log.Warning("Some sample logs");
            try
            {
                throw new Exception("This is a test exception");
            }
            catch (Exception ex)
            {
                //Catch exceptions and write hints
                Log.Error("An exception occured.".AddHints(
                    new
                    {
                        A = "Value of A",
                        B = "Value of B",
                        C = new { Fname = "Sangee", LName = "Nandakumar" }
                    }
                    , ex));
            }
            finally
            {
            }
            return Ok(1);
        }
    }
```
