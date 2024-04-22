using System.ComponentModel.DataAnnotations;

namespace APIExample.Models
{
    public class Skill
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int ApplicantId { get; set; }
        public Applicant Applicant { get; set; }
    }
}
