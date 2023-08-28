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
public class StudentsTest
{
    private IDialogService _dialogService;
    private IValidationService _validationService;
    private IDataAccessService _dataAccessService;
    private DbContextOptions<UniversityContext> _options;

    [TestInitialize()]
    public void Initialize()
    {
        _options = new DbContextOptionsBuilder<UniversityContext>()
            .UseInMemoryDatabase(databaseName: "UniversityTestDB")
            .Options;

        _dataAccessService = new DataAccessService(new(_options));
        _dialogService = new DialogService();
        _validationService = new ValidationService();

        SeedTestDB();
    }

    private void SeedTestDB()
    {
        _dataAccessService.EnsureDeleted();
        List<Student> students = new List<Student>
            {
                new Student { StudentId = "1", Name = "Wieñczys³aw", LastName = "Nowakowicz", PESEL="PESEL1", BirthDate = new DateTime(1987, 05, 22) },
                new Student { StudentId = "2", Name = "Stanis³aw", LastName = "Nowakowicz", PESEL = "PESEL2", BirthDate = new DateTime(2019, 06, 25) },
                new Student { StudentId = "3", Name = "Eugenia", LastName = "Nowakowicz", PESEL = "PESEL3", BirthDate = new DateTime(2021, 06, 08) }
            };
        List<Course> courses = new List<Course>
            {
                new Course { CourseCode = "MAT", Title = "Matematyka", Instructor = "Michalina Beldzik", Schedule = "schedule1", Description = "description1", Credits = 5, Department = "department1" },
                new Course { CourseCode = "BIOL", Title = "Biologia", Instructor = "Halina", Schedule = "schedule2", Description = "description2", Credits = 6, Department = "department3" },
                new Course { CourseCode = "CHEM", Title = "Chemia", Instructor = "Jan Nowak", Schedule = "schedule3", Description = "description3", Credits = 7, Department = "department3" }
            };

        foreach (Student student in students)
        {
            _dataAccessService.AddEntity(student);
        }

        foreach (Course course in courses)
        {
            _dataAccessService.AddEntity(course);
        }
    }


    [TestMethod]
    public void Show_all_students()
    {
        StudentsViewModel studentsViewModel = new StudentsViewModel(_dataAccessService, _dialogService, _validationService);
        bool hasData = studentsViewModel.Students.Any();
        Assert.IsTrue(hasData);
    }

    [TestMethod]
    public void Add_Studend_without_name()
    {
        AddStudentViewModel addStudentViewModel = new AddStudentViewModel(_dataAccessService, _dialogService, _validationService)
        {
            LastName = "Doe  III",
            PESEL = "67111994116",
            BirthDate = new DateTime(1967, 12, 06)
        };
        addStudentViewModel.Save.Execute(null);

        bool newStudentExists = _dataAccessService.GetEntities<Student>().Any(s => s.LastName == "Doe III" && s.PESEL == "67111994116");
        Assert.IsFalse(newStudentExists);
    }


    [TestMethod]
    public void Add_Studend_without_last_name()
    {
        AddStudentViewModel addStudentViewModel = new AddStudentViewModel(_dataAccessService, _dialogService, _validationService)
        {
            Name = "John IV",
            PESEL = "67111994116",
            BirthDate = new DateTime(1967, 12, 06)
        };
        addStudentViewModel.Save.Execute(null);

        bool newStudentExists = _dataAccessService.GetEntities<Student>().Any(s => s.Name == "John IV" && s.PESEL == "67111994116");
        Assert.IsFalse(newStudentExists);
    }


    [TestMethod]
    public void Add_Studend_without_PESEL()
    {
        AddStudentViewModel addStudentViewModel = new AddStudentViewModel(_dataAccessService, _dialogService, _validationService)
        {
            Name = "John",
            LastName = "Doe V",
            BirthDate = new DateTime(1967, 12, 06)
        };
        addStudentViewModel.Save.Execute(null);

        bool newStudentExists = _dataAccessService.GetEntities<Student>().Any(s => s.Name == "John" && s.LastName == "Doe V");
        Assert.IsFalse(newStudentExists);
    }
}
