using Serilog;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace WebApp
{
    public static class Setup
    {
        /// <summary>
        /// Sets the logger up and running
        /// </summary>
        public static void Serilog(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            const string OUTPUT_TEMPLATE = "{Timestamp:MM/dd/yyyy hh:mm:ss tt} [{Level}] {Message}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: OUTPUT_TEMPLATE
                )
            .WriteTo.Async(x =>
                x.File(@"D:\Logs\log.txt",
                outputTemplate: OUTPUT_TEMPLATE,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 10,
                fileSizeLimitBytes: 20 * 1000000,
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
