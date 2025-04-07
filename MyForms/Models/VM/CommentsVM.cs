namespace MyForms.Models.VM
{
    public class CommentsVM
    {
        public int TemplateId { get; set; }
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
    }
}
