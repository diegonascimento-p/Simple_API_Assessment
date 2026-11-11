using APIExample.Models;
using Microsoft.AspNetCore.Mvc;
using Simple_API_Assessment.Data.Repository;
using Simple_API_Assessment.Filters;

namespace Simple_API_Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [UnprocessableEntityOnInvalidModel]
    public class DisciplineController : ControllerBase
    {
        private readonly IDisciplineRepository _disciplineRepository;

        public DisciplineController(IDisciplineRepository disciplineRepository)
        {
            _disciplineRepository = disciplineRepository;
        }

        //New Discipline
        //Name and NumberOfAlumns are required; a request missing either of them is rejected with 422.
        [HttpPost]
        [ProducesResponseType(typeof(Discipline), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<Discipline> RegisterDiscipline([FromBody] DisciplineRegistrationRequest request)
        {
            var discipline = new Discipline
            {
                Name = request.Name!,
                NumberOfAlumns = request.NumberOfAlumns!.Value
            };

            _disciplineRepository.AddDiscipline(discipline);
            return StatusCode(StatusCodes.Status201Created, discipline);
        }
    }
}
