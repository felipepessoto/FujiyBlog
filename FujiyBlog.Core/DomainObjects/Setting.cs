namespace FujiyBlog.Core.DomainObjects
{
    public class Setting
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Value { get; set; }
    }
}
