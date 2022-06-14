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

            List<Category> categories = (await Database.GetAllCategoriesAsync()).ToList();

            

            FloorButtons = new ObservableCollection<FloorButtonViewModel>();
            orderFloorLayouts = new List<OrderFloorLayoutViewModel>();
            int i = 1;
            selectedFloor = -1;
            selectedTableFloor = -1;
            selectedTablePosition = new Position(-1, -1);
            foreach (Floor floor in cafe.Floors)
            {
                orderFloorLayouts.Add(new OrderFloorLayoutViewModel(floor, i));
                i += floor.Tables.Count;
                orderFloorLayouts.Last().TileClicked += FloorTileClicked;
                FloorButtons.Add(new FloorButtonViewModel(floor.FloorNumber.ToString(), floor.FloorName, new CommandBase(() =>
                {
                    SelectedFloor = floor.FloorNumber - 1;
                })));
            }

            Categories?.Clear();
            Categories = new ObservableCollection<OrderingCategoryViewModel>(categories.Select(c => new OrderingCategoryViewModel(c)));
            categories.Clear();
        }


        private int selectedTableFloor;
        private int SelectedTableFloor
        {
            get { return selectedTableFloor; }
            set
            {
                selectedTableFloor = value;
                OnPropertyChanged();
            }
        }

        private Position selectedTablePosition;
        private Position SelectedTablePosition
        {
            get { return selectedTablePosition; }
            set
            {
                selectedTablePosition = value;
                OnPropertyChanged();
            }
        }

        private void FloorTileClicked(object sender, Position e)
        {
            if (SelectedTableFloor != -1 && SelectedTablePosition != new Position(-1, -1))
            {
                SetTableSelected(SelectedTableFloor, SelectedTablePosition, false);
            }
            SelectedTableFloor = orderFloorLayouts.Count - (sender as OrderFloorLayoutViewModel).FloorNumber;
            SelectedTablePosition = e;
            SetTableSelected(SelectedTableFloor, SelectedTablePosition, true);
        }

        private void SetTableSelected(int floorIndex, Position tablePosition, bool value)
        {
            orderFloorLayouts[floorIndex].SetTableSelected(tablePosition, value);
        }

        private void SetTableActivated(int floorIndex, Position tablePosition, bool value)
        {
            orderFloorLayouts[floorIndex].SetTableSelected(tablePosition, value);
        }

        #region FloorLayout
        private int selectedFloor;
        public int SelectedFloor
        {
            get => selectedFloor;
            set
            {
                if (selectedFloor >= 0 && selectedFloor < FloorButtons.Count)
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
        #endregion


        #region Menu

        private ObservableCollection<OrderingCategoryViewModel> categories;
        public ObservableCollection<OrderingCategoryViewModel> Categories
        {
            get => categories;
            private set
            {
                categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        #endregion

    }
}
