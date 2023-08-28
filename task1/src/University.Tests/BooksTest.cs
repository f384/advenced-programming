using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Emit;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;
using University.ViewModels;

namespace University.Tests;

[TestClass]
public class BooksTest
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
            List<Book> books = new List<Book>
            {
                new Book { BookId = "B0001", Title = "Brave New World", Author = "Aldous Huxley", ISBN = "978-0060850524", Publisher = "Harper Perennial", PublicationDate = new DateTime(2006, 11, 26), Description = "Description... ", Genre = "Novel" }
            };
            context.Books.AddRange(books);
            context.SaveChanges();
        }
    }

    [TestMethod]
    public void Show_all_books()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            BooksViewModel booksViewModel = new BooksViewModel(context, _dialogService);
            bool hasData = booksViewModel.Books.Any();
            Assert.IsTrue(hasData);
        }
    }    
    
    [TestMethod]
    public void AddBook()
    {
        using UniversityContext context = new UniversityContext(_options);
        {
            AddBookViewModel addBookViewModel = new AddBookViewModel(context, _dialogService)
            {
                BookId = "B0002",
                Title = "The Shining",
                Author = "Stephen King",
                ISBN = "978-0307743657",
                Publisher = "Random House US",
                PublicationDate = new DateTime(2012, 1, 1),
                Description = "Description 2... ",
                Genre = "Novel"
            };
            addBookViewModel.Save.Execute(null);

            BooksViewModel booksViewModel = new BooksViewModel(context, _dialogService);
            bool hasData = booksViewModel.Books.Where(x => x.ISBN=="978-0307743657").Any();
            Assert.IsTrue(hasData);
        }
    }
}
