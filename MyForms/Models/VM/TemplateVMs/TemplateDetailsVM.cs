namespace MyForms.Models.VM.TemplateVMs
{
    public class TemplateDetailsVM
    {
        public Template Template { get; set; } = null!;
        public LikeVM LikeVM { get; set; } = null!;
        public CommentsVM CommentsVM { get; set; } = null!;
        public bool IsOwner  { get; set; }
    }
}
