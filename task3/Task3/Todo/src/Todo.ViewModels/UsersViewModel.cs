using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Todo.Data;
using Todo.Interfaces;
using Todo.Models;

namespace Todo.ViewModels
{
    public class UsersViewModel : ViewModelBase
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

        private ObservableCollection<User>? _users = null;
        public ObservableCollection<User>? Users
        {
            get
            {
                if (_users is null)
                {
                    _users = new ObservableCollection<User>();
                    return _users;
                }
                return _users;
            }
            set
            {
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        private ICommand? _add = null;
        public ICommand? Add
        {
            get
            {
                if (_add is null)
                {
                    _add = new RelayCommand<object>(AddNewUser);
                }
                return _add;
            }
        }

        private void AddNewUser(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.UsersSubView = new AddUserViewModel(_context, _dialogService);
            }
        }

        private ICommand? _edit = null;
        public ICommand? Edit
        {
            get
            {
                if (_edit is null)
                {
                    _edit = new RelayCommand<object>(EditUser);
                }
                return _edit;
            }
        }

        private void EditUser(object? obj)
        {
            if (obj is not null)
            {
                int userId = (int)obj;
                EditUserViewModel editUserViewModel = new EditUserViewModel(_context, _dialogService)
                {
                    UserId = userId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.UsersSubView = editUserViewModel;
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
                    _remove = new RelayCommand<object>(RemoveUser);
                }
                return _remove;
            }
        }

        private void RemoveUser(object? obj)
        {
            if (obj is not null)
            {
                int userId = (int)obj;
                User? user = _context.Users.Find(userId);
                if (user is not null)
                {
                    DialogResult = _dialogService.Show(user.FirstName + " " + user.LastName);
                    if (DialogResult == false)
                    {
                        return;
                    }

                    _context.Users.Remove(user);
                    _context.SaveChanges();
                }
            }
        }

        public UsersViewModel(TodoContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Users.Load();
            Users = _context.Users.Local.ToObservableCollection();
        }
    }
}