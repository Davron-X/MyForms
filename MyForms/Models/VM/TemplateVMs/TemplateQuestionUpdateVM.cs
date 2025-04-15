namespace MyForms.Models.VM.TemplateVMs
{
    public class TemplateQuestionUpdateVM
    {
        public int TemplateId { get; set; }
        public IList<Question> Questions { get; set; } = new List<Question>();
    }
}
