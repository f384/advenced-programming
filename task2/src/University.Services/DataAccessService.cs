using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Data;
using System.Text.Json;
using University.Models;
using University.Interfaces;
using System.Collections.ObjectModel;

namespace University.Services
{
    public class DataAccessService : IDataAccessService
    {
        private UniversityContext _dbContext = null;

        public DataAccessService(UniversityContext context)
        {
            _dbContext = context;
        }

        public void SetDatabaseContext(DbContext context)
        {
            _dbContext = (UniversityContext)context;
        }

        public void SaveDataToFile(string filePath)
        {
            string[] dbSets = new string[4] {
                JsonSerializer.Serialize(_dbContext.Students.ToList()),
                JsonSerializer.Serialize(_dbContext.Courses.ToList()),
                JsonSerializer.Serialize(_dbContext.Exams.ToList()),
                JsonSerializer.Serialize(_dbContext.Books.ToList())
            };
            File.WriteAllLines(filePath, dbSets);
        }

        public void ReadDataFromFile(string filePath)
        {
            string[] dbSets = File.ReadAllLines(filePath);
            _dbContext.Database.EnsureDeleted();
            _dbContext.Students.AddRange(JsonSerializer.Deserialize<List<Student>>(dbSets[0]));
            _dbContext.Courses.AddRange(JsonSerializer.Deserialize<List<Course>>(dbSets[1]));
            _dbContext.Exams.AddRange(JsonSerializer.Deserialize<List<Exam>>(dbSets[2]));
            _dbContext.Books.AddRange(JsonSerializer.Deserialize<List<Book>>(dbSets[3]));
            _dbContext.SaveChanges();
        }

        public void AddEntity(IEntity entity)
        {
            if (entity is Student)
            {
                _dbContext.Students.Add(entity as Student);
            }
            else if (entity is Book)
            {
                _dbContext.Books.Add(entity as Book);
            }
            else if (entity is Exam)
            {
                _dbContext.Exams.Add(entity as Exam);
            }
            else if (entity is Course)
            {
                _dbContext.Courses.Add(entity as Course);
            }
            else
            {
                throw new ArgumentException("Unexpected entity class.");
            }
            _dbContext.SaveChanges();
        }

        public void RemoveEntity(IEntity entity)
        {
            if (entity is Student)
            {
                _dbContext.Students.Remove(entity as Student);
            }
            else if (entity is Book)
            {
                _dbContext.Books.Remove(entity as Book);
            }
            else if (entity is Exam)
            {
                _dbContext.Exams.Remove(entity as Exam);
            }
            else if (entity is Course)
            {
                _dbContext.Courses.Remove(entity as Course);
            }
            else
            {
                throw new ArgumentException("Unexpected entity class.");
            }
            _dbContext.SaveChanges();
        }

        public void EditEntity(IEntity entity)
        {
            if (entity is Student)
            {
                _dbContext.Entry(entity as Student).State = EntityState.Modified;
            }
            else if (entity is Book)
            {
                _dbContext.Entry(entity as Book).State = EntityState.Modified;
            }
            else if (entity is Exam)
            {
                _dbContext.Entry(entity as Exam).State = EntityState.Modified;
            }
            else if (entity is Course)
            {
                _dbContext.Entry(entity as Course).State = EntityState.Modified;
            }
            else
            {
                throw new ArgumentException("Unexpected entity class.");
            }
            _dbContext.SaveChanges();
        }

        public ObservableCollection<T> GetEntities<T>() where T : IEntity
        {
            _dbContext.Database.EnsureCreated();
            switch (typeof(T).Name)
            {
                case nameof(Student):
                    _dbContext.Students.Load();
                    return new ObservableCollection<T>((IEnumerable<T>)_dbContext.Students.Local.ToObservableCollection());
                case nameof(Book):
                    _dbContext.Books.Load();
                    return new ObservableCollection<T>((IEnumerable<T>)_dbContext.Books.Local.ToObservableCollection());
                case nameof(Exam):
                    _dbContext.Exams.Load();
                    return new ObservableCollection<T>((IEnumerable<T>)_dbContext.Exams.Local.ToObservableCollection());
                case nameof(Course):
                    _dbContext.Courses.Load();
                    return new ObservableCollection<T>((IEnumerable<T>)_dbContext.Courses.Local.ToObservableCollection());
                default:
                    return null;
            }
        }

        public T FindEntity<T>(string key) where T : IEntity
        {
            switch(typeof(T).Name)
            {
                case nameof(Student):
                    return (T)(IEntity)_dbContext.Students.Find(key);
                case nameof(Book):
                    return (T)(IEntity)_dbContext.Books.Find(key);
                case nameof(Course):
                    return (T)(IEntity)_dbContext.Courses.Find(key);
                case nameof(Exam):
                    return (T)(IEntity)_dbContext.Exams.Find(key);
                default:
                    return default;
            }
        }

        public void EnsureDeleted()
        {
            _dbContext.Database.EnsureDeleted();
        }
    }
}
