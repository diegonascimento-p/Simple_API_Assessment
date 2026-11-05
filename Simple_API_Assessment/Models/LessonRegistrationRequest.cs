using System.ComponentModel.DataAnnotations;

namespace APIExample.Models
{
    //Payload accepted by the lesson registration endpoint.
    //Every attribute is nullable so that a missing value can be told apart from a default one
    //and reported as a validation error. The date must be sent as "yyyy-MM-dd".
    public class LessonRegistrationRequest
    {
        [Required]
        public string? DisciplineId { get; set; }

        [Required]
        public DateOnly? Date { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The field {0} must not be negative.")]
        public int? NumberPresent { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "The field {0} must not be negative.")]
        public int? NumberAbsent { get; set; }
    }
}
