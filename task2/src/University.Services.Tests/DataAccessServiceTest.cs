using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Emit;
using University.Data;
using University.Interfaces;
using University.Models;
using System.Collections.ObjectModel;
using Telerik.JustMock;

namespace University.Services.Tests
{
    [TestClass]
    public class DataAccessServiceTest
    {
        private IDialogService _dialogService;
        private DbContextOptions<UniversityContext> _options;

        [TestInitialize()]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<UniversityContext>()
                .UseInMemoryDatabase(databaseName: "UniversityTestDB")
                .Options;
            SeedTestDB();
            _dialogService = new DialogService();
        }

        private void SeedTestDB()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                context.Database.EnsureDeleted();

                List<Student> students = new List<Student>
                {
                    new Student { StudentId = "1", Name = "Wieńczysław", LastName = "Nowakowicz", PESEL="PESEL1", BirthDate = new DateTime(1987, 05, 22) },
                    new Student { StudentId = "2", Name = "Stanisław", LastName = "Nowakowicz", PESEL = "PESEL2", BirthDate = new DateTime(2019, 06, 25) },
                    new Student { StudentId = "3", Name = "Eugenia", LastName = "Nowakowicz", PESEL = "PESEL3", BirthDate = new DateTime(2021, 06, 08) }
                };

                List<Course> courses = new List<Course>
                {
                    new Course { CourseCode = "MAT", Title = "Matematyka", Instructor = "Michalina Beldzik", Schedule = "schedule1", Description = "description1", Credits = 5, Department = "department1" },
                    new Course { CourseCode = "BIOL", Title = "Biologia", Instructor = "Halina", Schedule = "schedule2", Description = "description2", Credits = 6, Department = "department3" },
                    new Course { CourseCode = "CHEM", Title = "Chemia", Instructor = "Jan Nowak", Schedule = "schedule3", Description = "description3", Credits = 7, Department = "department3" }
                };

                List<Book> books = new List<Book>
                {
                    new Book { BookId = "B0001", Title = "Brave New World", Author = "Aldous Huxley", ISBN = "978-0060850524", Publisher = "Harper Perennial", PublicationDate = new DateTime(2006, 11, 26), Description = "Description... ", Genre = "Novel" },
                    new Book { BookId = "B0002", Title = "The Shining", Author = "Stephen King", ISBN = "978-0307743657", Publisher = "Random House US", PublicationDate = new DateTime(2012, 1, 1), Description = "Description 2... ", Genre = "Novel" }
                };

                List<Exam> exams = new List<Exam>
                {
                    new Exam { ExamId = "E001", CourseCode = "MAT", Date = new DateTime(2024, 10, 1), StartTime = new DateTime(2000, 1, 1, 9, 0, 0), EndTime = new DateTime(2000, 1, 1, 11, 0, 0), Description = "Final exam", Location = "location1", Professor = "Michalina Warszawa" }
                };

                context.Students.AddRange(students);
                context.Courses.AddRange(courses);
                context.Books.AddRange(books);
                context.Exams.AddRange(exams);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public void Test1_SaveDataToFile()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                string fileName = "data.json";
                IDataAccessService dataAccessService = new DataAccessService(context);
                dataAccessService.SaveDataToFile(fileName);
                Assert.IsTrue(File.Exists(fileName));
            }
        }

        [TestMethod]
        public void Test2_LoadDataFromFile()
        {
            using UniversityContext context = new UniversityContext(_options);
            {
                const string fileName = "data.json";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                var mockDataAccessService = Mock.Create<IDataAccessService>();
                Mock.Arrange(() => mockDataAccessService.SaveDataToFile(Arg.AnyString)).DoInstead(
                    () =>
                    {
                        const string fileContent = "[{\"StudentId\":\"1\",\"Name\":\"Wie\\u0144czys\\u0142aw\",\"LastName\":\"Nowakowicz\",\"PESEL\":\"PESEL1\",\"BirthDate\":\"1987-05-22T00:00:00\",\"BirthPlace\":\"\",\"ResidencePlace\":\"\",\"AddressLine1\":\"\",\"AddressLine2\":\"\",\"Courses\":null,\"Exams\":null},{\"StudentId\":\"2\",\"Name\":\"Stanis\\u0142aw\",\"LastName\":\"Nowakowicz\",\"PESEL\":\"PESEL2\",\"BirthDate\":\"2019-06-25T00:00:00\",\"BirthPlace\":\"\",\"ResidencePlace\":\"\",\"AddressLine1\":\"\",\"AddressLine2\":\"\",\"Courses\":null,\"Exams\":null},{\"StudentId\":\"3\",\"Name\":\"Eugenia\",\"LastName\":\"Nowakowicz\",\"PESEL\":\"PESEL3\",\"BirthDate\":\"2021-06-08T00:00:00\",\"BirthPlace\":\"\",\"ResidencePlace\":\"\",\"AddressLine1\":\"\",\"AddressLine2\":\"\",\"Courses\":null,\"Exams\":null}]\r\n[{\"CourseCode\":\"MAT\",\"Title\":\"Matematyka\",\"Instructor\":\"Michalina Beldzik\",\"Schedule\":\"schedule1\",\"Description\":\"description1\",\"Credits\":5,\"Department\":\"department1\",\"IsSelected\":false},{\"CourseCode\":\"BIOL\",\"Title\":\"Biologia\",\"Instructor\":\"Halina\",\"Schedule\":\"schedule2\",\"Description\":\"description2\",\"Credits\":6,\"Department\":\"department3\",\"IsSelected\":false},{\"CourseCode\":\"CHEM\",\"Title\":\"Chemia\",\"Instructor\":\"Jan Nowak\",\"Schedule\":\"schedule3\",\"Description\":\"description3\",\"Credits\":7,\"Department\":\"department3\",\"IsSelected\":false}]\r\n[{\"ExamId\":\"E001\",\"CourseCode\":\"MAT\",\"Date\":\"2024-10-01T00:00:00\",\"StartTime\":\"2000-01-01T09:00:00\",\"EndTime\":\"2000-01-01T11:00:00\",\"Location\":\"location1\",\"Description\":\"Final exam\",\"Professor\":\"Michalina Warszawa\",\"IsSelected\":false}]\r\n[{\"BookId\":\"B0001\",\"Title\":\"Brave New World\",\"Author\":\"Aldous Huxley\",\"Publisher\":\"Harper Perennial\",\"PublicationDate\":\"2006-11-26T00:00:00\",\"ISBN\":\"978-0060850524\",\"Genre\":\"Novel\",\"Description\":\"Description... \"},{\"BookId\":\"B0002\",\"Title\":\"The Shining\",\"Author\":\"Stephen King\",\"Publisher\":\"Random House US\",\"PublicationDate\":\"2012-01-01T00:00:00\",\"ISBN\":\"978-0307743657\",\"Genre\":\"Novel\",\"Description\":\"Description 2... \"}]\n";
                        File.WriteAllText(fileName, fileContent);
                    }
                );
                
                IDataAccessService dataAccessService = new DataAccessService(context);
                mockDataAccessService.SaveDataToFile(fileName);
                dataAccessService.ReadDataFromFile(fileName);
                Assert.IsTrue(context.Students.Count() == 3 && context.Courses.Count() == 3 && context.Books.Count() == 2 && context.Exams.Count() == 1);
            }
        }
    }
}