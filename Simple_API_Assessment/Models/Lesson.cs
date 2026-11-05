using System.ComponentModel.DataAnnotations;

namespace APIExample.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        //Identifies the discipline the lesson belongs to. It must match the discipline of an existing teacher.
        [Required]
        public string DisciplineId { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public int NumberPresent { get; set; }
        public int NumberAbsent { get; set; }
    }
}
