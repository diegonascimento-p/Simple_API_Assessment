using APIExample.Models;
using Microsoft.AspNetCore.Mvc;
using Simple_API_Assessment.Data.Repository;
using Simple_API_Assessment.Filters;

namespace Simple_API_Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UnprocessableEntityOnInvalidModel]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepository;

        public TeacherController(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        //New Teacher
        //Name, Discipline and Age are required; a request missing any of them is rejected with 422.
        [HttpPost]
        [ProducesResponseType(typeof(Teacher), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<Teacher> RegisterTeacher([FromBody] TeacherRegistrationRequest request)
        {
            var teacher = new Teacher
            {
                Name = request.Name!,
                Discipline = request.Discipline!,
                Age = request.Age!.Value
            };

            _teacherRepository.AddTeacher(teacher);
            return StatusCode(StatusCodes.Status201Created, teacher);
        }
    }
}
