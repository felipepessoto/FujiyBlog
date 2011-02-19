using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.Core
{
    public class FeedGenerator
    {
        private readonly ISettingRepository settingRepository;
        private readonly IFeedRepository feedRepository;

        public FeedGenerator(ISettingRepository settingRepository, IFeedRepository feedRepository)
        {
            this.settingRepository = settingRepository;
            this.feedRepository = feedRepository;
        }

        public string GetBlog<T>(string feedUrl, Func<User, string> getAuthorUrl, Func<Post, string> getPostUrl) where T : SyndicationFeedFormatter
        {
            SyndicationFeed feed = new SyndicationFeed(settingRepository.BlogName, settingRepository.BlogDescription, new Uri(feedUrl));

            foreach (User user in feedRepository.GetAllUsers())
            {
                feed.Authors.Add(new SyndicationPerson(user.Email, user.DisplayName, getAuthorUrl(user)));
            }

            foreach (Category category in feedRepository.GetAllCategories())
            {
                feed.Categories.Add(new SyndicationCategory(category.Name));
            }

            List<SyndicationItem> items = new List<SyndicationItem>(settingRepository.PostsPerPage);

            foreach (Post post in feedRepository.GetPosts(settingRepository.PostsPerPage))
            {
                items.Add(new SyndicationItem(
                              post.Title,
                              post.Content,
                              new Uri(getPostUrl(post)),
                              post.Id.ToString(),
                              post.LastModificationDate));
            }

            feed.Items = items;

            SyndicationFeedFormatter feedFormatter;
            
            if (typeof(T) == typeof(Rss20FeedFormatter))
            {
                feedFormatter= new Rss20FeedFormatter(feed);
            }
            else if (typeof(T) == typeof(Atom10FeedFormatter))
            {
                feedFormatter = new Atom10FeedFormatter(feed);
            }
            else
            {
                return null;
            }

            StringBuilder rssBuilder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(rssBuilder))
            {
                feedFormatter.WriteTo(writer);
            }
            return rssBuilder.ToString();
        }
    }
}
