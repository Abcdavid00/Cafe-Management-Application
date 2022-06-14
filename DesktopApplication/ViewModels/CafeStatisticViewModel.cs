using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Service;
using CSWBManagementApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
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

        public CafeStatisticViewModel(Cafe cafe)
        {
            this.cafe = cafe;
            FilterType = 0;
            Address = cafe.Address;
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
            if (newDate == FDate)
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
            cafe.ChangeAddress(Address);
        });

        public ICommand DiscardAddressCommand => new CommandBase(() =>
        {
            Address = cafe.Address;
        });

        #endregion

        #region Seach

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
            int i = 1;
        }
        
        #endregion

    }
}
