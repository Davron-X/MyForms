namespace MyForms.Models.VM.FormVMs
{
    public class OverviewVM
    {
        public bool IsReadOnly { get; set; }
        public Models.Form Form { get; set; } = null!;
    }
}
