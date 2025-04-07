namespace MyForms.Models
{
    public class BaseEntity
    {
        public string? CreatedById { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
