using APIExample.Models;
using Microsoft.AspNetCore.Mvc;
using Simple_API_Assessment.Data.Repository;
using Simple_API_Assessment.Filters;

namespace Simple_API_Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UnprocessableEntityOnInvalidModel]
    public class LessonController : ControllerBase
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ITeacherRepository _teacherRepository;

        public LessonController(ILessonRepository lessonRepository, ITeacherRepository teacherRepository)
        {
            _lessonRepository = lessonRepository;
            _teacherRepository = teacherRepository;
        }

        //New Lesson
        //DisciplineId, Date (yyyy-MM-dd), NumberPresent and NumberAbsent are required and the numbers must not be negative.
        //The discipline ID must be the discipline of an existing teacher; otherwise the request is rejected with 422
        //and no lesson is stored.
        [HttpPost]
        [ProducesResponseType(typeof(Lesson), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<Lesson> RegisterLesson([FromBody] LessonRegistrationRequest request)
        {
            var disciplineId = request.DisciplineId!;

            if (!_teacherRepository.DisciplineExists(disciplineId))
            {
                ModelState.AddModelError(nameof(LessonRegistrationRequest.DisciplineId),
                    $"The discipline ID '{disciplineId}' is not linked to any teacher.");
                var problem = new ValidationProblemDetails(ModelState)
                {
                    Status = StatusCodes.Status422UnprocessableEntity
                };
                return UnprocessableEntity(problem);
            }

            var lesson = new Lesson
            {
                DisciplineId = disciplineId,
                Date = request.Date!.Value,
                NumberPresent = request.NumberPresent!.Value,
                NumberAbsent = request.NumberAbsent!.Value
            };

            _lessonRepository.AddLesson(lesson);
            return StatusCode(StatusCodes.Status201Created, lesson);
        }
    }
}
