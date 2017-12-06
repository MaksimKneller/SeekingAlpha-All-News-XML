using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SeekingAlpha_All_News_XML
{

    // holds individual news items from the rss feed
    public class NewsItem
    {

        public String title;
        public String link;
        public List<String> categories;
        public DateTime pubdate;

        public NewsItem()
        {
            categories = new List<String>();
        }

    }

    class Program
    {

        // downloads, parses and collects news items into a list
        static public List<NewsItem> fetchNewsXML()
        {

            // result list
            List<NewsItem> parsedNewsItems = new List<NewsItem>();

            // request SeekingAlpha All News RSS feed
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://seekingalpha.com/market_currents.xml");
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            // fetch stream with StreamReader
            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            // create HtmlAgilityPack's doc from the downloaded data
            HtmlNode.ElementsFlags["link"] = HtmlElementFlag.Closed; // otherwise will output empty <link /> since default for <link> tags is HtmlElementFlag.Empty
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(results);


            // build a list of NewsItem items from each XML group
            HtmlNodeCollection itemElements = doc.DocumentNode.SelectNodes("//item");
            
            foreach (HtmlNode itemTag in itemElements)
            {
                
                HtmlNode itemTitle = itemTag.SelectSingleNode("title");
                HtmlNode itemLink = itemTag.SelectSingleNode("link");
                HtmlNodeCollection itemCategories = itemTag.SelectNodes("category");
                HtmlNode itemPubdate = itemTag.SelectSingleNode("pubdate");

                NewsItem itm = new NewsItem();

                if(itemTitle != null && itemCategories != null && itemLink != null && itemCategories != null && itemPubdate != null)
                {

                    itm.title = itemTitle.InnerHtml;
                    itm.link = itemLink.InnerHtml;

                    // there may be multiple category elements in the XML
                    foreach (HtmlNode category in itemCategories)
                                itm.categories.Add(category.InnerHtml);

                    itm.pubdate = Convert.ToDateTime(itemPubdate.InnerText);

                    parsedNewsItems.Add(itm);
                }

                
            }

            
            return parsedNewsItems;

        }

        static void Main(string[] args)
        {
            List<NewsItem> newsItems = fetchNewsXML();

            foreach(NewsItem itm in newsItems)
            {
                Console.WriteLine("Title: " + itm.title);
                Console.WriteLine("Link: " + itm.link);

                itm.categories.ForEach(Console.WriteLine);

                Console.WriteLine("Date: " + itm.pubdate);

                Console.WriteLine("-------------------------------------------");
            }

            Console.Read();

        }
    }
}
