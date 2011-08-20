using FujiyBlog.Core.DomainObjects;

namespace FujiyBlog.Core.Dto
{
    public class PostSummary
    {
        public Post Post { get; set; }
        public bool ShowFullPost { get; set; }

        public int CommentsTotal { get; set; }

        public Post PreviousPost { get; set; }
        public Post NextPost { get; set; }
    }
}
