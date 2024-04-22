using APIExample.Models;

namespace Simple_API_Assessment.Data.Repository
{
    public interface IApplicantRepository
    {
        IEnumerable<Applicant> GetAllApplicantsWithSkills();
        Applicant GetApplicantById(int id);
        void AddApplicant(Applicant applicant);
        void UpdateApplicant(Applicant applicant);
        void DeleteApplicant(int id);
    }
}
