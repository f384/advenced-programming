using System;
using University.Interfaces;
using University.Services;

namespace University.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDataAccessService _dataAccessService;
    private readonly IDialogService _dialogService;
    private readonly IValidationService _validationService;

    private int _selectedTab;
    public int SelectedTab
    {
        get
        {
            return _selectedTab;
        }
        set
        {
            _selectedTab = value;
            OnPropertyChanged(nameof(SelectedTab));
        }
    }

    private object? _studentsSubView = null;
    public object? StudentsSubView
    {
        get
        {
            return _studentsSubView;
        }
        set
        {
            _studentsSubView = value;
            OnPropertyChanged(nameof(StudentsSubView));
        }
    }

    private object? _coursesSubView = null;
    public object? CoursesSubView
    {
        get
        {
            return _coursesSubView;
        }
        set
        {
            _coursesSubView = value;
            OnPropertyChanged(nameof(CoursesSubView));
        }
    }
    
    private object? _booksSubView = null;
    public object? BooksSubView
    {
        get
        {
            return _booksSubView;
        }
        set
        {
            _booksSubView = value;
            OnPropertyChanged(nameof(BooksSubView));
        }
    } 
    
    private object? _examsSubView = null;
    public object? ExamsSubView
    {
        get
        {
            return _examsSubView;
        }
        set
        {
            _examsSubView = value;
            OnPropertyChanged(nameof(ExamsSubView));
        }
    }

    private object? _searchSubView = null;
    public object? SearchSubView
    {
        get
        {
            return _searchSubView;
        }
        set
        {
            _searchSubView = value;
            OnPropertyChanged(nameof(SearchSubView));
        }
    }

    private static MainWindowViewModel? _instance = null;
    public static MainWindowViewModel? Instance()
    {
        return _instance;
    }

    public MainWindowViewModel(IDataAccessService dataAccessService, IDialogService dialogService, IValidationService validationService)
    {
        _dataAccessService = dataAccessService;
        _dialogService = dialogService;
        _validationService = validationService;

        if (_instance is null)
        {
            _instance = this;
        }

        StudentsSubView = new StudentsViewModel(_dataAccessService, _dialogService, _validationService);
        CoursesSubView = new CoursesViewModel(_dataAccessService, _dialogService);
        BooksSubView = new BooksViewModel(_dialogService, _dataAccessService);
        ExamsSubView = new ExamsViewModel(_dataAccessService, _dialogService);
        SearchSubView = new SearchViewModel(_dataAccessService, _dialogService, _validationService);
    }
}
