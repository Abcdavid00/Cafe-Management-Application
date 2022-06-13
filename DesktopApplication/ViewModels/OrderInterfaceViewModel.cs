using CSWBManagementApplication.Commands;
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
            Initiallize();
        }

        private async void Initiallize()
        {
            this.cafe = await Database.GetCafe(staff.CafeID);
            await cafe.GetCafeFloorsInfo();
            List<ActiveOrder> activeOrders = (await Database.GetAllActiveOrdersAsync(cafe.CafeID)).ToList();
            FloorButtons = new ObservableCollection<FloorButtonViewModel>();
            orderFloorLayouts = new List<OrderFloorLayoutViewModel>();
            int i = 1;
            selectedFloor = -1;
            foreach (Floor floor in cafe.Floors)
            {
                orderFloorLayouts.Add(new OrderFloorLayoutViewModel(floor, i));
                i += floor.Tables.Count;
                FloorButtons.Add(new FloorButtonViewModel(floor.FloorNumber.ToString(), floor.FloorName, new CommandBase(() =>
                {
                    SelectedFloor = floor.FloorNumber - 1;
                })));
            }
        }

        private int selectedFloor;
        public int SelectedFloor
        {
            get => selectedFloor;
            set
            {
                if (selectedFloor >=0 && selectedFloor < FloorButtons.Count)
                {
                    FloorButtons[currentFloorButtonIndex].IsActive = false;
                }
                selectedFloor = value;
                if (selectedFloor >= 0 && selectedFloor < FloorButtons.Count)
                {
                    FloorButtons[currentFloorButtonIndex].IsActive = true;
                }
                if (selectedFloor >= 0 && selectedFloor < orderFloorLayouts.Count)
                {
                    CurrentOrderFloorLayout = orderFloorLayouts[selectedFloor];
                }
                OnPropertyChanged();
            }
        }
        private int currentFloorButtonIndex
        {
            get => FloorButtons.Count - 1 - selectedFloor;
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

        private List<OrderFloorLayoutViewModel> orderFloorLayouts;

        private OrderFloorLayoutViewModel currentOrderFloorLayout;
        
        public OrderFloorLayoutViewModel CurrentOrderFloorLayout
        {
            get => currentOrderFloorLayout;
            set
            {
                currentOrderFloorLayout = value;
                OnPropertyChanged(nameof(CurrentOrderFloorLayout));
            }
        }
    
        
    }
}
