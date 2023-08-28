using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Todo.Data;
using Todo.Interfaces;
using Todo.Models;

namespace Todo.ViewModels
{
    public class CategoriesViewModel : ViewModelBase
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

        private ObservableCollection<Category>? _categories = null;
        public ObservableCollection<Category>? Categories
        {
            get
            {
                if (_categories is null)
                {
                    _categories = new ObservableCollection<Category>();
                    return _categories;
                }
                return _categories;
            }
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        private ICommand? _add = null;
        public ICommand? Add
        {
            get
            {
                if (_add is null)
                {
                    _add = new RelayCommand<object>(AddNewCategory);
                }
                return _add;
            }
        }

        private void AddNewCategory(object? obj)
        {
            var instance = MainWindowViewModel.Instance();
            if (instance is not null)
            {
                instance.CategoriesSubView = new AddCategoryViewModel(_context, _dialogService);
            }
        }

        private ICommand? _edit = null;
        public ICommand? Edit
        {
            get
            {
                if (_edit is null)
                {
                    _edit = new RelayCommand<object>(EditCategory);
                }
                return _edit;
            }
        }

        private void EditCategory(object? obj)
        {
            if (obj is not null)
            {
                int categoryId = (int)obj;
                EditCategoryViewModel editCategoryViewModel = new EditCategoryViewModel(_context, _dialogService)
                {
                    CategoryId = categoryId
                };
                var instance = MainWindowViewModel.Instance();
                if (instance is not null)
                {
                    instance.CategoriesSubView = editCategoryViewModel;
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
                    _remove = new RelayCommand<object>(RemoveCategory);
                }
                return _remove;
            }
        }

        private void RemoveCategory(object? obj)
        {
            if (obj is not null)
            {
                int categoryId = (int)obj;
                Category? category = _context.Categories.Find(categoryId);
                if (category is not null)
                {
                    DialogResult = _dialogService.Show(category.Name);
                    if (DialogResult == false)
                    {
                        return;
                    }

                    _context.Categories.Remove(category);
                    _context.SaveChanges();
                }
            }
        }

        public CategoriesViewModel(TodoContext context, IDialogService dialogService)
        {
            _context = context;
            _dialogService = dialogService;

            _context.Database.EnsureCreated();
            _context.Categories.Load();
            Categories = _context.Categories.Local.ToObservableCollection();
        }
    }
}