using University.Models;
using Microsoft.EntityFrameworkCore;
using University.Services;

namespace University.Data
{
    public class UniversityContext : DbContext
    {
        public UniversityContext()
        {
        }

        public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Exam> Exams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("UniversityDb");
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().Ignore(s => s.IsSelected);

            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = "1", Name = "Wieńczysław", LastName = "Nowakowicz", PESEL = "PESEL1", BirthDate = new DateTime(1987, 05, 22), BirthPlace = "Warszawa", AddressLine1 = "ul. Długa 1", AddressLine2 = "", ResidencePlace = "Warszawa", Courses = new List<Course>() },
                new Student { StudentId = "2", Name = "Stanisław", LastName = "Nowakowicz", PESEL = "PESEL2", BirthDate = new DateTime(2019, 06, 25), BirthPlace = "Wrocław", AddressLine1 = "ul. Krótka 20", AddressLine2 = "", ResidencePlace = "Kraków", Courses = new List<Course>() },
                new Student { StudentId = "3", Name = "Eugenia", LastName = "Nowakowicz", PESEL = "PESEL3", BirthDate = new DateTime(2021, 06, 08), BirthPlace = "Poznań", AddressLine1 = "ul. Kolorowa 8", AddressLine2 = "", ResidencePlace = "Gdanśk", Courses = new List<Course>() });

            modelBuilder.Entity<Student>().HasKey(x => x.StudentId);

            modelBuilder.Entity<Course>().HasData(
                new Course { CourseCode = "MAT", Title = "Matematyka", Instructor = "Michalina Warszawa", Schedule = "schedule1", Description = "description1", Credits = 5, Department = "department1" },
                new Course { CourseCode = "BIOL", Title = "Biologia", Instructor = "Halina Katowice", Schedule = "schedule2", Description = "description2", Credits = 6, Department = "department3" },
                new Course { CourseCode = "CHEM", Title = "Chemia", Instructor = "Jan Nowak", Schedule = "schedule3", Description = "description3", Credits = 7, Department = "department3" }
            );
            modelBuilder.Entity<Course>().HasKey(x => x.CourseCode);
            
            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = "B0001", Title = "Brave New World", Author = "Aldous Huxley", ISBN = "978-0060850524", Publisher = "Harper Perennial", PublicationDate = new DateTime(2006, 11, 26), Description = "Description... ", Genre = "Novel" },
                new Book { BookId = "B0002", Title = "The Shining", Author = "Stephen King", ISBN = "978-0307743657", Publisher = "Random House US", PublicationDate = new DateTime(2012, 1, 1), Description = "Description 2... ", Genre = "Novel" }
            );
            modelBuilder.Entity<Book>().HasKey(x => x.BookId);

            modelBuilder.Entity<Exam>().HasData( 
                new Exam { ExamId = "E001", CourseCode = "MAT", Date = new DateTime(2024, 10, 1), StartTime = new DateTime(2000,1,1, 9, 0,0), EndTime = new DateTime(2000, 1, 1, 11, 0, 0), Description = "Final exam", Location = "location1", Professor = "Michalina Warszawa" } 
            );
            modelBuilder.Entity<Exam>().HasKey(x => x.ExamId);
        }
    }
}
