namespace MyForms.Models.VM.User
{
    public class ApplicationUserVM
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public bool IsBlocked { get; set; }
        public bool IsSelected { get; set; }

    }
}
