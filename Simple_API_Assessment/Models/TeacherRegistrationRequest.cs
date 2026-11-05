using System.ComponentModel.DataAnnotations;

namespace APIExample.Models
{
    //Payload accepted by the teacher registration endpoint.
    //Every attribute is nullable so that a missing value can be told apart from a default one
    //and reported as a validation error. Blank or whitespace-only strings are treated as missing.
    public class TeacherRegistrationRequest
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Discipline { get; set; }

        [Required]
        public int? Age { get; set; }
    }
}
