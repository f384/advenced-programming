using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using University.Interfaces;
using University.Models;
using University.Services;

namespace University.ViewModels;

public class ExamsViewModel : ViewModelBase
{
    private readonly IDataAccessService _dataAccessService;
    private readonly IDialogService _dialogService;

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

    private ObservableCollection<Exam>? _exams = null;
    public ObservableCollection<Exam>? Exams
    {
        get
        {
            if (_exams is null)
            {
                _exams = new ObservableCollection<Exam>();
                return _exams;
            }
            return _exams;
        }
        set
        {
            _exams = value;
            OnPropertyChanged(nameof(Exams));
        }
    }

    private ICommand? _add = null;
    public ICommand? Add
    {
        get
        {
            if (_add is null)
            {
                _add = new RelayCommand<object>(AddNewExam);
            }
            return _add;
        }
    }

    private void AddNewExam(object? obj)
    {
        var instance = MainWindowViewModel.Instance();
        if (instance is not null)
        {
            instance.ExamsSubView = new AddExamViewModel(_dataAccessService, _dialogService);
        }
    }

    private ICommand? _edit = null;
    public ICommand? Edit
    {
        get
        {
            if (_edit is null)
            {
                _edit = new RelayCommand<object>(EditExam);
            }
            return _edit;
        }
    }

    private void EditExam(object? obj)
    {
        if (obj is not null)
        {
            string examId = (string)obj;
            EditExamViewModel editExamViewModel = new EditExamViewModel(_dataAccessService, _dialogService)
            {
                ExamId = examId
            };
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.ExamsSubView = editExamViewModel;
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
                _remove = new RelayCommand<object>(RemoveExam);
            }
            return _remove;
        }
    }

    private void RemoveExam(object? obj)
    {
        if (obj is not null)
        {
            string examId = (string)obj;
            Exam? exam = _dataAccessService.FindEntity<Exam>(examId);
            if (exam is not null)
            {
                DialogResult = _dialogService.Show(exam.CourseCode + " " + exam.Date.ToString());
                if (DialogResult == false)
                {
                    return;
                }

                _dataAccessService.RemoveEntity(exam);
            }
        }
    }

    public ExamsViewModel(IDataAccessService dataAccessService, IDialogService dialogService)
    {
        _dataAccessService = dataAccessService;
        _dialogService = dialogService;

        Exams = _dataAccessService.GetEntities<Exam>();
    }
}
