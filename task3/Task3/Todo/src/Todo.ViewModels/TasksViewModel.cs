using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Todo.Data;
using Todo.Interfaces;
using Todo.Models;
using Task = Todo.Models.Task;

namespace Todo.ViewModels
{
    public class TasksViewModel : ViewModelBase
    {
        private readonly TodoContext _context;
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

        private ObservableCollection<Task>? _tasks = null;
        public ObservableCollection<Task>? Tasks
        {
            get
            {
                if (_tasks is null)
                {
                    _tasks = new ObservableCollection<Task>();
                    return _tasks;
                }
                return _tasks;
            }
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
            }
        }

        private ICommand? _add = null;
        public ICommand? Add
        {
            get
            {
                if (_add is null)
                {
                    _add = new RelayCommand<object>(AddNewTask);
                }
                return _add;
            }
        }

        private void AddNewTask(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.TasksSubView = new AddTaskViewModel(_context, _dialogService);
            }
        }

        private ICommand? _edit = null;
        public ICommand? Edit
        {
            get
            {
                if (_edit is null)
                {
                    _edit = new RelayCommand<object>(EditTask);
                }
                return _edit;
            }
        }

        private void EditTask(object? obj)
        {
            if (obj is not null)
            {
                int taskId = (int)obj;
                EditTaskViewModel editTaskViewModel = new EditTaskViewModel(_context, _dialogService)
                {
                    TaskId = taskId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.TasksSubView = editTaskViewModel;
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
                    _remove = new RelayCommand<object>(RemoveTask);
                }
                return _remove;
            }
        }

        private void RemoveTask(object? obj)
        {
            if (obj is not null)
            {
                int taskId = (int)obj;
                Task? task = _context.Tasks.Find(taskId);
                if (task is not null)
                {
                    DialogResult = _dialogService.Show(task.TaskId + " " + task.Name);
                    if (DialogResult == false)
                    {
                        return;
                    }

                    _context.Tasks.Remove(task);
                    _context.SaveChanges();
                }
            }
        }

        public TasksViewModel(TodoContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Tasks.Load();
            Tasks = _context.Tasks.Local.ToObservableCollection();
        }
    }
}