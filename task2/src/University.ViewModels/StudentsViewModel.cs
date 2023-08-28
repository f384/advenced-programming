using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;
using University.Services;

namespace University.ViewModels;

public class StudentsViewModel : ViewModelBase
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

    private ICommand? _add = null;
    public ICommand? Add
    {
        get
        {
            if (_add is null)
            {
                _add = new RelayCommand<object>(AddNewStudent);
            }
            return _add;
        }
    }

    private void AddNewStudent(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.StudentsSubView = new AddStudentViewModel(_dataAccessService, _dialogService, _validationService);
        }
    }

    private ICommand? _edit = null;
    public ICommand? Edit
    {
        get
        {
            if (_edit is null)
            {
                _edit = new RelayCommand<object>(EditStudent);
            }
            return _edit;
        }
    }

    private void EditStudent(object? obj)
    {
        if (obj is not null)
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
            }
        }
    }

    private ICommand? _remove = null;
    public ICommand? Remove
    {
        get
        {
            if (_remove is null)
            {
                _remove = new RelayCommand<object>(RemoveStudent);
            }
            return _remove;
        }
    }

    private void RemoveStudent(object? obj)
    {
        if (obj is not null)
        {
            string studentId = (string)obj;
            Student? student = _dataAccessService.FindEntity<Student>(studentId);
            if (student is not null)
            {
                DialogResult = _dialogService.Show(student.Name + " " + student.LastName);
                if (DialogResult == false)
                {
                    return;
                }

                _dataAccessService.RemoveEntity(student);
            }
        }
    }

    public StudentsViewModel(IDataAccessService dataAccessService, IDialogService dialogService, IValidationService validationService)
    {
        _dataAccessService = dataAccessService;
        _dialogService = dialogService;
        _validationService = validationService;

        Students = _dataAccessService.GetEntities<Student>();        
    }
}
