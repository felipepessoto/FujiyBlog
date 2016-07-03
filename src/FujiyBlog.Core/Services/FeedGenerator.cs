using cloudscribe.Syndication.Models.Rss;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FujiyBlog.Core.Services
{
    public class FeedGenerator : IChannelProvider
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly FeedRepository feedRepository;
        private readonly SettingRepository settings;
        public IUrlHelper urlHelper { get; set; }

        public FeedGenerator(FeedRepository feedRepository, SettingRepository settings, IHttpContextAccessor contextAccessor)
        {
            this.feedRepository = feedRepository;
            this.settings = settings;
            this.contextAccessor = contextAccessor;
            this.urlHelper = urlHelper;
        }

        public string Name
        {
            get
            {
                return settings.BlogName;
            }
        }

        public Task<RssChannel> GetChannel(CancellationToken cancellationToken = default(CancellationToken))
        {
            var posts = feedRepository.GetPosts(settings.ItemsShownInFeed);

            var channel = new RssChannel();
            channel.Title = settings.BlogName;
            channel.Description = settings.BlogDescription;

            foreach (Category category in feedRepository.GetAllCategories())
            {
                channel.Categories.Add(new RssCategory(category.Name));
            }

            foreach (var user in feedRepository.GetAllUsers())
            {
            //    feed.Authors.Add(new SyndicationPerson(user.Email, user.DisplayName, getAuthorUrl(user)));
            }

            channel.Generator = Name;

            var indexUrl = urlHelper.Action("Index", "Post", null, contextAccessor.HttpContext.Request.Scheme);
            channel.Link = new Uri(indexUrl);

            string feedUrl = urlHelper.Action("Rss20", "Feed", null, contextAccessor.HttpContext.Request.Scheme);

            channel.SelfLink = new Uri(feedUrl);

            var items = new List<RssItem>();
            foreach (var post in posts)
            {
                var rssItem = new RssItem();
                rssItem.Author = post.Author.DisplayName;

                foreach (var c in post.PostCategories)
                {
                    rssItem.Categories.Add(new RssCategory(c.Category.Name));
                }

                string content = post.Content ?? string.Empty;

                int moreIndex = content.IndexOf("[more]", StringComparison.OrdinalIgnoreCase);
                if (moreIndex >= 0)
                {
                    content = content.Remove(moreIndex, 6);
                }
                rssItem.Description = content;
                rssItem.Guid = new RssGuid(post.Id.ToString());
                string postUrl = urlHelper.RouteUrl("PostDetail", new { postSlug = post.Slug }, contextAccessor.HttpContext.Request.Scheme);

                rssItem.Link = new Uri(postUrl);
                rssItem.PublicationDate = post.LastModificationDate;
                rssItem.Title = post.Title;

                items.Add(rssItem);
            }

            channel.PublicationDate = posts.Max(x => (DateTime?)x.LastModificationDate).GetValueOrDefault(DateTime.MinValue);
            channel.Items = items;

            return Task.FromResult(channel);
        }
    }
}
