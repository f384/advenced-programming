using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Data;
using University.Interfaces;
using University.Models;
using University.Services;

namespace University.ViewModels;

public class SearchViewModel : ViewModelBase
{
    private readonly IDataAccessService _dataAccessService;
    private readonly IDialogService _dialogService;
    private readonly IValidationService _validationService;

    private bool? _dialogResult = null;
    public bool? DialogResult
    {
        get
        {
            return _dialogResult;
        }
        set
        {
            _dialogResult = value;
        }
    }

    private string _firstCondition = string.Empty;
    public string FirstCondition
    {
        get
        {
            return _firstCondition;
        }
        set
        {
            _firstCondition = value;
            OnPropertyChanged(nameof(FirstCondition));
        }
    }

    private string _secondCondition = string.Empty;
    public string SecondCondition
    {
        get
        {
            return _secondCondition;
        }
        set
        {
            _secondCondition = value;
            OnPropertyChanged(nameof(SecondCondition));
        }
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get
        {
            return _isVisible;
        }
        set
        {
            _isVisible = value;
            OnPropertyChanged(nameof(IsVisible));
        }
    }

    private bool _areStudentsVisible;
    public bool AreStudentsVisible
    {
        get
        {
            return _areStudentsVisible;
        }
        set
        {
            _areStudentsVisible = value;
            OnPropertyChanged(nameof(AreStudentsVisible));
        }
    }

    private bool _areCoursesVisible;
    public bool AreCoursesVisible
    {
        get
        {
            return _areCoursesVisible;
        }
        set
        {
            _areCoursesVisible = value;
            OnPropertyChanged(nameof(AreCoursesVisible));
        }
    }

    private bool _areBooksVisible;
    public bool AreBooksVisible
    {
        get
        {
            return _areBooksVisible;
        }
        set
        {
            _areBooksVisible = value;
            OnPropertyChanged(nameof(AreBooksVisible));
        }
    }

    private ObservableCollection<Student>? _students = null;
    public ObservableCollection<Student>? Students
    {
        get
        {
            if (_students is null)
            {
                _students = new ObservableCollection<Student>();
                return _students;
            }
            return _students;
        }
        set
        {
            _students = value;
            OnPropertyChanged(nameof(Students));
        }
    }

    private ObservableCollection<Book>? _books = null;
    public ObservableCollection<Book>? Books
    {
        get
        {
            if (_books is null)
            {
                _books = new ObservableCollection<Book>();
                return _books;
            }
            return _books;
        }
        set
        {
            _books = value;
            OnPropertyChanged(nameof(Books));
        }
    }

    private ObservableCollection<Course>? _courses = null;
    public ObservableCollection<Course>? Courses
    {
        get
        {
            if (_courses is null)
            {
                _courses = new ObservableCollection<Course>();
                return _courses;
            }
            return _courses;
        }
        set
        {
            _courses = value;
            OnPropertyChanged(nameof(Courses));
        }
    }

    private ICommand? _comboBoxSelectionChanged = null;
    public ICommand? ComboBoxSelectionChanged
    {
        get
        {
            if (_comboBoxSelectionChanged is null)
            {
                _comboBoxSelectionChanged = new RelayCommand<object>(UpdateCondition);
            }
            return _comboBoxSelectionChanged;
        }
    }

    private void UpdateCondition(object? obj)
    {
        if (obj is string objAsString)
        {
            IsVisible = true;
            string selectedValue = objAsString;
            SecondCondition = string.Empty;
            if (selectedValue == "Students")
            {
                FirstCondition = "who attends";
            }
            else if (selectedValue == "Courses")
            {
                FirstCondition = "attended by Student with PESEL";
            }
            else if (selectedValue == "Books")
            {
                FirstCondition = "with ISBN";
            }
        }
    }

    private ICommand? _search = null;
    public ICommand? Search
    {
        get
        {
            if (_search is null)
            {
                _search = new RelayCommand<object>(SelectData);
            }
            return _search;
        }
    }

    private void SelectData(object? obj)
    {
        if (FirstCondition == "who attends")
        {
            Course? course = _dataAccessService.GetEntities<Course>().Where(s => s.Title == SecondCondition).FirstOrDefault();
            if (course is not null)
            {
                var students = _dataAccessService.GetEntities<Student>()
                    .Where(s => s.Courses != null && s.Courses.Contains(course))
                    .ToList();

                var filteredStudents = students
                    .Where(s => s.Courses != null && s.Courses.Any(sub => sub.Title == course.Title))
                    .ToList();

                Students = new ObservableCollection<Student>(filteredStudents);
                AreCoursesVisible = false;
                AreStudentsVisible = true;
                AreBooksVisible = false;
            }
        }
        else if (FirstCondition == "with ISBN")
        {
            Books = new ObservableCollection<Book>(_dataAccessService.GetEntities<Book>().Where(x => x.ISBN == SecondCondition).ToList());

            AreCoursesVisible = false;
            AreStudentsVisible = false;
            AreBooksVisible = true;
        }
    }

    private ICommand? _edit = null;
    public ICommand? Edit
    {
        get
        {
            if (_edit is null)
            {
                _edit = new RelayCommand<object>(EditItem);
            }
            return _edit;
        }
    }

    private void EditItem(object? obj)
    {
        if (obj is not null)
        {
            if (FirstCondition == "who attends")
            {
                string studentId = (string)obj;
                EditStudentViewModel editStudentViewModel = new EditStudentViewModel(_dataAccessService, _dialogService, _validationService)
                {
                    StudentId = studentId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.StudentsSubView = editStudentViewModel;
                    instance.SelectedTab = 0;
                }
            }
            else if (FirstCondition == "attended by Student with PESEL")
            {
                string courseCode = (string)obj;
                EditCourseViewModel editSubjectViewModel = new EditCourseViewModel(_dataAccessService, _dialogService)
                {
                    CourseCode = courseCode
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.CoursesSubView = editSubjectViewModel;
                    instance.SelectedTab = 1;
                }
            }
        }
    }

    private ICommand ?_remove = null;
    public ICommand? Remove
    {
        get
        {
            if (_remove is null)
            {
                _remove = new RelayCommand<object>(RemoveItem);
            }
            return _remove;
        }
    }

    private void RemoveItem(object? obj)
    {
        if (obj is not null)
        {
            if (FirstCondition == "who attends")
            {
                string studentId = (string)obj;
                Student? student = _dataAccessService.FindEntity<Student>(studentId);
                if (student is null)
                {
                    return;
                }

                DialogResult = _dialogService.Show(student.Name + " " + student.LastName);
                if (DialogResult == false)
                {
                    return;
                }
                _dataAccessService.RemoveEntity(student);
            }
            else if (FirstCondition == "attended by Student with PESEL")
            {
                string courseCode = (string)obj;
                Course? course = _dataAccessService.FindEntity<Course>(courseCode);
                if (course is null)
                {
                    return;
                }
                DialogResult = _dialogService.Show(course.Title);
                if (DialogResult == false)
                {
                    return;
                }
                _dataAccessService.RemoveEntity(course);
            }
        }
    }

    public SearchViewModel(IDataAccessService dataAccessService, IDialogService dialogService, IValidationService validationService)
    {
        _dataAccessService = dataAccessService;
        _dialogService = dialogService;
        _validationService = validationService;

        IsVisible = false;
        AreStudentsVisible = false;
        AreCoursesVisible = false;
    }
}
