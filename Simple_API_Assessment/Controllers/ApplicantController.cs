using APIExample.Models;
using Microsoft.AspNetCore.Mvc;
using Simple_API_Assessment.Data.Repository;

namespace Simple_API_Assessment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicantController : ControllerBase
    {
        private readonly IApplicantRepository _applicantRepository;

        public ApplicantController(IApplicantRepository applicantRepository)
        {
            _applicantRepository = applicantRepository;
        }

        #region Endpoints for list Applicants
        //
        //Get all Applicants and your Skills
        [HttpGet]
        public ActionResult<IEnumerable<Applicant>> GetAllApplicants()
        {
            var applicants = _applicantRepository.GetAllApplicantsWithSkills();
            return Ok(applicants);
        }
        #endregion


        #region Endpoints for a single id
        //Get Applicant by ID
        [HttpGet("{id}")]
        public ActionResult<Applicant> GetApplicantById(int id)
        {
            var applicant = _applicantRepository.GetApplicantById(id);
            if (applicant == null)
            {
                return NotFound();
            }
            return Ok(applicant);
        }

        //New Applicant
        [HttpPost]
        public ActionResult<Applicant> AddApplicant(Applicant applicant)
        {
            _applicantRepository.AddApplicant(applicant);
            if (applicant == null)
            {
                return NotFound();
            }
            return Ok(applicant);
        }

        //Update Applicant
        [HttpPut("{id}")]
        public IActionResult UpdateApplicant(int id, Applicant applicant)
        {
            if (id != applicant.Id)
            {
                return BadRequest();
            }

            try
            {
                _applicantRepository.UpdateApplicant(applicant);
            }
            catch (Exception)
            {
                return NotFound();
            }
            return NoContent();
        }

        //Delete Applicant
        [HttpDelete("{id}")]
        public IActionResult DeleteApplicant(int id)
        {
            var existingApplicant = _applicantRepository.GetApplicantById(id);
            if (existingApplicant == null)
            {
                return NotFound();
            }

            _applicantRepository.DeleteApplicant(id);
            return NoContent();
        }
        #endregion
    }
}
