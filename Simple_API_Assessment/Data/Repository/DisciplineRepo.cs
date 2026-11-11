using APIExample.Data;
using APIExample.Models;

namespace Simple_API_Assessment.Data.Repository
{
    public class DisciplineRepo : IDisciplineRepository
    {
        private readonly DataContext _context;

        public DisciplineRepo(DataContext context)
        {
            _context = context;
        }

        public void AddDiscipline(Discipline discipline)
        {
            _context.Disciplines.Add(discipline);
            _context.SaveChanges();
        }
    }
}
