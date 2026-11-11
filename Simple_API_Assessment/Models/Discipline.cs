using System.ComponentModel.DataAnnotations;

namespace APIExample.Models
{
    public class Discipline
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public int NumberOfAlumns { get; set; }
    }
}
