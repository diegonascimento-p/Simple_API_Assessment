using System.ComponentModel.DataAnnotations;

namespace APIExample.Models
{
    public class Applicant
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Skill> Skills { get; set; }
    }
}
