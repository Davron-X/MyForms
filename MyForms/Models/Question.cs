using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyForms.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        [ForeignKey("TemplateId")]
        public Template? Template { get; set; }
        public int OrderIndex { get; set; }
        [Display(Name = "Question Text")]
        public string Text { get; set; } = string.Empty;
        [Display(Name = "Answer type")]
        public QuestionType Type { get; set; }
        public string? Description { get; set; }
        public bool ShowInTable { get; set; }
        public List<AnswerOption> AnswerOptions { get; set; } = new List<AnswerOption>();
    }
}
