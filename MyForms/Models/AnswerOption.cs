using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyForms.Models
{
    public class AnswerOption
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}
