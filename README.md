# Express.Logging

Express Logging is not a library. It's an example of clear implementation of logging on .NET Core applications by making use of SeriLog and it's async writing feature to file sink.

![alt text](https://lh3.googleusercontent.com/proxy/B_7eIUlcSWIhijMsKkvsKeB4sv5ZqG8cOGXyWFKIrIAgPlQTL_RyHreEs5bCSthMBUgPzIuifuFL89mIzpZfMhA)

### Repository Contents
This repo maintains 1 project which implements Serilog and its configurations for clear and consise logging implementation.

### PreRequesties
Your project need to install the following Serilog nuget modules to configure logging. Install the following packages from nuget

This module deals with core logging functionalities
```nuget
Serilog.AspNetCore
```
This module allows Serilog to read from appsettings.json (For .NET Core projects)
```nuget
Serilog.Settings.Configuration
```
This is a wrapper sink that floats over other sinks and facilitate asynchronous logging. (Which contributes significant perfomance advantage)
```nuget
Serilog.Sinks.Async
```
File sink allows serilog to write to files
```nuget
Serilog.Sinks.File
```

### Configuration
Different logging levels set in the appsettings.json will output logs in that or higher levels
| Level | Name
| ------ | ------
| 0 | Verbose
| 1 | Debug
| 2 | Information
| 3 | Warning
| 4 | Error
| 4 | Fatal
### appsettings.json
Serilog works with appsettings.json configuration tree. It can be configured like this
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Verbose",
      "Override": {
        "Microsoft": "Warning",
        "System": "Error"
      }
    },
    "WriteTo": [
      {
        "Name": "Async",
        "Args": {
          "configure": [
            {
              "Name": "File",
              "Args": {
                "path": "C:\\Sangeeth\\AppLogs\\log.txt",
                "rollingInterval": "Day",
                "retainedFileCountLimit": 7,
                "buffered": false
              }
            }
          ]
        }
      }
    ]
  },
  "AllowedHosts": "*"
}
```
### Configuration Information
| Key | Significance
| ------ | ------
| MinimumLevel > Default | During development this can be set to either "Verbose" or "Debug"
| Override | Override is to limit logs emitted from other namespaces. "Warning" and "Error" correspondint to Microsoft and System is optimal in development and production scenerios
| Args > Path | Location of log file
| Args > RollingInterval | At what interval logs need to be rolled/archived. Now it's everyday
| Args > RetainedFileCountLimit | Days after which logs can be cleared. Here it's 7 days after created, logs will be deleted
| Args > Buffered | Enabling this will buffer logs and write whenever Log.CloseAndFlush(); is called. It has perfomance advantage but requires the programmer to call it once ready to log whatever buffered. This depends on programmers preference

### Adding Serilog To Program.cs
Add this line to Program.cs.
This step allows Serilog to be used anywhere on our project
```csharp
.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
```
So the Program.cs looks like this
```csharp
public class Program {
  public static void Main(string[] args) {
    CreateHostBuilder(args).Build().Run();
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>{
    webBuilder.UseStartup < Startup > ();
  }).UseSerilog((hostingContext, loggerConfiguration) =>loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
}
```
### Write logs
Now you can write logs anywhere in the application
```csharp
Log.Verbose("Verbose");
Log.Debug("Debug");
Log.Information("Information");
Log.Warning("Warning");
Log.Error("Error");
Log.Fatal("Fatal");
```
If buffered is "true" on appsettings.json. Donot forget to add this line whenever logs need to be written.
```csharp
Log.CloseAndFlush();
```
### Logs
This configuration gives clear and proper log output. The output generated file will look like this with the above configured "appsettings.json" configuration
```text
2020-07-31 15:38:26.562 +05:30 [VRB] Verbose
2020-07-31 15:38:26.570 +05:30 [DBG] Debug
2020-07-31 15:38:26.570 +05:30 [INF] Information
2020-07-31 15:38:26.570 +05:30 [WRN] Warning
2020-07-31 15:38:26.570 +05:30 [ERR] Error
2020-07-31 15:38:26.570 +05:30 [FTL] Fatal
```
