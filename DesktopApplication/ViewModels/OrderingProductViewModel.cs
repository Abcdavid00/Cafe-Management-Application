using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class OrderingProductEventArgs
    {
        public OrderingProductEventArgs(Product product, int size)
        {
            Product = product;
            Size = size;
        }

        public Product Product { get; set; }
        public int Size { get; set; }

        
    }

    internal class OrderingProductViewModel : ViewModelBase
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

        public OrderingProductViewModel(Product product)
        {
            this.Product = product;
        }

        public event EventHandler<OrderingProductEventArgs> ProductClicked;

        public string Name
        {
            get => product.Name;
        }

        public bool IsSPriceEnabled
        {
            get => product.SPrice > 0;
        }
        
        public string SPrice
        {
            get => MiscFunctions.IntToPrice(product.SPrice);
        }

        public ICommand SPriceCommand
        {
            get => new CommandBase(()=> { ProductClicked?.Invoke(this, new OrderingProductEventArgs(Product, 0)); });
        }


        public bool IsMPriceEnabled
        {
            get => product.MPrice > 0;
        }

        public string MPrice
        {
            get => MiscFunctions.IntToPrice(product.MPrice);
        }

        public ICommand MPriceCommand
        {
            get => new CommandBase(() => { ProductClicked?.Invoke(this, new OrderingProductEventArgs(Product, 1)); });
        }


        public bool IsLPriceEnabled
        {
            get => product.LPrice > 0;
        }

        public string LPrice
        {
            get => MiscFunctions.IntToPrice(product.LPrice);
        }

        public ICommand LPriceCommand
        {
            get => new CommandBase(() => { ProductClicked?.Invoke(this, new OrderingProductEventArgs(Product, 2)); });
        }


    }
}

