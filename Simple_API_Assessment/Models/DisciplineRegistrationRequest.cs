using System.ComponentModel.DataAnnotations;

namespace APIExample.Models
{
    //Payload accepted by the discipline registration endpoint.
    //Every attribute is nullable so that a missing value can be told apart from a default one
    //and reported as a validation error. Blank or whitespace-only strings are treated as missing.
    public class DisciplineRegistrationRequest
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public int? NumberOfAlumns { get; set; }
    }
}
