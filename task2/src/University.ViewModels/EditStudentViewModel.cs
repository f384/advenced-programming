using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;
using University.Services;

namespace University.ViewModels;

public class EditStudentViewModel : ViewModelBase, IDataErrorInfo
{
    private readonly IDataAccessService _dataAccessService;
    private readonly IDialogService _dialogService;
    private readonly IValidationService _validationService;
    private Student? _student = new Student();

    public string Error
    {
        get { return string.Empty; }
    }

    public string this[string columnName]
    {
        get
        {
            if (columnName == "Name")
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return "Name is Required";
                }
            }
            if (columnName == "LastName")
            {
                if (string.IsNullOrEmpty(LastName))
                {
                    return "Last Name is Required";
                }
            }
            if (columnName == "PESEL")
            {
                if (string.IsNullOrEmpty(PESEL))
                {
                    return "PESEL is Required";
                }
                if (!_validationService.ValidatePESEL(PESEL))
                {
                    return "PESEL is Invalid";
                }
            }
            if (columnName == "BirthDate")
            {
                if (_validationService.ValidateBirthDate(BirthDate))
                {
                    return "BirthDate is Required";
                }
            }
            if (columnName == "BirthPlace")
            {
                if (string.IsNullOrEmpty(BirthPlace))
                {
                    return "Birth Place is Required";
                }
            }
            if (columnName == "ResidencePlace")
            {
                if (string.IsNullOrEmpty(ResidencePlace))
                {
                    return "Residence Place is Required";
                }
            }
            if (columnName == "AddressLine1")
            {
                if (string.IsNullOrEmpty(AddressLine1))
                {
                    return "Address Line 1 is Required";
                }
            }
            return string.Empty;
        }
    }

    private string _name = string.Empty;
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private string _lastName = string.Empty;
    public string LastName
    {
        get
        {
            return _lastName;
        }
        set
        {
            _lastName = value;
            OnPropertyChanged(nameof(LastName));
        }
    }

    private string _pesel = string.Empty;
    public string PESEL
    {
        get
        {
            return _pesel;
        }
        set
        {
            _pesel = value;
            OnPropertyChanged(nameof(PESEL));
        }
    }

    private DateTime? birthDate = null;
    public DateTime? BirthDate
    {
        get
        {
            return birthDate;
        }
        set
        {
            birthDate = value;
            OnPropertyChanged(nameof(BirthDate));
        }
    }

private string _birthPlace = string.Empty;
public string BirthPlace
{
    get
    {
        return _birthPlace;
    }
    set
    {
        _birthPlace = value;
        OnPropertyChanged(nameof(BirthPlace));
    }
}

private string _residencePlace = string.Empty;
public string ResidencePlace
{
    get
    {
        return _residencePlace;
    }
    set
    {
        _residencePlace = value;
        OnPropertyChanged(nameof(ResidencePlace));
    }
}

private string _addressLine1 = string.Empty;
public string AddressLine1
{
    get
    {
        return _addressLine1;
    }
    set
    {
        _addressLine1 = value;
        OnPropertyChanged(nameof(AddressLine1));
    }
}

private string _addressLine2 = string.Empty;
public string AddressLine2
{
    get
    {
        return _addressLine2;
    }
    set
    {
        _addressLine2 = value;
        OnPropertyChanged(nameof(AddressLine2));
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

    private string _studentId = string.Empty;
    public string StudentId
    {
        get
        {
            return _studentId;
        }
        set
        {
            _studentId = value;
            OnPropertyChanged(nameof(StudentId));
            LoadStudentData();
        }
    }

    private ObservableCollection<Course>? _assignedCourses = null;
    public ObservableCollection<Course> AssignedCourses
    {
        get
        {
            if (_assignedCourses is null)
            {
                _assignedCourses = LoadCourses();
                return _assignedCourses;
            }
            return _assignedCourses;
        }
        set
        {
            _assignedCourses = value;
            OnPropertyChanged(nameof(AssignedCourses));
        }
    } 
    
    private ObservableCollection<Exam>? _assignedExams = null;
    public ObservableCollection<Exam> AssignedExams
    {
        get
        {
            if (_assignedExams is null)
            {
                _assignedExams = LoadExams();
                return _assignedExams;
            }
            return _assignedExams;
        }
        set
        {
            _assignedExams = value;
            OnPropertyChanged(nameof(AssignedExams));
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
            instance.StudentsSubView = new StudentsViewModel(_dataAccessService, _dialogService, _validationService);
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

        if (_student is null)
        {
            return;
        }
        _student.Name = Name;
        _student.LastName = LastName;
        _student.PESEL = PESEL;
        _student.BirthDate = BirthDate;
        _student.Courses = AssignedCourses.Where(s => s.IsSelected).ToList();
        _student.Exams = AssignedExams.Where(s => s.IsSelected).ToList();


        _dataAccessService.EditEntity(_student);

        Response = "Data Updated";
    }

    public EditStudentViewModel(IDataAccessService dataAccessService, IDialogService dialogService, IValidationService validationService)
    {
        _dataAccessService = dataAccessService;
        _dialogService = dialogService;
        _validationService = validationService;
    }

    private ObservableCollection<Course> LoadCourses()
    {
        return _dataAccessService.GetEntities<Course>();
    }

    private ObservableCollection<Exam> LoadExams()
    {
        return _dataAccessService.GetEntities<Exam>();
    }

    private bool IsValid()
    {
        string[] properties = { "Name", "LastName", "PESEL", "BirthDay", "BirthPlace", "ResidencePlace", "AddressLine1" };
        foreach (string property in properties)
        {
            if (!string.IsNullOrEmpty(this[property]))
            {
                return false;
            }
        }
        return true;
    }

    private void LoadStudentData()
    {
        if (_dataAccessService.GetEntities<Student>() is null)
        {
            return;
        }
        _student = _dataAccessService.FindEntity<Student>(StudentId);
        if (_student is null)
        {
            return;
        }
        this.Name = _student.Name;
        this.LastName = _student.LastName;
        this.PESEL = _student.PESEL;
        this.BirthDate = _student.BirthDate;
        this.AddressLine1 = _student.AddressLine1;
        this.AddressLine2 = _student.AddressLine2;
        this.ResidencePlace = _student.ResidencePlace;
        this.BirthPlace = _student.BirthPlace;
        if (_student.Courses is null)
        {
            return;
        }
        foreach (Course course in _student.Courses)
        {
            if (course is not null && AssignedCourses is not null)
            {
                var assignedCourse = AssignedCourses
                    .FirstOrDefault(s => s.CourseCode == course.CourseCode);
                if (assignedCourse is not null)
                { 
                    assignedCourse.IsSelected = true;
                }
            }
        }

        if (_student.Exams is null)
        {
            return;
        }
        foreach (Exam exam in _student.Exams)
        {
            if (exam is not null && AssignedExams is not null)
            {
                var assignedExam = AssignedExams
                    .FirstOrDefault(s => s.ExamId == exam.ExamId);
                if (assignedExam is not null)
                {
                    assignedExam.IsSelected = true;
                }
            }
        }
    }
}
