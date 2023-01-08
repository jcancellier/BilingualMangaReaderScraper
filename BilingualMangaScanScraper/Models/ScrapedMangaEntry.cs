namespace BilingualMangaScanScraper.Models
{
    public class ScrapedMangaEntry
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public string Artist { get; set; }

        public Languages Language { get; set; }

        public int ReleaseYear { get; set; }

        public string CompletionStatus { get; set; }

        public IList<string> Genres { get; set; }
    }
}
