using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class OrderingCategoryViewModel : ViewModelBase
    {
        private Category category;
        
        public OrderingCategoryViewModel(Category category)
        {
            this.category = category;
            this.category.ProductListUpdated += Category_ProductListUpdated;
        }

        public string Name { get => category.Name; }

        private ObservableCollection<OrderingProductViewModel> products;
        public ObservableCollection<OrderingProductViewModel> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<OrderingProductEventArgs> ProductClicked;

        private void Category_ProductListUpdated(object sender, EventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            Products?.Clear();
            if (category.Products == null)
            {
                Products = new ObservableCollection<OrderingProductViewModel>();
            }
            else
            {
                Products = new ObservableCollection<OrderingProductViewModel>(category.Products.Select(p => new OrderingProductViewModel(p)));
                foreach (OrderingProductViewModel productViewModel in Products)
                {
                    productViewModel.ProductClicked += ProductViewModel_ProductClicked;
                }
            }
        }

        private void ProductViewModel_ProductClicked(object sender, OrderingProductEventArgs e)
        {
            ProductClicked?.Invoke(this, e);
        }
    }
}
