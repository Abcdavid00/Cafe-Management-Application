using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Resources;
using CSWBManagementApplication.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class ProductViewModel : ViewModelBase
    {        
        private Product product;
        public Product Product
        {
            get => product;
            set
            {
                product = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(SPrice));
                OnPropertyChanged(nameof(MPrice));
                OnPropertyChanged(nameof(LPrice));
            }
        }
        
        private bool inCategory
        {
            get => !string.IsNullOrEmpty(product.CategoryID);
        }

        public ProductViewModel(Product product)
        {
            this.Product = product;
            this.Product.ProductInfoChanged += OnProductInfoChanged;
            activated = false;
        }

        private void OnProductInfoChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(SPrice));
            OnPropertyChanged(nameof(MPrice));
            OnPropertyChanged(nameof(LPrice));
        }

        public string Name
        {
            get => product.Name;
        }

        public string SPrice
        {
            get => MiscFunctions.IntToPrice(product.SPrice);
        }

        public string MPrice
        {
            get => MiscFunctions.IntToPrice(product.MPrice);
        }
        
        public string LPrice
        {
            get => MiscFunctions.IntToPrice(product.LPrice);
        }

        public int NameColumnSpan
        {
            get => inCategory ? 1 : 4;
        }

        public Visibility PriceVisibility
        {
            get => inCategory ? Visibility.Visible : Visibility.Collapsed;
        }

        public Visibility AddButtonVisibility
        {
            get => inCategory ? Visibility.Collapsed : Visibility.Visible;
        }

        public Visibility RemoveButtonVisibility
        {
            get => inCategory ? Visibility.Visible : Visibility.Collapsed;
        }

        public event EventHandler<Product> OnAddButtonClicked;

        public ICommand Add
        {
            get => new CommandBase(() =>
            {
                OnAddButtonClicked?.Invoke(this, product);
            });
        }

        public event EventHandler<Product> OnRemoveButtonClicked;

        public ICommand Remove
        {
            get => new CommandBase(() =>
            {
                OnRemoveButtonClicked?.Invoke(this, product);
            });
        }

        public event EventHandler<Product> OnEditButtonClicked;

        public ICommand Edit
        {
            get => new CommandBase(() =>
            {
                OnEditButtonClicked?.Invoke(this, product);
            });
        }

        private bool activated;
        public bool Activated
        {
            get => activated;
            set
            {
                activated = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Background));
            }
        }

        public Brush Background
        {
            get => activated ? DarkTheme.LinearMain : DarkTheme.SolidLight;
        }
        
        public Brush ButtonBackground
        {
            get => activated ? DarkTheme.SolidDark : DarkTheme.SolidMain;
        }
    }
}
