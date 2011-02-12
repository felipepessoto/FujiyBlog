using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Web.ViewModels
{
    public class PostDetails
    {
        public Post Post { get; set; }
        public Post PreviousPost { get; set; }
        public Post NextPost { get; set; }
    }
}