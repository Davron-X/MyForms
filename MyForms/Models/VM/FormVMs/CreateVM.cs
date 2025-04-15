namespace MyForms.Models.VM.Form
{
    public class CreateVM
    {
        public Template? Template { get; set; }
        public IList<FormAnswer> FormAnswers { get; set; } = new List<FormAnswer>();
    }
}
