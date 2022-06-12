using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Resources;
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

        public CategoryViewModel(Category category)
        {
            this.category = category;
            Refresh();
            this.category.ProductListUpdated += Category_ProductListUpdated;
            activated = false;
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
            }
            
        }

        public event EventHandler TitleClicked;
        
        public ICommand TitleButton
        {
            get => new CommandBase(() => TitleClicked?.Invoke(this, EventArgs.Empty));
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
            get => (Activated ? DarkTheme.LinearMain : DarkTheme.SolidMain);
        }
            
    }
}
