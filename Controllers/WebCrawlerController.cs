using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using System.Net.Http;
using HtmlAgilityPack;

namespace Day61.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebCrawlerController : ControllerBase
    {
        // GET: api/<WebCrawlerController>
        [HttpGet]
        public async Task<List<string>> Get(string seed, int depth = 1)
        {
            List<string> listUrls = new List<string>();
            Func<string, int, Task> crawl = null;
            crawl = async (string url, int curDepth) => {
                List<string> list = new List<string>();

                var httpClient = new HttpClient();
                var html = await httpClient.GetStringAsync(HttpUtility.UrlDecode(seed));
                var document = new HtmlDocument();
                document.LoadHtml(html);

                var hyperLinks = document.DocumentNode.Descendants("a");

                if(curDepth < depth) {
                    foreach (var it in hyperLinks)
                    {
                        var href = it.GetAttributeValue("href", "");
                        listUrls.Add(href);
                        await crawl(href, curDepth + 1);
                    }
                } else {
                    foreach (var it in hyperLinks)
                    {
                        listUrls.Add(it.GetAttributeValue("href", ""));
                    }
                }
                
            };

            await crawl(seed, 1);

            return listUrls;
        }
    }
}
