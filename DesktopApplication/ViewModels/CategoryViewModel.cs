using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class CategoryViewModel : ViewModelBase
    {
        private Category category;

        public CategoryViewModel(Category category)
        {
            this.category = category;
            this.category.ProductListUpdated += Category_ProductListUpdated;
            Refresh();
        }

        private void Category_ProductListUpdated(object sender, EventArgs e)
        {
            //Refresh();
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
            Products = new ObservableCollection<ProductViewModel>(category.Products.Select(p=>new ProductViewModel(p)));
        }
            
    }
}
