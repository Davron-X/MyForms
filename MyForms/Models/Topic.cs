using System.ComponentModel.DataAnnotations;

namespace MyForms.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
