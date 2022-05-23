using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;

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

        private CommandBase pressCommand;
        public CommandBase PressCommand
        {
            get => pressCommand;
            set
            {
                pressCommand = value;
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
                ResetViewModel();
            }
        }

        private async void ResetViewModel()
        {
            ID = cafe.CafeID;

            Address = cafe.Address;
            ThisMonthRevenue = "$0";
            LastMonthRevenue = "$0";

            Staff manager = await Database.GetStaff(await Database.FindManagerReference(cafe.CafeID));
            if (manager != null)
            {
                ManagerName = manager.Name;
            }
            else
            {
                ManagerName = "No manager";
            }
        }

        public CafeCardViewModel(Cafe cafe)
        {
            this.Cafe = cafe;
        }
    }
}