using System;
using System.Xml;
using HtmlAgilityPack;

namespace BilingualMangaReaderScraper.Services
{
	public class MangaScraperService
	{
		public MangaScraperService()
		{
            // Load web page
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://bilingualmanga.net");

            // Get book ids

        }
	}
}

