using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Resources;
using CSWBManagementApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class CategoryViewModel : ViewModelBase
    {
        private Category category;
        public Category Category
        {
            get => category;
        }

        public CategoryViewModel(Category category)
        {
            this.category = category;
            
            this.category.ProductListUpdated += Category_ProductListUpdated;
            this.category.CategoryNameUpdated += Category_CategoryNameUpdated;
            activated = false;
            Refresh();
        }

        private void Category_CategoryNameUpdated(object sender, string e)
        {
            OnPropertyChanged(nameof(Name));
        }

        private void Category_ProductListUpdated(object sender, EventArgs e)
        {
            Refresh();
        }

        public string Name { get => category.Name; }

        private ObservableCollection<ProductViewModel> products;
        public ObservableCollection<ProductViewModel> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }        

        public void Refresh()
        {
            Products?.Clear();
            if (category.Products == null )
            {
                Products = new ObservableCollection<ProductViewModel>();
            } else
            {
                Products = new ObservableCollection<ProductViewModel>(category.Products.Select(p => new ProductViewModel(p)));
                foreach (ProductViewModel productViewModel in Products)
                {
                    productViewModel.OnEditButtonClicked += ((object o, Product p) => OnProductEditButtonClicked?.Invoke(this, o as ProductViewModel));
                    productViewModel.OnRemoveButtonClicked += ((object o, Product p)=> Category.RemoveProduct(p));
                }
            }
            OnProductsListChanged?.Invoke(this, EventArgs.Empty);
            if (Products.Count ==0)
            {
                OnProductsListGetEmpty?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler OnProductsListGetEmpty;
        
        public event EventHandler<Category> OnTitleClicked;

        public event EventHandler<ProductViewModel> OnProductEditButtonClicked;

        public event EventHandler OnProductsListChanged;

        public ICommand TitleButton
        {
            get => new CommandBase(() => OnTitleClicked?.Invoke(this, this.Category));
        }       
        
        private bool activated;
        public bool Activated
        {
            get => activated;
            set
            {
                activated = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TitleBrush));
            }
        }
        
        public Brush TitleBrush
        {
            get => (Activated ? DarkTheme.LinearPrimary : DarkTheme.LinearMain);
        }
            
    }
}
