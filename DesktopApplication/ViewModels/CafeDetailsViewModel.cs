using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class CafeDetailsViewModel : ViewModelBase
    {
        private string address;
        public string Address
        {
            get { return address; }
            set
            {
                address = value;
                OnPropertyChanged();
            }
        }

        private Cafe cafe;
        public Cafe Cafe
        {
            get { return cafe; }
            set
            {
                cafe = value;
                OnPropertyChanged();
            }
        }

        public CafeDetailsViewModel(Cafe cafe)
        {
            this.Cafe = cafe;
            this.Address = cafe.Address;
        }
    }
}
