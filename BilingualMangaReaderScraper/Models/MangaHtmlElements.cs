namespace BilingualMangaReaderScraper.Models
{
    public static class MangaHtmlElements
    {

        public static string TITLE_ID = "mainimagetitle";

        public static string CHAPTER_LIST_CONTAINER_ID = "chlist";

        public static string DESCRIPTION_SELECTOR = "#metac > div.metaesyn";

        public static string AUTHOR_SELECTOR = "#metac > div:nth-child(6) > a";

        public static string ARTIST_SELECTOR = "#metac > div:nth-child(8) > a";

        public static string RELEASE_YEAR_SELECTOR = "#metac > div:nth-child(9) > div:nth-child(1) > div > a";

        public static string COMPLETED_SELECTOR = "#metac > div:nth-child(9) > div:nth-child(2) > div > a";

        public static string GENRES_LIST_CONTAINER_SELECTOR = "#metac > div:nth-child(4)";

    }
}
