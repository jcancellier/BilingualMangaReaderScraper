using BilingualMangaReaderScraper.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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

            // Get manga data
            foreach (var mangaId in mangaIds)
            {
                var scrapedMangeEntryEnglish = ScrapeMangaData(mangaId, web, doc, Languages.English);
                var scrapedMangeEntryJapanese = ScrapeMangaData(mangaId, web, doc, Languages.Japanese);

                _logger.LogInformation(scrapedMangeEntryEnglish.ToString());
                _logger.LogInformation(scrapedMangeEntryJapanese.ToString());
            }
        }

        private ScrapedMangaEntry ScrapeMangaData(
            string mangaId, HtmlWeb web, HtmlDocument doc, Languages language)
        {
            var languageString = language switch
            {
                Languages.English => "en",
                Languages.Japanese => "jp",
                _ => "en"
            };

            // load manga web page with selenium since it is a dynamic web page
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");

            string html;
            using (var driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), chromeOptions))
            {
                driver.Navigate().GoToUrl($"{BASE_URL}/manga/{mangaId}?lang={languageString}");

                // Wait for the page to load
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                var element = wait.Until(d => d.FindElement(By.Id(MangaHtmlElementIds.CHAPTER_LIST_CONTAINER)));

                html = driver.PageSource;
            }

            // parse html for manga data
            doc.LoadHtml(html);

            var mangaName = doc.GetElementbyId(MangaHtmlElementIds.TITLE).InnerHtml;
            var description = doc.DocumentNode.QuerySelector(MangaHtmlElementIds.DESCRIPTION_SELECTOR).InnerText;

            return new ScrapedMangaEntry()
            {
                Id = mangaId,
                Name = mangaName,
                Language = language,
                Description = description
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

