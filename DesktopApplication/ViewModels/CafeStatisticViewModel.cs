using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Service;
using CSWBManagementApplication.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class MiniOrderedProductViewModel : ViewModelBase
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
                if (size == 0)
                {
                    return "S";
                }
                if (size == 1)
                {
                    return "M";
                }
                return "L";
            }
        }

        private int quantity;

        public MiniOrderedProductViewModel(string name, int size, int quantity )
        {
            Name = name;
            this.size = size;
            Quantity = quantity;
        }

        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }

        public string CombinedName
        {
            get => $"{Name} ({Size})";
        }
    }

    internal class OrderDetailsViewModel : ViewModelBase
    {
        private Order order;
        public Order Order
        {
            get => order;
            set
            {
                order = value;
                OnPropertyChanged(nameof(Time));
                OnPropertyChanged(nameof(TotalString));
                UpdateProducts();
                OnPropertyChanged();
            }
        }

        public string Time
        {
            get => (this.Order != null) ? order.Time.ToString("dd/MM/yyyy\nHH:mm:ss"):"";
        }

        private string staffName;
        public string StaffName
        {
            get => staffName;
            set
            {
                staffName = value;
                OnPropertyChanged();
            }
        }


        private int Total
        {
            get
            {
                if (Order != null)
                {
                    return (int)Order.Total;
                }
                return 0;
            }
        }
        public string TotalString => (this.Order != null) ? MiscFunctions.IntToPrice(Total) : "";

        ObservableCollection<MiniOrderedProductViewModel> products;
        public ObservableCollection<MiniOrderedProductViewModel> Products
        {
            get => products;
            set
            {
                products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        public void UpdateProducts()
        {
            if (Order != null)
            {
                Products = new ObservableCollection<MiniOrderedProductViewModel>(Order.NestedOrderedProducts.Select(p => new MiniOrderedProductViewModel(GetName(p.ProductID), p.Size, p.Count)));
            }
            else
            {
                Products = new ObservableCollection<MiniOrderedProductViewModel>();
            }
        }

        private Func<string, string> GetName;

        public void Update(Order order, string staffName)
        {
            Order = order;
            StaffName = staffName;
        }

        public OrderDetailsViewModel(Func<string, string> getName)
        {
            Order = null;
            GetName = getName;
        }
    }
    
    internal class HistoryOrderViewModel : ViewModelBase
    {
        private Order order;

        private ICommand command;
        public ICommand Command
        {
            get => command;
            set
            {
                command = value;
                OnPropertyChanged();
            }
        }

        public string Time
        {
            get => order.Time.ToString("dd/MM/yyyy\nHH:mm:ss");
        }

        private string staffName;
        public string StaffName
        {
            get => staffName;
            set
            {
                staffName = value;
                OnPropertyChanged();
            }
        }

        public string Total
        {
            get => MiscFunctions.IntToPrice((int)order.Total);
        }

        public HistoryOrderViewModel(Order order, string staffName , CommandBase command)
        {
            this.order = order;
            this.staffName = staffName;
            this.command = command;
        }
    }
    
    internal class CafeStatisticViewModel : ViewModelBase
    {
        private Cafe cafe;
        private List<Staff> staffs;
        private List<Product> products;
        private OrderDetailsViewModel orderDetails;
        public OrderDetailsViewModel OrderDetails
        {
            get => orderDetails;
            set
            {
                orderDetails = value;
                OnPropertyChanged();
            }
        }
        
        public CafeStatisticViewModel(Cafe cafe)
        {
            this.cafe = cafe;
            cafe.OnCafeManagerChanged += Cafe_OnCafeManagerChanged;
            FilterType = 0;
            Address = cafe.Address;
            initiallized = false;
            HistoryOrders = new ObservableCollection<HistoryOrderViewModel>();
            OrderDetails = new OrderDetailsViewModel(GetProductName);
            FDate = DateTime.Now.Day;
            FMonth = DateTime.Now.Month;
            FYear = DateTime.Now.Year;
            TDate = DateTime.Now.Day;
            TMonth = DateTime.Now.Month;
            TYear = DateTime.Now.Year;
            Initiallize();
        }

        private ObservableCollection<HistoryOrderViewModel> historyOrders;
        public ObservableCollection<HistoryOrderViewModel> HistoryOrders
        {
            get => historyOrders;
            set
            {
                historyOrders = value;
                OnPropertyChanged();
            }
        }

        private bool initiallized;

        public async void Initiallize()
        {
            staffs = (await Database.GetAllStaffsAsync()).ToList();
            products = (await Database.GetAllProductsAsync()).ToList();
            Staff manager = await Database.GetStaff(await Database.FindManagerAsync(cafe.CafeID));
            if (manager != null)
            {
                ManagerName = manager.Name;
            }
            else
            {
                ManagerName = "No manager";
            }
            initiallized = true;
            
        }

        private async void Cafe_OnCafeManagerChanged(object sender, System.EventArgs e)
        {
            Staff manager = await Database.GetStaff(await Database.FindManagerAsync(cafe.CafeID));
            if (manager != null)
            {
                ManagerName = manager.Name;
            }
            else
            {
                ManagerName = "No manager";
            }
        }

        #region Datetime
        private bool isToEnabled;
        public bool IsToEnabled
        {
            get => isToEnabled;
            set
            {
                isToEnabled = value;
                OnPropertyChanged(nameof(IsTDateEnabled));
                OnPropertyChanged(nameof(IsTMonthEnabled));
                OnPropertyChanged();
            }
        }

        private int fDateIndex;
        public int FDateIndex
        {
            get => fDateIndex;
            set
            {
                fDateIndex = value;
                OnPropertyChanged(nameof(FDate));
                OnPropertyChanged();
                ValidateFDate();
            }
        }
        public int FDate
        {
            get => fDateIndex + 1;
            set
            {
                fDateIndex = value - 1;                
                OnPropertyChanged(nameof(FDateIndex));
                OnPropertyChanged();
                ValidateFDate();
            }
        }

        private int fMonthIndex;
        public int FMonthIndex
        {
            get => fMonthIndex;
            set
            {
                fMonthIndex = value;
                OnPropertyChanged(nameof(FMonth));
                OnPropertyChanged();
                ValidateFDate();
            }
        }
        public int FMonth
        {
            get => fMonthIndex + 1;
            set
            {
                fMonthIndex = value - 1;
                OnPropertyChanged(nameof(FMonthIndex));
                OnPropertyChanged();
                ValidateFDate();
            }
        }

        private int fYear;
        public int FYear
        {
            get => fYear;
            set
            {
                fYear = value;
                OnPropertyChanged(nameof(FYearString));
                OnPropertyChanged();
                ValidateFDate();
            }
        }
        public string FYearString
        {
            get => fYear.ToString();
            set
            {
                if (int.TryParse(value, out int year))
                {
                    fYear = year;
                    
                }
                OnPropertyChanged(nameof(FYear));
                OnPropertyChanged();
                ValidateFDate();
            }
        }

        private void ValidateFDate()
        {
            int newDate = FDate;
            MiscFunctions.ValidateDate(ref newDate, FMonth, FYear);
            if (newDate == FDate)
            {
                return;
            }
            FDate = newDate;
        }
        
        private int tDateIndex;
        public int TDateIndex
        {
            get => tDateIndex;
            set
            {
                tDateIndex = value;
                OnPropertyChanged(nameof(TDate));
                OnPropertyChanged();
                ValidateTDate();
            }
        }
        public int TDate
        {
            get => tDateIndex + 1;
            set
            {
                tDateIndex = value - 1;
                OnPropertyChanged(nameof(TDateIndex));
                OnPropertyChanged();
                ValidateTDate();
            }
        }

        private int tMonthIndex;
        public int TMonthIndex
        {
            get => tMonthIndex;
            set
            {
                tMonthIndex = value;
                OnPropertyChanged(nameof(TMonth));
                OnPropertyChanged();
                ValidateTDate();
            }
        }
        public int TMonth
        {
            get => tMonthIndex + 1;
            set
            {
                tMonthIndex = value - 1;
                OnPropertyChanged(nameof(TMonthIndex));
                OnPropertyChanged();
                ValidateTDate();
            }
        }

        private int tYear;
        public int TYear
        {
            get => tYear;
            set
            {
                tYear = value;
                OnPropertyChanged(nameof(TYearString));
                OnPropertyChanged();
                ValidateTDate();
            }
        }
        public string TYearString
        {
            get => tYear.ToString();
            set
            {
                if (int.TryParse(value, out int year))
                {
                    tYear = year;
                }
                OnPropertyChanged(nameof(TYear));
                OnPropertyChanged();
                ValidateTDate();
            }
        }

        private void ValidateTDate()
        {
            int newDate = TDate;
            MiscFunctions.ValidateDate(ref newDate, TMonth, TYear);
            if (newDate == TDate)
            {
                return;
            }
            TDate = newDate;
        }

        private int filterType;
        public int FilterType
        {
            get => filterType;
            set
            {
                filterType = MiscFunctions.CappedSetter(value, 0, 2);
                OnPropertyChanged(nameof(IsDateEnabled));
                OnPropertyChanged(nameof(IsMonthEnabled));
                OnPropertyChanged(nameof(IsTDateEnabled));
                OnPropertyChanged(nameof(IsTMonthEnabled));
                OnPropertyChanged();

            }
        }

        public ICommand FilterByDayCommand => new CommandBase(() => FilterType = 0);
        public ICommand FilterByMonthCommand => new CommandBase(() => FilterType = 1);
        public ICommand FilterByYearCommand => new CommandBase(() => FilterType = 2);

        public bool IsDateEnabled
        {
            get => filterType == 0;
        }

        public bool IsTDateEnabled
        {
            get => IsDateEnabled && IsToEnabled;
        }

        public bool IsMonthEnabled
        {
            get => filterType < 2;
        }

        public bool IsTMonthEnabled
        {
            get => IsMonthEnabled && IsToEnabled;
        }
        #endregion

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

        #region Address

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

        public ICommand SaveAddressCommand => new CommandBase(() =>
        {
            SaveAddress();
        });

        private void SaveAddress()
        {
            cafe.ChangeAddress(Address);
        }

        public ICommand DiscardAddressCommand => new CommandBase(() =>
        {
            Address = cafe.Address;
        });

        #endregion

        #region Seach

        private int totalOrders;
        public int TotalOrders
        {
            get => totalOrders;
            set
            {
                totalOrders = value;
                OnPropertyChanged(nameof(TotalOrdersString));
                OnPropertyChanged();
            }
        }
        public string TotalOrdersString
        {
            get => totalOrders.ToString();
        }

        public string GetStaffName(string staffID)
        {
            while (!initiallized)
            {
                Thread.Sleep(100);
            }
            return staffs.First(s => s.UID == staffID).Name;
        } 

        public string GetProductName(string productID)
        {
            while (!initiallized)
            {
                Thread.Sleep(100);
            }
            return products.First(p => p.ProductID == productID).Name;
        }

        public ICommand SearchCommand => new CommandBase(()=> Search());

        private async void Search()
        {
            DateTime startDate = new DateTime(FYear, FMonth, FDate);
            DateTime endDate;
            if (IsToEnabled)
            {
                endDate = new DateTime(TYear, TMonth, TDate);
            }
            else
            {
                endDate = startDate;
            }

            long start;
            long end;
            if (FilterType == 0)
            {
                start = MiscFunctions.MinDate(startDate);
                end = MiscFunctions.MaxDate(endDate);
            }
            else if (FilterType == 1)
            {
                start = MiscFunctions.MinMonth(startDate);
                end = MiscFunctions.MaxMonth(endDate);
            }
            else
            {
                start = MiscFunctions.MinYear(startDate);
                end = MiscFunctions.MaxYear(endDate);
            }

            List<Order> orders = (await Database.GetOrdersAsync(cafe.CafeID,start, end)).ToList();
            TotalOrders = orders.Count;
            HistoryOrders?.Clear();

            HistoryOrders = new ObservableCollection<HistoryOrderViewModel>(orders.Select(o => new HistoryOrderViewModel(o, GetStaffName(o.StaffID), new CommandBase(() => DisplayOrderDetails(o)))));
        }
        
        private void DisplayOrderDetails(Order order)
        {
            OrderDetails.Update(order, GetStaffName(order.StaffID));
        }
        
        #endregion

    }
}
