using FujiyBlog.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FujiyBlog.Web.Controllers
{
    //cloudscribe.Syndication already exposes a controller at /api/rss
    [ResponseCache(CacheProfileName = "RssCacheProfile")]
    public class FeedController : Controller
    {
        private readonly FeedGenerator feedGenerator;

        public FeedController(FeedGenerator feedGenerator)
        {
            this.feedGenerator = feedGenerator;            
        }

        public async Task<ActionResult> Rss20()
        {            
            return new cloudscribe.Syndication.Web.XmlResult(new cloudscribe.Syndication.Models.Rss.DefaultXmlFormatter().BuildXml(await feedGenerator.GetChannel()));
        }
    }
}
