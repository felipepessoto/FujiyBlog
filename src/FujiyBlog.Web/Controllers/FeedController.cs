using System;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace FujiyBlog.Web.Controllers
{
    //TODO
    //[ResponseCache(CacheProfileName = "ByUserAndLastCache")]
    public class FeedController : Controller
    {
        private readonly FeedGenerator feedGenerator;

        public FeedController(FeedGenerator feedGenerator)
        {
            this.feedGenerator = feedGenerator;            
        }

        public async Task<ActionResult> Rss20()
        {
            feedGenerator.urlHelper = Url;
            string feedUrl = Url.Action("Rss20", "Feed");
            
            return new cloudscribe.Syndication.Web.XmlResult(new cloudscribe.Syndication.Models.Rss.DefaultXmlFormatter().BuildXml(await feedGenerator.GetChannel()));
        }

        //TODO
        //public ActionResult Atom10()
        //{
        //    string feedUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Action(MVC.Feed.Rss20());

        //    Func<User, string> authorUrl = x => Request.Url.Scheme + "://" + Request.Url.Authority + Url.Action(MVC.Post.Author(x.Username, null));
        //    Func<Post, string> postUrl = x => Request.Url.Scheme + "://" + Request.Url.Authority + Url.Action(MVC.Post.Details(x.Slug));

        //    return Content(feedGenerator.GetBlog<Atom10FeedFormatter>(feedUrl, authorUrl, postUrl));
        //}
    }
}
