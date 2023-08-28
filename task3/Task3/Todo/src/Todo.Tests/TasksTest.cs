using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Todo.Data;
using Todo.Interfaces;
using Todo.Models;
using Todo.Services;
using Task = Todo.Models.Task;

namespace Todo.Tests
{
    [TestClass]
    public class TasksTest
    {
        private IDialogService _dialogService;
        private DbContextOptions<TodoContext> _options;

        [TestInitialize()]
        public void Initialize()
        {
            _options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase(databaseName: "TodoTestDB")
                .Options;
            SeedTestDB();
            _dialogService = new DialogService();
        }

        private void SeedTestDB()
        {
            using TodoContext context = new TodoContext(_options);
            {
                context.Database.EnsureDeleted();
                List<Task> tasks = new List<Task>
                {
                    new Task { TaskId = 1, CategoryId = 1, Name = "Math exam", StartDate = new DateTime(2023, 09, 01), DueDate = new DateTime(2023, 09, 8), Description = "Adding and subtracting", Importance = Models.Task.Priority.Medium, IsCompleted = false }
                };

                List<Category> categories = new List<Category>
                {
                    new Category { CategoryId = 1, Name = "School" }
                };

                context.Tasks.AddRange(tasks);
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
        }
    }
}
