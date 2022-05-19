using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class CafeCardViewModel : ViewModelBase
    {
        private string id;
        public string ID
        {
            get => id;
            set 
            {
                id = value;
                OnPropertyChanged();
            }
        }

        private string managerName;
        public string ManagerName
        {
            get => managerName;
            set
            {
                managerName = value;
                OnPropertyChanged();
            }
        }

        private string address;
        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged();
            }
        }

        private string thisMonthRevenue;
        public string ThisMonthRevenue
        {
            get => thisMonthRevenue;
            set
            {
                thisMonthRevenue = value;
                OnPropertyChanged();
            }
        }

        private string lastMonthRevenue;
        public string LastMonthRevenue
        {
            get => lastMonthRevenue;
            set
            {
                lastMonthRevenue = value;
                OnPropertyChanged();
            }
        }

        private Cafe cafe;
        public Cafe Cafe
        {
            get => cafe;
            set
            {
                cafe = value;
                OnPropertyChanged();
            }
        }
    }
}
