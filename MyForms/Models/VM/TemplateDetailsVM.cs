namespace MyForms.Models.VM
{
    public class TemplateDetailsVM
    {
        public Template Template { get; set; } = null!;
        public LikeVM LikeVM { get; set; } = null!;
        public CommentsVM CommentsVM { get; set; } = null!;
    }
}
