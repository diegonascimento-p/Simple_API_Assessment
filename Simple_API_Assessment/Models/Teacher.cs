using System.ComponentModel.DataAnnotations;

namespace APIExample.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Discipline { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
