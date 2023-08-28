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
    private IDataAccessService _dataAccessService;
    private DbContextOptions<UniversityContext> _options;

    [TestInitialize()]
    public void Initialize()
    {
        _options = new DbContextOptionsBuilder<UniversityContext>()
            .UseInMemoryDatabase(databaseName: "UniversityTestDB")
            .Options;
        SeedTestDB();
        _dialogService = new DialogService();
        _dataAccessService = new DataAccessService(new UniversityContext(_options));
    }

    private void SeedTestDB()
    {
        DataAccessService dataAccessService = new(new UniversityContext(_options));
        dataAccessService.EnsureDeleted();

        List<Book> books = new List<Book>
        {
                new Book { BookId = "B0001", Title = "Brave New World", Author = "Aldous Huxley", ISBN = "978-0060850524", Publisher = "Harper Perennial", PublicationDate = new DateTime(2006, 11, 26), Description = "Description... ", Genre = "Novel" }
        };

        foreach (Book book in books)
        {
            dataAccessService.AddEntity(book);
        }
    }


    [TestMethod]
    public void Show_all_books()
    {
        BooksViewModel booksViewModel = new BooksViewModel(_dialogService, _dataAccessService);
        bool hasData = booksViewModel.Books.Any();
        Assert.IsTrue(hasData);
    }

    [TestMethod]
    public void AddBook()
    {
        AddBookViewModel addBookViewModel = new AddBookViewModel(_dialogService, _dataAccessService)
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

        BooksViewModel booksViewModel = new BooksViewModel(_dialogService, _dataAccessService);
        bool hasData = booksViewModel.Books.Where(x => x.ISBN == "978-0307743657").Any();
        Assert.IsTrue(hasData);
    }
}
