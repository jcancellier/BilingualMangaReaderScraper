using BilingualMangaReaderScraper.Models;
using BilingualMangaReaderScraper.Utilities;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace BilingualMangaReaderScraper.Services
{
	public class MangaScraperService : IMangaScraperService
	{

        private string MANGA_ID_REGEX_PATTERN = @"enid:\s*""([^""]+)""";
        private string BASE_URL = "https://bilingualmanga.net";

        ILogger<MangaScraperService> _logger;

		public MangaScraperService(ILogger<MangaScraperService> logger)
		{
            _logger = logger;
        }

        public void Scrape()
        {
            // Load web page
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load($"{BASE_URL}");

            var html = doc.DocumentNode.InnerHtml;

            // Get manga ids
            var mangaIds = ExtractMangaIds(html, MANGA_ID_REGEX_PATTERN);

            _logger.LogInformation($"Found {mangaIds.Count()} manga ids.");
            foreach (var mangaId in mangaIds)
            {
                _logger.LogInformation(mangaId);
            }

            // TODO
            // Make service more robust by only retrieving new data
            // Criteria
            // 1. New MangaIds
            // 2. New content for mangas (volumes/chapters)

            // load manga web page with selenium since it is a dynamic web page
            var chromeOptions = new ChromeOptions();
            
            // TODO control this with appsettings
            chromeOptions.AddArgument("--headless");

            var scrapedMangaEntries = new List<ScrapedMangaEntry>();
            using (var driver = new RemoteWebDriver(new Uri("http://host.docker.internal:4444"), chromeOptions))
            {
                driver.Manage().Window.Maximize();

                // Get manga data
                foreach (var mangaId in mangaIds)
                {
                    try 
                    {
                        var scrapedMangaEntryEnglish = 
                            ScrapeMangaData(mangaId, web, doc, driver, Languages.English);
                        var scrapedMangaEntryJapanese = 
                            ScrapeMangaData(mangaId, web, doc, driver, Languages.Japanese);

                        scrapedMangaEntries.Add(scrapedMangaEntryEnglish);
                        scrapedMangaEntries.Add(scrapedMangaEntryJapanese);

                        _logger.LogInformation(scrapedMangaEntryEnglish.ToStringEx());
                        _logger.LogInformation(scrapedMangaEntryJapanese.ToStringEx());
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, $"Exception occured while scraping for manga id {mangaId}");
                        // TODO send notification/email when errors occur
                        // TODO implement remote logging with something like aws
                    }
                }
            }
        }

        private ScrapedMangaEntry ScrapeMangaData(
            string mangaId, HtmlWeb web, HtmlDocument doc, IWebDriver driver, Languages language)
        {
            var languageString = language switch
            {
                Languages.English => "en",
                Languages.Japanese => "jp",
                _ => "en"
            };

            driver.Navigate().GoToUrl($"{BASE_URL}/manga/{mangaId}?lang={languageString}");

            // Wait for the page to load
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var element = wait.Until(d => d.FindElement(By.Id(MangaHtmlElements.CHAPTER_LIST_CONTAINER_ID)));

            string html = driver.PageSource;

            // parse html for manga metadata
            doc.LoadHtml(html);

            var mangaName = doc.GetElementbyId(MangaHtmlElements.TITLE_ID).InnerHtml;
            var description = doc.DocumentNode.QuerySelector(MangaHtmlElements.DESCRIPTION_SELECTOR).InnerText;
            var author = doc.DocumentNode.QuerySelector(MangaHtmlElements.AUTHOR_SELECTOR).InnerText;
            var artist = doc.DocumentNode.QuerySelector(MangaHtmlElements.ARTIST_SELECTOR).InnerText;
            var releaseYear = doc.DocumentNode.QuerySelector(MangaHtmlElements.RELEASE_YEAR_SELECTOR).InnerText;
            var completionStatus = doc.DocumentNode.QuerySelector(MangaHtmlElements.COMPLETED_SELECTOR).InnerText;
            
            var genresContainer = doc.DocumentNode.QuerySelector(MangaHtmlElements.GENRES_LIST_CONTAINER_SELECTOR);
            var genreNodes = genresContainer.SelectNodes(".//a");
            var genres = genreNodes.Select(x => x.InnerText).ToList();

            return new ScrapedMangaEntry()
            {
                Id = mangaId,
                Name = mangaName,
                Language = language,
                Description = description,
                Author = author,
                Artist = artist,
                ReleaseYear = Convert.ToInt32(releaseYear),
                CompletionStatus = completionStatus,
                Genres = genres
            };
        }

        private IEnumerable<string> ExtractMangaIds(string data, string pattern)
        {
            var matches = Regex.Matches(data, pattern);
            
            HashSet<string> mangaIds = new HashSet<string>();

            foreach (Match match in matches)
            {
                mangaIds.Add(match.Groups[1].Value);
            }

            return mangaIds.AsEnumerable();
        }
	}
}

