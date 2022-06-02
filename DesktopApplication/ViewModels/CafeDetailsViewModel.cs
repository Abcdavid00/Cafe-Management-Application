using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class CafeDetailsViewModel : ViewModelBase
    {
        public bool IsStatisticSelected
        {
            get => SelectedIndex == 0;
        }

        public ICommand StatisticsButtonPressed
        {
            get => new CommandBase(() => SelectedIndex = 0);
        }

        public bool IsLayoutSelected
        {
            get => SelectedIndex == 1;
        }

        public ICommand LayoutButtonPressed
        {
            get => new CommandBase(() => SelectedIndex = 1);
        }
        
        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsStatisticSelected));
                OnPropertyChanged(nameof(IsLayoutSelected));
            }
        }
        
        private CafeLayoutViewModel cafeLayoutViewModel;
        public CafeLayoutViewModel CafeLayoutViewModel
        {
            get => cafeLayoutViewModel;
            set
            {
                cafeLayoutViewModel = value;
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
            this.CafeLayoutViewModel = new CafeLayoutViewModel(cafe);
        }
    }
}
