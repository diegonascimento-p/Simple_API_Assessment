using APIExample.Data;
using APIExample.Models;

namespace Simple_API_Assessment.Data.Repository
{
    public class LessonRepo : ILessonRepository
    {
        private readonly DataContext _context;

        public LessonRepo(DataContext context)
        {
            _context = context;
        }

        public void AddLesson(Lesson lesson)
        {
            _context.Lessons.Add(lesson);
            _context.SaveChanges();
        }
    }
}
