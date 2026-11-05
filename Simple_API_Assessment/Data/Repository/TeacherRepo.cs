using APIExample.Data;
using APIExample.Models;

namespace Simple_API_Assessment.Data.Repository
{
    public class TeacherRepo : ITeacherRepository
    {
        private readonly DataContext _context;

        public TeacherRepo(DataContext context)
        {
            _context = context;
        }

        public void AddTeacher(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            _context.SaveChanges();
        }
    }
}
