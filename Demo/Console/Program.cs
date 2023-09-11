using ConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using static System.Net.Mime.MediaTypeNames;

//Setup Logger
Setup.Serilog();


//Dependency Injection Setup
static void ConfigureServices(ServiceCollection services)
{
    services.AddSingleton<SeperateClass>();

    var serilogLogger = new LoggerConfiguration()
    .WriteTo.File("TheCodeBuzz.txt")
    .CreateLogger();

    services.AddLogging(builder =>
    {
        builder.SetMinimumLevel(LogLevel.Information);
        builder.AddSerilog(logger: serilogLogger, dispose: true);
    });
}




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