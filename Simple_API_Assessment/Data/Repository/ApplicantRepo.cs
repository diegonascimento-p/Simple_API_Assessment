using System;
using System.Collections.Generic;
using System.Linq;
using APIExample.Data;
using APIExample.Models;
using Microsoft.EntityFrameworkCore;

namespace Simple_API_Assessment.Data.Repository
{
    public class ApplicantRepo : IApplicantRepository
    {
        private readonly DataContext _context;

        public ApplicantRepo(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Applicant> GetAllApplicantsWithSkills()
        {
            return _context.Applicants.Include(a => a.Skills).ToList();
        }

        public Applicant GetApplicantById(int id)
        {
            return _context.Applicants.Include(a => a.Skills).FirstOrDefault(a => a.Id == id);
        }

        public void AddApplicant(Applicant applicant)
        {
            _context.Applicants.Add(applicant);
            _context.SaveChanges();
        }

        public void UpdateApplicant(Applicant applicant)
        {
            _context.Entry(applicant).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteApplicant(int id)
        {
            var applicantToDelete = _context.Applicants.Find(id);
            if (applicantToDelete != null)
            {
                _context.Applicants.Remove(applicantToDelete);
                _context.SaveChanges();
            }
        }
    }
}
