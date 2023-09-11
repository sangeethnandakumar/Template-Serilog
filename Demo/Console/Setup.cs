using Microsoft.Extensions.Logging;
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

        public static ILogger ConfigureLogging()
        {
            const string OUTPUT_TEMPLATE = "{Timestamp:MM/dd/yyyy hh:mm:ss tt} [{Level}] {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
                .WriteTo.File(
                    @"D:\Logs\log.txt",
                    outputTemplate: OUTPUT_TEMPLATE,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 5,
                    fileSizeLimitBytes: 20 * 1000000,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            // Create an instance of Serilog logger
            var logger = new LoggerConfiguration()
                .ReadFrom.Services(provider)
                .CreateLogger();

            return logger;
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
