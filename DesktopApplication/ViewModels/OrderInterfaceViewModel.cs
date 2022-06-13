using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class OrderInterfaceViewModel : ViewModelBase
    {
        private Staff staff;

        private Cafe cafe;
        
        public OrderInterfaceViewModel(Staff staff)
        {
            this.staff = staff;
            
        }

        private ObservableCollection<FloorButtonViewModel> floorButtons;

        public ObservableCollection<FloorButtonViewModel> FloorButtons
        {
            get => floorButtons;
            private set
            {
                floorButtons = value;
                OnPropertyChanged(nameof(FloorButtons));
            }
        }

        private async void Initiallize()
        {
            this.cafe = await Database.GetCafe(staff.CafeID);
        }
    }
}
