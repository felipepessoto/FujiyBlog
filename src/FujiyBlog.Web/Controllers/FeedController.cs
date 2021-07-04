using FujiyBlog.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace FujiyBlog.Web.Controllers
{
    [ResponseCache(Duration = 100)]
    public class FeedController : Controller
    {
        private readonly FeedGenerator feedGenerator;

        public FeedController(FeedGenerator feedGenerator)
        {
            this.feedGenerator = feedGenerator;            
        }

        public ActionResult Rss20()
        {
            //TODO cache on server-side
            SyndicationFeedFormatter feedFormatter = new Rss20FeedFormatter(feedGenerator.GetBlog()); //Atom10FeedFormatter

            StringBuilder rssBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(rssBuilder))
            {
                feedFormatter.WriteTo(writer);
            }

            return new ContentResult
            {
                ContentType = "application/xml",
                Content = rssBuilder.ToString(),
                StatusCode = 200
            };
        }
    }
}
