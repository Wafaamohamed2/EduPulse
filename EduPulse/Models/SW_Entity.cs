using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EduPulse.Models.Users;
using EduPulse.Models.Exam_Sub;
namespace SW_Project.Models
{
    public class SW_Entity : DbContext
    {

        public SW_Entity(DbContextOptions<SW_Entity> options) : base(options)
     
        {


        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Attendence> Attendances { get; set; }
        public DbSet<AttendenceRecord> AttendenceRecords { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentSubject>()
               .HasKey(ss => new { ss.StudentId, ss.TeacherId });


            modelBuilder.Entity<StudentSubject>()
               .HasOne(ss => ss.Student)
               .WithMany(s => s.StudentSubjects)
               .HasForeignKey(ss => ss.StudentId);

            modelBuilder.Entity<StudentSubject>()
              .HasOne(ss => ss.Teacher)
              .WithMany(t => t.StudentSubjects)
              .HasForeignKey(ss => ss.TeacherId);

            modelBuilder.Entity<Student>()
              .HasIndex(s => s.FingerId)
              .IsUnique();

            modelBuilder.Entity<Attendence>()
             .HasOne(a => a.Student)
             .WithMany(s => s.Attendances)
             .HasForeignKey(a => a.studentId);

            modelBuilder.Entity<Parent>()
            .HasOne(p => p.student)
            .WithOne(s => s.parent)
            .HasForeignKey<Parent>(p => p.studentId);

            modelBuilder.Entity<Exam>()
            .HasMany(e => e.Students)
            .WithMany(s => s.Exams)
            .UsingEntity(j => j.ToTable("ExamStudents"));   


            base.OnModelCreating(modelBuilder);

        }
    }
}