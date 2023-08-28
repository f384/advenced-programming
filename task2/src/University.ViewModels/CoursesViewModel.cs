using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;
using University.Services;

namespace University.ViewModels;

public class CoursesViewModel : ViewModelBase
{
    private readonly IDataAccessService _dataAccessService;
    private readonly IDialogService _dialogService;

    private bool? dialogResult;
    public bool? DialogResult
    {
        get
        {
            return dialogResult;
        }
        set
        {
            dialogResult = value;
        }
    }

    private ObservableCollection<Course>? courses = null;
    public ObservableCollection<Course>? Courses
    {
        get
        {
            if (courses is null)
            {
                courses = new ObservableCollection<Course>();
                return courses;
            }
            return courses;
        }
        set
        {
            courses = value;
            OnPropertyChanged(nameof(Courses));
        }
    }

    private ICommand? _add = null;
    public ICommand? Add
    {
        get
        {
            if (_add is null)
            {
                _add = new RelayCommand<object>(AddNewCourse);
            }
            return _add;
        }
    }

    private void AddNewCourse(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.CoursesSubView = new AddCourseViewModel(_dataAccessService, _dialogService);
        }
    }

    private ICommand? _edit;
    public ICommand? Edit
    {
        get
        {
            if (_edit is null)
            {
                _edit = new RelayCommand<object>(EditCourse);
            }
            return _edit;
        }
    }

    private void EditCourse(object? obj)
    {
        if (obj is not null)
        {
            string courseCode = (string)obj;
            EditCourseViewModel editCourseViewModel = new EditCourseViewModel(_dataAccessService, _dialogService)
            {
                CourseCode = courseCode
            };
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.CoursesSubView = editCourseViewModel;
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
                _remove = new RelayCommand<object>(RemoveCourse);
            }
            return _remove;
        }
    }

    private void RemoveCourse(object? obj)
    {
        if (obj is not null)
        {
            string courseCode = (string)obj;
            Course? course = _dataAccessService.FindEntity<Course>(courseCode);
            if (course is not null)
            {
                DialogResult = _dialogService.Show(course.Title);
                if (DialogResult == false)
                {
                    return;
                }

                _dataAccessService.RemoveEntity(course);
            }
        }
    }

    public CoursesViewModel(IDataAccessService dataAccessService, IDialogService dialogService)
    {
        _dataAccessService = dataAccessService;
        _dialogService = dialogService;

        Courses = _dataAccessService.GetEntities<Course>();
    }
}