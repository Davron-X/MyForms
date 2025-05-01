namespace MyForms.Models.VM.TemplateVMs
{
    public class TemplateStatisticsVM
    {
        public int TemplateId { get; set; }
        public IEnumerable<QuestionStatisticsVM> QuestionStatistics { get; set; } = new List<QuestionStatisticsVM>();
    }
}
