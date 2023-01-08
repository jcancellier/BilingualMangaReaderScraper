using BilingualMangaScanScraper.Services;

namespace BilingualMangaScanScraper;

public class Worker : BackgroundService
{
    
    private readonly ILogger<Worker> _logger;
    private readonly IMangaScraperService _mangaScraperService;

    public Worker(ILogger<Worker> logger, IMangaScraperService mangaScraperService)
    {
        _logger = logger;
        _mangaScraperService = mangaScraperService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            _mangaScraperService.Scrape();

            await Task.Delay(3600000, stoppingToken);
        }
    }
}

