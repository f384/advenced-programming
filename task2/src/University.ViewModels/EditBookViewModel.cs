using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Input;
using University.Interfaces;
using University.Models;
using University.Services;

namespace University.ViewModels;

public class EditBookViewModel : ViewModelBase, IDataErrorInfo
{
    private readonly IDialogService _dialogService;
    private readonly IDataAccessService _dataAccessService;
    private Book? _book = new Book();

    public string Error
    {
        get { return string.Empty; }
    }

    public string this[string columnName]
    {
        get
        {
            if (columnName == "Title")
            {
                if (string.IsNullOrEmpty(Title))
                {
                    return "Title is Required";
                }
            }
            if (columnName == "BookId")
            {
                if (string.IsNullOrEmpty(BookId))
                {
                    return "BookId is Required";
                }
            }
            if (columnName == "Auhor")
            {
                if (string.IsNullOrEmpty(Author))
                {
                    return "Author is Required";
                }
            }
            if (columnName == "Publisher")
            {
                if (string.IsNullOrEmpty(Publisher))
                {
                    return "Publisher is Required";
                }
            }
            if (columnName == "ISBN")
            {
                if (string.IsNullOrEmpty(ISBN))
                {
                    return "ISBN is Required";
                }
            }
            if (columnName == "Genre")
            {
                if (string.IsNullOrEmpty(Genre))
                {
                    return "Genre is Required";
                }
            }
            if (columnName == "Description")
            {
                if (string.IsNullOrEmpty(Description))
                {
                    return "Description is Required";
                }
            }
            if (columnName == "PublicationDate")
            {
                if (PublicationDate is null)
                {
                    return "Publication Date is Required";
                }
            }

            return string.Empty;
        }
    }

    private string _title = string.Empty;
    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    private string _bookId = string.Empty;
    public string BookId
    {
        get
        {
            return _bookId;
        }
        set
        {
            _bookId = value;
            OnPropertyChanged(nameof(BookId));
            LoadBookData();
        }
    }

    private string _author = string.Empty;
    public string Author
    {
        get
        {
            return _author;
        }
        set
        {
            _author = value;
            OnPropertyChanged(nameof(Author));
        }
    }

    private string _publisher = string.Empty;
    public string Publisher
    {
        get
        {
            return _publisher;
        }
        set
        {
            _publisher = value;
            OnPropertyChanged(nameof(Publisher));
        }
    }

    private string _isbn = string.Empty;
    public string ISBN
    {
        get
        {
            return _isbn;
        }
        set
        {
            _isbn = value;
            OnPropertyChanged(nameof(ISBN));
        }
    }

    private string _genre = string.Empty;
    public string Genre
    {
        get
        {
            return _genre;
        }
        set
        {
            _genre = value;
            OnPropertyChanged(nameof(Genre));
        }
    }

    private string _description = string.Empty;
    public string Description
    {
        get
        {
            return _description;
        }
        set
        {
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    private DateTime? _publicationDate = null;
    public DateTime? PublicationDate
    {
        get
        {
            return _publicationDate;
        }
        set
        {
            _publicationDate = value;
            OnPropertyChanged(nameof(PublicationDate));
        }
    }

    private string _response = string.Empty;
    public string Response
    {
        get
        {
            return _response;
        }
        set
        {
            _response = value;
            OnPropertyChanged(nameof(Response));
        }
    }

    private ICommand? _back = null;
    public ICommand Back
    {
        get
        {
            if (_back is null)
            {
                _back = new RelayCommand<object>(NavigateBack);
            }
            return _back;
        }
    }

    private void NavigateBack(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.BooksSubView = new BooksViewModel(_dialogService, _dataAccessService);
        }
    }

    private ICommand? _save = null;
    public ICommand Save
    {
        get
        {
            if (_save is null)
            {
                _save = new RelayCommand<object>(SaveData);
            }
            return _save;
        }
    }

    private void SaveData(object? obj)
    {
        if (!IsValid())
        {
            Response = "Please complete all required fields";
            return;
        }

        if (_book is null)
        {
            return;
        }

        _book.Title = Title;
        _book.BookId = BookId;
        _book.Author = Author;
        _book.Publisher = Publisher;
        _book.PublicationDate = PublicationDate;
        _book.ISBN = ISBN;
        _book.Genre = Genre;
        _book.Description = Description;

        _dataAccessService.EditEntity(_book);

        Response = "Data Updated";
    }

    public EditBookViewModel(IDialogService dialogService, IDataAccessService dataAccessService)
    {
        _dialogService = dialogService;
        _dataAccessService = dataAccessService;
    }

    private bool IsValid()
    {
        string[] properties = { "BookId", "Title", "Author", "Publisher", "Description", "PublicationDate", "ISBN", "Genre" };
        foreach (string property in properties)
        {
            if (!string.IsNullOrEmpty(this[property]))
            {
                return false;
            }
        }
        return true;
    }

    private void LoadBookData()
    {
        if (_dataAccessService.GetEntities<Book>() is null)
        {
            return;
        }
        _book = _dataAccessService.FindEntity<Book>(BookId);
        if (_book is null)
        {
            return;
        }
        this.Title = _book.Title;
        this.Author = _book.Author;
        this.Publisher = _book.Publisher;
        this.PublicationDate = _book.PublicationDate;
        this.ISBN = _book.ISBN;
        this.Genre = _book.Genre;
        this.Description = _book.Description;
    }
}