using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Linq;
using System.Globalization;
using BlogML;
using BlogML.Xml;
using FujiyBlog.Core.DomainObjects;
using FujiyBlog.Core.Repositories;

namespace FujiyBlog.Core.BlogML
{
    /// <summary>
    /// Class to validate BlogML data and import it into Blog
    /// </summary>
    public class BlogReader
    {
        private readonly IBlogMLRepository blogMLRepository;

        public BlogReader(IBlogMLRepository blogMLRepository)
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
            BlogMLBlog blog = BlogMLSerializer.Deserialize(xmlBlogML);

            LoadCategories(blog);
            LoadPosts(blog);
            //LoadFromXmlDocument();

            return true;
        }

        private void LoadCategories(BlogMLBlog blog)
        {
            foreach (BlogMLCategory blogMLCategory in blog.Categories)
            {
                
            }
        }

        private void LoadPosts(BlogMLBlog blog)
        {
            foreach (BlogMLPost blogMLPost in blog.Posts)
            {
                Post post = new Post
                                {
                                    Title = blogMLPost.Title,
                                    Description = blogMLPost.Excerpt.UncodedText,
                                    Slug = blogMLPost.PostUrl,
                                    Content = blogMLPost.Content.UncodedText,
                                    CreationDate = blogMLPost.DateCreated,
                                    LastModificationDate = blogMLPost.DateModified,
                                    PublicationDate = blogMLPost.DateCreated,
                                    IsPublished =  blogMLPost.Approved,
                                };

                foreach (BlogMLComment blogMLComment in blogMLPost.Comments)
                {
                    PostComment postComment = new PostComment
                                                  {
                                                      AuthorName = blogMLComment.UserName,
                                                      AuthorEmail = blogMLComment.UserEMail,
                                                      AuthorWebsite = blogMLComment.UserUrl,
                                                      Comment = blogMLComment.Content.UncodedText,
                                                      CreationDate = blogMLComment.DateCreated,
                                                      IsApproved = blogMLComment.Approved

                                                  };

                    post.Comments.Add(postComment);
                }

                blogMLRepository.AddPost(post);
            }
        }

        /// <summary>
        /// BlogML does not support tags - load directly fro XML
        /// </summary>
        //private void LoadFromXmlDocument()
        //{
        //    var doc = new XmlDocument();
        //    doc.Load(XmlReader);
        //    var posts = doc.GetElementsByTagName("post");

        //    foreach (XmlNode post in posts)
        //    {
        //        var blogX = new BlogMlExtendedPost();

        //        if (post.ChildNodes.Count <= 0)
        //        {
        //            blogsExtended.Add(blogX);
        //            continue;
        //        }

        //        foreach (XmlNode child in post.ChildNodes)
        //        {
        //            if (child.Name == "tags")
        //            {
        //                foreach (XmlNode tag in child.ChildNodes)
        //                {
        //                    if (tag.Attributes != null)
        //                    {
        //                        if (blogX.Tags == null)
        //                        {
        //                            blogX.Tags = new List<Tag>();
        //                        }
        //                        blogX.Tags.Add(new Tag { Name = tag.Attributes["ref"].Value });
        //                    }
        //                }
        //            }
        //        }
        //        blogsExtended.Add(blogX);
        //    }
        //}
    }
}