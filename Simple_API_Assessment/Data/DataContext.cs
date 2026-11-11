using APIExample.Models;
using Microsoft.EntityFrameworkCore;


namespace APIExample.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>()
                .HasOne(s => s.Applicant)
                .WithMany(a => a.Skills)
                .HasForeignKey(s => s.ApplicantId)
                .IsRequired();
        }
    }
}
