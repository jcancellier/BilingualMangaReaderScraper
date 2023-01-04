using BilingualMangaReaderScraper.Services;
using Serilog;
namespace BilingualMangaReaderScraper;

public class Program
{
    public static void Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();

                services.AddTransient<IMangaScraperService, MangaScraperService>();
            })
            .UseSerilog((hostingContext, loggerConfiguration) => 
            {
                loggerConfiguration
                    .ReadFrom.Configuration(hostingContext.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
            })
            .Build();

        host.Run();
    }
}
