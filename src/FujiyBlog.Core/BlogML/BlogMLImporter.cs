using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using BlogML.Xml;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.EntityFramework;

namespace FujiyBlog.Core.BlogML
{
    /// <summary>
    /// Class to validate BlogML data and import it into Blog
    /// </summary>
    public class BlogMLImporter
    {
        private readonly BlogMLRepository blogMLRepository;
        private List<Category> allCategories;
        private List<Tag> allTags;
        private List<User> allUsers;

        public BlogMLImporter(BlogMLRepository blogMLRepository)
        {
            this.blogMLRepository = blogMLRepository;
        }

        /// <summary>
        /// Imports BlogML file into blog
        /// </summary>
        /// <returns>
        /// True if successful
        /// </returns>
        public bool Import(Stream xmlBlogML)
        {
            long initialStreamPosition = xmlBlogML.Position;
            BlogMLBlog blog = BlogMLSerializer.Deserialize(xmlBlogML);
            xmlBlogML.Position = initialStreamPosition;
            List<BlogMLExtendedPost> extendedPosts = LoadFromXmlDocument(xmlBlogML, blog.Posts);

            allCategories = blogMLRepository.GetAllCategories();
            allTags = blogMLRepository.GetAllTags();
            allUsers = blogMLRepository.GetAllUsers();
            

            InsertTags(extendedPosts.SelectMany(x => x.Tags));
            InsertCategories(blog.Categories);
            InsertUsers(blog.Authors);
            InsertPosts(blog, extendedPosts);
            

            return true;
        }

        private void InsertUsers(IEnumerable<BlogMLAuthor> authors)
        {
            foreach (BlogMLAuthor blogMLAuthor in authors)
            {
                string userName = blogMLAuthor.ID;
                if (blogMLRepository.GetUser(userName) == null && !allUsers.Any(x => string.Compare(x.Username, userName, StringComparison.InvariantCultureIgnoreCase) == 0))
                {
                    User user = new User
                                    {
                                        Username = userName,
                                        CreationDate = DateTime.UtcNow,
                                        Password = Guid.NewGuid().ToString(),
                                        Email = Guid.NewGuid() + "@importer.blog",
                                    };
                    blogMLRepository.AddUser(user);
                    allUsers.Add(user);
                }
            }
        }

        private void InsertTags(IEnumerable<string> tags)
        {
            foreach (string tagName in tags)
            {
                if (blogMLRepository.GetTag(tagName) == null && !allTags.Any(x => string.Compare(x.Name, tagName, StringComparison.InvariantCultureIgnoreCase) == 0))
                {
                    Tag tag = new Tag {Name = tagName};
                    blogMLRepository.AddTag(tag);
                    allTags.Add(tag);
                }
            }
        }

        private void InsertCategories(IEnumerable<BlogMLCategory> blogMLCategories)
        {
            foreach (BlogMLCategory blogMLCategory in blogMLCategories)
            {
                if (blogMLRepository.GetCategory(blogMLCategory.Title) == null && !allCategories.Any(x => string.Compare(x.Name, blogMLCategory.Title, StringComparison.InvariantCultureIgnoreCase) == 0))
                {
                    Category category = new Category {Name = blogMLCategory.Title};
                    blogMLRepository.AddCategory(category);
                    allCategories.Add(category);
                }
            }
        }

        private void InsertPosts(BlogMLBlog blogML, IEnumerable<BlogMLExtendedPost> blogPosts)
        {
            foreach (BlogMLExtendedPost blogMLPost in blogPosts.Where(x => blogMLRepository.GetPost(GenerateSlug(x.Post.PostUrl)) == null))
            {
                string slug = GenerateSlug(blogMLPost.Post.PostUrl);

                Post post = new Post
                                {
                                    Title = blogMLPost.Post.Title,
                                    Description = blogMLPost.Post.Excerpt.UncodedText,
                                    Slug = slug,
                                    Content = blogMLPost.Post.Content.UncodedText,
                                    CreationDate = blogMLPost.Post.DateCreated,
                                    LastModificationDate = blogMLPost.Post.DateModified,
                                    PublicationDate = blogMLPost.Post.DateCreated,
                                    IsPublished = blogMLPost.Post.Approved,
                                    Author = allUsers.Single(x=> x.Username == blogMLPost.Post.Authors[0].Ref),
                                    IsCommentEnabled = true,
                                };

                foreach (BlogMLComment blogMLComment in blogMLPost.Post.Comments)
                {
                    PostComment postComment = new PostComment
                                                  {
                                                      AuthorName = blogMLComment.UserName,
                                                      AuthorEmail = blogMLComment.UserEMail,
                                                      AuthorWebsite = blogMLComment.UserUrl,
                                                      Comment = blogMLComment.Content.UncodedText,
                                                      CreationDate = blogMLComment.DateCreated,
                                                      IsApproved = blogMLComment.Approved,
                                                      IpAddress = "127.0.0.1"
                                                  };

                    if (post.Comments == null)
                    {
                        post.Comments = new List<PostComment>();
                    }

                    post.Comments.Add(postComment);
                }

                foreach (BlogMLCategoryReference categoryRef in blogMLPost.Post.Categories)
                {
                    BlogMLCategory blogMLCategory = blogML.Categories.Single(x => x.ID == categoryRef.Ref);
                    Category category = allCategories.Single(x => string.Compare(x.Name, blogMLCategory.Title, StringComparison.InvariantCultureIgnoreCase) == 0);

                    if (category.Posts == null)
                    {
                        category.Posts = new List<Post>();
                    }
                    category.Posts.Add(post);
                }

                foreach (string tagName in blogMLPost.Tags)
                {
                    Tag tag = allTags.Single(x => string.Compare(x.Name, tagName, StringComparison.InvariantCultureIgnoreCase) == 0);

                    if (tag.Posts == null)
                    {
                        tag.Posts = new List<Post>();
                    }

                    tag.Posts.Add(post);
                }

                blogMLRepository.AddPost(post);
            }
        }

        private List<BlogMLExtendedPost> LoadFromXmlDocument(Stream xmlBlogML, IEnumerable<BlogMLPost> blogMLPosts)
        {
            string blogMLNamespace = "http://www.blogml.com/2006/09/BlogML";

            XDocument doc = XDocument.Load(xmlBlogML);
            IEnumerable<XElement> posts = doc.Root.Element(XName.Get("posts", blogMLNamespace)).Elements(XName.Get("post", blogMLNamespace));

            List<BlogMLExtendedPost> list = new List<BlogMLExtendedPost>();

            foreach (BlogMLPost blogMLPost in blogMLPosts)
            {
                XElement postXml = posts.Single(x => x.Attribute("id").Value == blogMLPost.ID);

                XElement tagsElement = postXml.Element(XName.Get("tags", blogMLNamespace));
                IEnumerable<string> tags;

                if (tagsElement != null)
                {
                    tags = tagsElement.Elements(XName.Get("tag", blogMLNamespace)).Select(x => x.Attribute("ref").Value);
                }
                else
                {
                    tags = Enumerable.Empty<string>();
                }
                list.Add(new BlogMLExtendedPost(blogMLPost, tags));
            }

            return list;
        }

        private static string GenerateSlug(string postUrl)
        {
            postUrl = postUrl.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
            postUrl = postUrl.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).First();

            postUrl = RemoveAccent(postUrl).ToLower();

            postUrl = Regex.Replace(postUrl, @"[^a-z0-9\s-]", ""); // invalid chars           
            postUrl = Regex.Replace(postUrl, @"\s+", " ").Trim(); // convert multiple spaces into one space   
            postUrl = postUrl.Substring(0, postUrl.Length <= 200 ? postUrl.Length : 200).Trim(); // cut and trim it   
            postUrl = Regex.Replace(postUrl, @"\s", "-"); // hyphens   

            return postUrl;
        }

        private static string RemoveAccent(string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }
    }
}