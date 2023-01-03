using BilingualMangaReaderScraper.Services;

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
            .Build();

        host.Run();
    }
}
