using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FujiyBlog.Core.Services
{
    public class FeedGenerator
    {
        private readonly SettingRepository settingRepository;
        private readonly FeedRepository feedRepository;
        private readonly LinkGenerator linkGenerator;
        private readonly IHttpContextAccessor contextAccessor;

        public FeedGenerator(SettingRepository settingRepository, FeedRepository feedRepository, LinkGenerator linkGenerator, IHttpContextAccessor contextAccessor)
        {
            this.settingRepository = settingRepository;
            this.feedRepository = feedRepository;
            this.linkGenerator = linkGenerator;
            this.contextAccessor = contextAccessor;
        }

        public SyndicationFeed GetBlog()
        {
            string feedUrl = linkGenerator.GetUriByAction(contextAccessor.HttpContext, "Rss20", "Feed");

            SyndicationFeed feed = new SyndicationFeed(settingRepository.BlogName, settingRepository.BlogDescription, new Uri(feedUrl));

            //foreach (ApplicationUser user in feedRepository.GetAllUsers())
            //{
            //    feed.Authors.Add(new SyndicationPerson(user.Email, user.DisplayName, getAuthorUrl(user)));
            //}

            //foreach (Category category in feedRepository.GetAllCategories())
            //{
            //    feed.Categories.Add(new SyndicationCategory(category.Name));
            //}

            List<SyndicationItem> items = new List<SyndicationItem>(settingRepository.ItemsShownInFeed);

            foreach (Post post in feedRepository.GetPosts(settingRepository.ItemsShownInFeed))
            {
                string content = post.Content ?? string.Empty;

                int moreIndex = content.IndexOf("[more]", StringComparison.OrdinalIgnoreCase);
                if (moreIndex >= 0)
                {
                    content = content.Remove(moreIndex, 6);
                }

                items.Add(new SyndicationItem(
                              post.Title,
                              new TextSyndicationContent(content, TextSyndicationContentKind.Html),
                              new Uri(linkGenerator.GetUriByRouteValues(contextAccessor.HttpContext, "PostDetailId", new { Id = post.Id })),
                              post.Id.ToString(),
                              post.LastModificationDate));
            }

            feed.Items = items;

            return feed;
        }
    }
}
