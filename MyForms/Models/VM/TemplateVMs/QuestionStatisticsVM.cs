namespace MyForms.Models.VM.TemplateVMs
{
    public class QuestionStatisticsVM
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public double? AverageValue { get; set; }
        public QuestionType Type { get; set; }
        public string? MostFrequentAnswer { get; set; }
        public int? Frequency { get; set; }
        public Dictionary<string,int>? OptionFrequencies { get; set; }
    }
}
