using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class CafeStatisticViewModel : ViewModelBase
    {
        private Cafe cafe;

        public CafeStatisticViewModel(Cafe cafe)
        {
            this.cafe = cafe;
        }

        private bool isToEnabled;
        public bool IsToEnabled
        {
            get => isToEnabled;
            set
            {
                isToEnabled = value;
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
            }
        }

        private int fYear;
        public int FYear
        {
            get => FYear;
            set
            {
                fYear = value;
                OnPropertyChanged(nameof(FYearString));
                OnPropertyChanged();
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
            }
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
            }
        }

    }
}
