using Microsoft.Extensions.Logging;
using Serilog;

namespace TheCodeKitchen.Infrastructure.Logging.Serilog;

public static class SerilogLoggingRegistration
{
    public static void RegisterSerilog(this ILoggingBuilder loggingBuilder)
    {
        var logger = new LoggerConfiguration()
            .Destructure.With<MaskPasswordPolicy>()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
        
        loggingBuilder.AddSerilog(logger,  dispose: true);
    }
}