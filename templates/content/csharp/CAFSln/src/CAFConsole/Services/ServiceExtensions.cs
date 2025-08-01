using CAFConsole.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Templates;

namespace CAFConsole.Services;

public static class ServiceExtensions
{
    public static IConfiguration CreateConfiguration() =>
        new ConfigurationBuilder().AddJsonFile("./config.json", false).Build();

    public static void ConfigureSerilog(this ILoggingBuilder builder)
    {
        builder.AddSerilog(
            new LoggerConfiguration()
                .WriteTo.File(
                    formatter: new ExpressionTemplate("[{@t:HH:mm:ss} {@l:u3}] {@m}\n{@x}"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app-.log"),
                    shared: true,
                    rollingInterval: RollingInterval.Day
                )
                .Enrich.WithProperty("Application Name", "<APP NAME>")
                .WriteTo.Console(theme: AnsiConsoleTheme.Sixteen)
                .CreateLogger()
        );
    }

    public static IServiceCollection RegisterAppServices(this IServiceCollection services)
    {
        var configuration = CreateConfiguration();

        services.AddLogging(ConfigureSerilog);

        services.AddSingleton(configuration);
        services.AddSingleton<IService, ServiceImplementation>();
        services.AddSingleton<MyCommands>();

        return services;
    }
}
