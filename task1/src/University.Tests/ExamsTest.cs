using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests;

[TestClass]
public class ExamsTest
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
            List<Exam> exams = new List<Exam>
            {
                new Exam { ExamId = "E001", CourseCode = "MAT", Date = new DateTime(2024, 10, 1), StartTime = new DateTime(2000,1,1, 9, 0,0), EndTime = new DateTime(2000, 1, 1, 11, 0, 0), Description = "Final exam", Location = "location1", Professor = "Michalina Warszawa" }
            };
            context.Exams.AddRange(exams);
            context.SaveChanges();
        }
    }

    [TestMethod]
    public void Show_all_exams()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            ExamsViewModel examsViewModel = new ExamsViewModel(context, _dialogService);
            bool hasData = examsViewModel.Exams.Any();
            Assert.IsTrue(hasData);
        }
    }

    [TestMethod]
    public void AddExam()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            AddExamViewModel addExamViewModel = new AddExamViewModel(context, _dialogService)
            {
                ExamId = "E010", 
                CourseCode = "CHEM", 
                Date = new DateTime(2024, 12, 10), 
                StartTime = new DateTime(2000,1,1, 10, 0,0), 
                EndTime = new DateTime(2000, 1, 1, 12, 0, 0), 
                Description = "Exam description", 
                Location = "location2", 
                Professor = "Jan Kowalski"
            };
            addExamViewModel.Save.Execute(null);

            ExamsViewModel booksViewModel = new ExamsViewModel(context, _dialogService);
            bool hasData = booksViewModel.Exams.Where(x => x.ExamId == "E010").Any();
            Assert.IsTrue(hasData);
        }
    }
}
