using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class ProductViewModel : ViewModelBase
    {        
        private Product product;

        private bool inCategory
        {
            get => !string.IsNullOrEmpty(product.CategoryID);
        }

        public ProductViewModel(Product product)
        {
            this.product = product;
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

        public event EventHandler<Product> AddButtonClicked;

        public ICommand Add
        {
            get => new CommandBase(() =>
            {
                AddButtonClicked?.Invoke(this, product);
            });
        }

        public event EventHandler<Product> RemoveButtonClicked;

        public ICommand Remove
        {
            get => new CommandBase(() =>
            {
                RemoveButtonClicked?.Invoke(this, product);
            });
        }

        public event EventHandler<Product> EditButtonClicked;

        public ICommand Edit
        {
            get => new CommandBase(() =>
            {
                EditButtonClicked?.Invoke(this, product);
            });
        }
    }
}
