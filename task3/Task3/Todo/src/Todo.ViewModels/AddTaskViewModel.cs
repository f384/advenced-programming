using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Todo.Data;
using Todo.Interfaces;
using Todo.Models;
using Task = Todo.Models.Task;

namespace Todo.ViewModels
{
    public class AddTaskViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly TodoContext _context;
        private readonly IDialogService _dialogService;

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "TaskId")
                {
                    if (TaskId <= 0)
                    {
                        return "TaskId is Required";
                    }
                }
                if (columnName == "Name")
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        return "Name is Required";
                    }
                }
                if (columnName == "Description")
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        return "Description is Required";
                    }
                }
                if (columnName == "StartDate")
                {
                    if (StartDate == null)
                    {
                        return "StartDate is Required";
                    }
                }
                if (columnName == "DueDate")
                {
                    if (DueDate == null)
                    {
                        return "DueDate is Required";
                    }
                }
                if (columnName == "Category")
                {
                    if (CategoryId <= 0)
                    {
                        return "CategoryId is Required";
                    }
                }
                if (columnName == "User")
                {
                    if (CategoryId <= 0)
                    {
                        return "UserId is Required";
                    }
                }

                return string.Empty;
            }
        }

        private int _taskId = 0;
        public int TaskId
        {
            get
            {
                return _taskId;
            }
            set
            {
                _taskId = value;
                OnPropertyChanged(nameof(TaskId));
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

        private Task.Priority _importance = Models.Task.Priority.Low;
        public Task.Priority Importance
        {
            get
            {
                return _importance;
            }
            set
            {
                _importance = value;
                OnPropertyChanged(nameof(Importance));
            }
        }

        private DateTime? _startDate = null;
        public DateTime? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        private DateTime? _dueDate = null;
        public DateTime? DueDate
        {
            get
            {
                return _dueDate;
            }
            set
            {
                _dueDate = value;
                OnPropertyChanged(nameof(DueDate));
            }
        }

        private int _categoryId = 0;
        public int CategoryId
        {
            get { return _categoryId; }
            set
            {
                _categoryId = value;
                OnPropertyChanged(nameof(CategoryId));
            }
        }
        
        private int _userId = 0;
        public int UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
            }
        }

        private ObservableCollection<Category>? _assignedCategories = null;
        public ObservableCollection<Category>? AssignedCategories
        {
            get
            {
                if (_assignedCategories is null)
                {
                    _assignedCategories = LoadCategories();
                    return _assignedCategories;
                }
                return _assignedCategories;
            }
            set
            {
                _assignedCategories = value;
                OnPropertyChanged(nameof(AssignedCategories));
            }
        }

        private bool _isCompleted = false;
        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
            set
            {
                _isCompleted = value;
                OnPropertyChanged(nameof(IsCompleted));
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
        public ICommand? Back
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
                instance.TasksSubView = new TasksViewModel(_context, _dialogService);
            }
        }

        private ICommand? _save = null;
        public ICommand? Save
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

        private ObservableCollection<Category> LoadCategories()
        {
            _context.Database.EnsureCreated();
            _context.Categories.Load();
            return _context.Categories.Local.ToObservableCollection();
        }

        private void SaveData(object? obj)
        {
            if (!IsValid())
            {
                Response = "Please complete all required fields";
                return;
            }

            Task task = new Task
            {
                TaskId = this.TaskId,
                Name = this.Name,
                Description = this.Description,
                Importance = this.Importance,
                StartDate = this.StartDate,
                DueDate = this.DueDate,
                CategoryId = this.CategoryId,
                UserId = this.UserId,
                IsCompleted = this.IsCompleted
            };

            _context.Tasks.Add(task);
            _context.SaveChanges();

            Response = "Data Saved";
        }

        public AddTaskViewModel(TodoContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private bool IsValid()
        {
            string[] properties = { "TaskId", "Name", "Description", "Importance", "StartDate", "DueDate", "CategoryId", "UserId", "IsCompleted" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}