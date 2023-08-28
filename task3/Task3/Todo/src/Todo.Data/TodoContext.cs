using Todo.Models;
using Microsoft.EntityFrameworkCore;
using Task = Todo.Models.Task;
using System.Text.Json;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace Todo.Data
{
    public class TodoContext : DbContext
    {
        private readonly string fileName = "data.json";
        public TodoContext()
        {
        }

        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseInMemoryDatabase("TodoDb");
                optionsBuilder.UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasKey(x => x.CategoryId);
            modelBuilder.Entity<Task>().HasKey(x => x.TaskId);
            modelBuilder.Entity<User>().HasKey(x => x.UserId);

            modelBuilder.Entity<Category>(entity => { entity.Property(e => e.CategoryId).IsRequired(); });
            modelBuilder.Entity<User>(entity => { entity.Property(e => e.UserId).IsRequired(); });

            modelBuilder.Entity<Task>(
                entity =>
                {
                    entity.HasOne(d => d.Category)
                        .WithMany(p => p.Tasks)
                        .HasForeignKey("CategoryId");
                    entity.HasOne(d => d.User)
                        .WithMany(p => p.Tasks)
                        .HasForeignKey("UserId");
                });

            if (File.Exists(fileName))
            {
                ReadDataFromFile(modelBuilder);
            }
            else
            {
                modelBuilder.Entity<Category>().HasData(
                    new Category { CategoryId = 10, Name = "School" },
                    new Category { CategoryId = 20, Name = "Work" }
                );    
                
                modelBuilder.Entity<User>().HasData(
                    new User { UserId = 101, FirstName = "Jan", LastName = "Kowalski" },
                    new User { UserId = 102, FirstName = "John", LastName = "Smith" }
                );

                modelBuilder.Entity<Task>().HasData(
                   new Task { TaskId = 1, Name = "Math exam", CategoryId = 10, UserId = 101, StartDate = new DateTime(2023, 09, 01), DueDate = new DateTime(2023, 09, 8), Description = "Adding and subtracting", Importance = Models.Task.Priority.Medium, IsCompleted = false },
                   new Task { TaskId = 2, Name = "Task 2", CategoryId = 10, UserId = 102, StartDate = new DateTime(2023, 10, 10), DueDate = new DateTime(2023, 10, 11), Description = "Description", Importance = Models.Task.Priority.Low, IsCompleted = false },
                   new Task { TaskId = 3, Name = "Project", CategoryId = 20, UserId = 102, StartDate = new DateTime(2023, 01, 01), DueDate = new DateTime(2023, 12, 31), Description = "Something important", Importance = Models.Task.Priority.VeryHigh, IsCompleted = false }
               );
            }
        }

        public void SaveDataToFile()
        {
            string[] dbSets = new string[3] {
                JsonSerializer.Serialize(Tasks.ToList()),
                JsonSerializer.Serialize(Categories.ToList()),
                JsonSerializer.Serialize(Users.ToList())
            };
            File.WriteAllLines(fileName, dbSets);
        }

        public void ReadDataFromFile(ModelBuilder modelBuilder)
        {
            string[] dbSets = File.ReadAllLines(fileName);
            modelBuilder.Entity<Task>().HasData(JsonSerializer.Deserialize<List<Task>>(dbSets[0]));
            modelBuilder.Entity<Category>().HasData(JsonSerializer.Deserialize<List<Category>>(dbSets[1]));
            modelBuilder.Entity<User>().HasData(JsonSerializer.Deserialize<List<User>>(dbSets[2]));
        }
    }
}
