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
    public class EditUserViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly TodoContext _context;
        private readonly IDialogService _dialogService;
        private User? _user = new User();

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "UserId")
                {
                    if (UserId <= 0)
                    {
                        return "UserId is Required";
                    }
                }
                if (columnName == "FirstName")
                {
                    if (string.IsNullOrEmpty(FirstName))
                    {
                        return "First name is Required";
                    }
                }
                if (columnName == "LastName")
                {
                    if (string.IsNullOrEmpty(LastName))
                    {
                        return "Last name is Required";
                    }
                }
                return string.Empty;
            }
        }

        private int _userId = 0;
        public int UserId
        {
            get
            {
                return _userId;
            }
            set
            {
                _userId = value;
                OnPropertyChanged(nameof(UserId));
                LoadUserData();
            }
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
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

        private ObservableCollection<Task>? _tasks = null;
        public ObservableCollection<Task>? Tasks
        {
            get
            {
                return _tasks;
            }
            set
            {
                _tasks = value;
                OnPropertyChanged(nameof(Tasks));
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
                instance.UsersSubView = new UsersViewModel(_context, _dialogService);
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

            if (_user is null)
            {
                return;
            }
            _user.UserId = UserId;
            _user.FirstName = FirstName;
            _user.LastName = LastName;

            _context.Entry(_user).State = EntityState.Modified;
            _context.SaveChanges();

            Response = "Data Updated";
        }

        public EditUserViewModel(TodoContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;
        }

        private ObservableCollection<Task> LoadTasks()
        {
            _context.Database.EnsureCreated();
            _context.Tasks.Load();
            return new ObservableCollection<Task>(_context.Tasks.Local.Where(x => x.User.UserId == UserId));
        }

        private bool IsValid()
        {
            string[] properties = { "UserId", "FirstName", "LastName" };
            foreach (string property in properties)
            {
                if (!string.IsNullOrEmpty(this[property]))
                {
                    return false;
                }
            }
            return true;
        }

        private void LoadUserData()
        {
            if (_context?.Users is null)
            {
                return;
            }
            _context.Users.Load();
            _user = _context.Users.Find(UserId);
            if (_user is null)
            {
                return;
            }
            this.FirstName = _user.FirstName;
            this.LastName = _user.LastName;
            Tasks = LoadTasks();
        }
    }
}