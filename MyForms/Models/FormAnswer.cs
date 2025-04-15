using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyForms.Models
{
    public class FormAnswer
    {
        [Key]
        public int Id { get; set; }

        public string AnswerText { get; set; } = string.Empty;

        public int FormId { get; set; }

        [ForeignKey("FormId")]
        public Form? Form { get; set; }

        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}
