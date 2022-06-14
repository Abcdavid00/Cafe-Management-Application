using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Service;
using CSWBManagementApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class OrderedProductViewModel : ViewModelBase
    {
        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private int size;
        public string Size
        {
            get
            {
                if (size==0)
                {
                    return "S";
                }
                if (size==1)
                {
                    return "M";
                }
                return "L";
            }
        }

        private int quantity;
        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public string CombinedName
        {
            get => $"{Name} ({Size})";
        }

        private int price;

        public int Price
        {
            get => price;
            set
            {
                price = value;
                OnPropertyChanged(nameof(PriceString));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged();
            }
        }
        public string PriceString
        {
            get => MiscFunctions.IntToPrice(price);
        }

        public string Total
        {
            get => MiscFunctions.IntToPrice(price * quantity);
        }

        public OrderedProductViewModel(string name, int size, int quantity, int price)
        {
            Name = name;
            this.size = size;
            Quantity = quantity;
            Price = price;
        }
    }

    internal class OrderDisplayViewModel : ViewModelBase
    {
        private ActiveOrder activeOrder;
        public ActiveOrder ActiveOrder
        {
            get => activeOrder;
            set
            {
                if (activeOrder != null)
                {
                    activeOrder.OrderedProductsChanged -= ActiveOrder_OrderedProductsChanged;
                }
                activeOrder = value;               
                if (activeOrder!= null)
                {
                    UpdateProducts();
                    activeOrder.OrderedProductsChanged += ActiveOrder_OrderedProductsChanged;
                }
                OnPropertyChanged(nameof(ActiveOrder));
            }
        }

        private void ActiveOrder_OrderedProductsChanged(object sender, EventArgs e)
        {
            UpdateProducts();
        }

        private Order previousOrder;
        public Order PreviousOrder
        {
            get => previousOrder;
            set
            {
                previousOrder = value;
                if (previousOrder!=null)
                {
                    UpdateProducts();
                }
                OnPropertyChanged(nameof(PreviousOrder));
            }
        }

        private int Total
        { 
            get
            {
                if (ActiveOrder != null)
                {
                    int total = 0;
                    foreach (OrderedProduct op in ActiveOrder.OrderedProducts)
                    {
                        total += GetPrice(op.ProductID, op.Size);
                    }
                    return total;
                }
                if (PreviousOrder != null)
                {
                    return (int)PreviousOrder.Total;
                }
                return 0;
            }
        }

        public string TotalString => MiscFunctions.IntToPrice(Total);

        private int floorNumber;
        public int FloorNumber
        {
            get => floorNumber;
            set
            {
                floorNumber = value;
                OnPropertyChanged(nameof(FloorString));
                OnPropertyChanged();
            }
        }
        public string FloorString => (floorNumber == 0) ? "" : $"Floor {floorNumber}";

        ObservableCollection<OrderedProductViewModel> products;
        public ObservableCollection<OrderedProductViewModel> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        private Func<string, int, int> GetPrice;
        private Func<string, string> GetName;

        private int tableNumber;
        public int TableNumber
        {
            get => tableNumber;
            set
            {
                tableNumber = value;
                OnPropertyChanged(nameof(TableString));
                OnPropertyChanged();
            }
        }
        public string TableString => (tableNumber != 0) ? $"Table: {tableNumber}" : "";

        public OrderDisplayViewModel(Func<string, int, int> getPrice, Func<string, string>getName)
        {
            GetPrice = getPrice;
            GetName = getName;
            ActiveOrder = null;
            PreviousOrder = null;
            FloorNumber = 0;
            TableNumber = 0;
        }

        private void UpdateProducts()
        {
            Products?.Clear();           
            
            if (ActiveOrder != null)
            {
                Products = new ObservableCollection<OrderedProductViewModel>(ActiveOrder.NestedOrderedProducts.Select(p => new OrderedProductViewModel(GetName(p.ProductID), p.Size, p.Count, GetPrice(p.ProductID, p.Size))));
            }
            else
            if (PreviousOrder != null)
            {
                Products = new ObservableCollection<OrderedProductViewModel>(PreviousOrder.NestedOrderedProducts.Select(p => new OrderedProductViewModel(GetName(p.ProductID), p.Size, p.Count, GetPrice(p.ProductID, p.Size))));
            }
            else
            {
                Products = new ObservableCollection<OrderedProductViewModel>();
            }
            
        }

        public void Update(ActiveOrder activeOrder, int floorIndex, int tableNumber)
        {
            PreviousOrder = null;
            ActiveOrder = activeOrder;
            FloorNumber = floorIndex+1;
            TableNumber = tableNumber;
            UpdateProducts();
        }

        public void Update(Order previousOrder, int floorIndex, int tableNumber)
        {
            ActiveOrder = null;
            PreviousOrder = previousOrder;
            FloorNumber = floorIndex+1;
            TableNumber = tableNumber;
            UpdateProducts();
        }

        
    }

    internal class OrderInterfaceViewModel : ViewModelBase
    {
        private Staff staff;

        private Cafe cafe;

        private OrderDisplayViewModel orderDisplayViewModel;
        public OrderDisplayViewModel OrderDisplayViewModel
        {
            get => orderDisplayViewModel;
            set
            {
                orderDisplayViewModel = value;
                OnPropertyChanged(nameof(OrderDisplayViewModel));
            }
        }

        public OrderInterfaceViewModel(Staff staff)
        {
            this.staff = staff;
            MainButtonIsEnabled = false;
            Initiallize();
        }

        private List<Product> products;

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

            activeOrdersMap = new List<Dictionary<Position, ActiveOrder>>();
            previousOrdersMap = new List<Dictionary<Position, Order>>();
            
            foreach (Floor floor in cafe.Floors)
            {
                orderFloorLayouts.Add(new OrderFloorLayoutViewModel(floor, i));
                i += floor.Tables.Count;
                orderFloorLayouts.Last().TileClicked += FloorTileClicked;
                FloorButtons.Add(new FloorButtonViewModel(floor.FloorNumber.ToString(), floor.FloorName, new CommandBase(() =>
                {
                    SelectedFloor = floor.FloorNumber - 1;
                })));
                activeOrdersMap.Add(new Dictionary<Position, ActiveOrder>());
                previousOrdersMap.Add(new Dictionary<Position, Order>());
                
            }

            Categories?.Clear();
            Categories = new ObservableCollection<OrderingCategoryViewModel>(categories.Select(c => new OrderingCategoryViewModel(c)));
            products = new List<Product>();
            foreach (Category category in categories)
            {
                await category.GetProductsAsync();
                products.AddRange(category.Products);
            }
            foreach (var category in Categories)
            {
                category.ProductClicked += Category_ProductClicked;
            }
            OrderDisplayViewModel = new OrderDisplayViewModel(GetPrice, GetName);
            categories.Clear();
        }

        private bool mainButtonIsEnabled;
        public bool MainButtonIsEnabled
        {
            get => mainButtonIsEnabled;
            set
            {
                mainButtonIsEnabled = value;
                OnPropertyChanged();
            }
        }
        private int GetPrice(string productID, int size)
        {
            Product product = products.First(p => p.ProductID == productID);
            if (product != null)
            {
                if (size == 0)
                {
                    return product.SPrice;
                }
                else if (size == 1)
                {
                    return product.MPrice;
                }
                return product.LPrice;
            }
            return 0;
        }

        private string GetName(string productID)
        {
            Product product = products.First(p => p.ProductID == productID);
            if (product != null)
            {
                return product.Name;
            }
            return "";
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


        #region Ordering

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

        private int selectedTableNumber;
        private int SelectedTableNumber
        {
            get { return selectedTableNumber; }
            set
            {
                selectedTableNumber = value;
                OnPropertyChanged();
            }
        }

        private List<Dictionary<Position, ActiveOrder>> activeOrdersMap;
        private List<Dictionary<Position, Order>> previousOrdersMap;

        private void FloorTileClicked(object sender, Position e)
        {
            if (SelectedTableFloor != -1 && SelectedTablePosition != new Position(-1, -1))
            {
                SetTableSelected(SelectedTableFloor, SelectedTablePosition, false);
            }
            SelectedTableFloor = orderFloorLayouts.Count - (sender as OrderFloorLayoutViewModel).FloorNumber;
            SelectedTablePosition = e;
            SelectedTableNumber = GetTableNumber(SelectedTableFloor, SelectedTablePosition);
            OnPropertyChanged(nameof(MainButtonContent));
            SetTableSelected(SelectedTableFloor, SelectedTablePosition, true);

            ActiveOrder activeOrder;
            if (!activeOrdersMap[SelectedTableFloor].TryGetValue(SelectedTablePosition, out activeOrder))
            {
                activeOrdersMap[SelectedTableFloor].Add(SelectedTablePosition, null);
                activeOrder = activeOrdersMap[SelectedTableFloor][SelectedTablePosition];
            }
            CurrentActiveOrder = activeOrder;

            Order order;
            if (!previousOrdersMap[SelectedTableFloor].TryGetValue(SelectedTablePosition, out order))
            {
                previousOrdersMap[SelectedTableFloor].Add(SelectedTablePosition, null);
                order = previousOrdersMap[SelectedTableFloor][SelectedTablePosition];
            }
            CurrentOrder = order;
            
            if (CurrentActiveOrder!=null)
            {
                OrderDisplayViewModel.Update(CurrentActiveOrder, SelectedTableFloor, GetTableNumber(SelectedTableFloor, SelectedTablePosition));
            } else
            {
                OrderDisplayViewModel.Update(CurrentOrder, SelectedTableFloor, GetTableNumber(SelectedTableFloor, SelectedTablePosition));
            }
            MainButtonIsEnabled = true;   
        }

        private ActiveOrder currentActiveOrder;
        private ActiveOrder CurrentActiveOrder
        {
            get { return currentActiveOrder; }
            set
            {
                currentActiveOrder = value;
                OnPropertyChanged(nameof(MainButtonContent));
                OnPropertyChanged(nameof(IsMenuEnabled));
                OnPropertyChanged();
            }
        }
        private Order currentOrder;
        private Order CurrentOrder
        {
            get { return currentOrder; }
            set
            {
                currentOrder = value;
                
                OnPropertyChanged();
            }
        }

        private void SetTableSelected(int floorIndex, Position tablePosition, bool value)
        {
            orderFloorLayouts[floorIndex].SetTableSelected(tablePosition, value);
        }

        private void SetTableActivated(int floorIndex, Position tablePosition, bool value)
        {
            orderFloorLayouts[floorIndex].SetTableActivated(tablePosition, value);
        }

        private int GetTableNumber(int floorIndex, Position tablePosition)
        {
            if (!orderFloorLayouts[floorIndex].TableMap.TryGetValue(tablePosition, out int result))
            {
                return -1;
            }
            return result;
        }

        private void StartNewOrder()
        {
            if (CurrentActiveOrder == null)
            {
                activeOrdersMap[SelectedTableFloor][SelectedTablePosition] = new ActiveOrder(this.cafe.CafeID,SelectedTableFloor,SelectedTablePosition);
                CurrentActiveOrder = activeOrdersMap[SelectedTableFloor][SelectedTablePosition];
                OrderDisplayViewModel.Update(CurrentActiveOrder, SelectedTableFloor, GetTableNumber(SelectedTableFloor, SelectedTablePosition));
                SetTableActivated(SelectedTableFloor, SelectedTablePosition, true);
            }
        }
        private void Category_ProductClicked(object sender, OrderingProductEventArgs e)
        {
            if (CurrentActiveOrder!= null)
            {
                CurrentActiveOrder.AddOrderedProduct(new OrderedProduct(e.Product.ProductID, e.Size));
                int i = 1;
            }
        }

        private async void FinishOrder()
        {
            if (CurrentActiveOrder != null)
            {
                int total = 0;
                foreach (OrderedProduct op in CurrentActiveOrder.OrderedProducts)
                {
                    total += GetPrice(op.ProductID, op.Size);
                }

                DateTime now = DateTime.Now;
                DateTime time = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
                Order order = await Database.CreateOrderAsync(new Order(this.cafe.CafeID, this.staff.UID, time, total, CurrentActiveOrder.OrderedProducts));
                previousOrdersMap[SelectedTableFloor][SelectedTablePosition]= order;
                CurrentOrder = previousOrdersMap[SelectedTableFloor][SelectedTablePosition];
                activeOrdersMap[SelectedTableFloor][SelectedTablePosition]= null;
                CurrentActiveOrder = null;
                SetTableActivated(SelectedTableFloor, SelectedTablePosition, false);
            }
        }

        public ICommand MainButtonCommand
        {
            get => new CommandBase(() =>
            {
                if (CurrentActiveOrder==null)
                {
                    StartNewOrder();
                } else
                {
                    FinishOrder();
                }
            });
        }

        public bool IsMenuEnabled
        {
            get
            {
                if (!(SelectedTableFloor == -1 || (SelectedTablePosition ?? new Position(-1, -1)) == new Position(-1, -1)))
                {
                    if (CurrentActiveOrder != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public string MainButtonContent
        {
            get
            {
                if (SelectedTableFloor == -1 || (SelectedTablePosition?? new Position(-1,-1)) == new Position(-1,-1))
                {
                    return "Select a table to order";
                }
                if (CurrentActiveOrder == null)
                {
                    return "Order";
                }
                return "Finish Order";
            }
        }

        #endregion
    }
}
