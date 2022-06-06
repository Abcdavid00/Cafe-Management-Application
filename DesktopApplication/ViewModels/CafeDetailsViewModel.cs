using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class CafeDetailsViewModel : ViewModelBase
    {
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
            this.CafeStaffViewModel = new CafeStaffViewModel(cafe);
        }

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

        public bool IsStaffsSelected
        {
            get => SelectedIndex == 2;
        }

        public ICommand StaffsButtonPressed
        {
            get => new CommandBase(() => SelectedIndex = 2);
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
                OnPropertyChanged(nameof(IsStaffsSelected));
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

        private CafeStaffViewModel cafeStaffViewModel;

        public CafeStaffViewModel CafeStaffViewModel
        {
            get => cafeStaffViewModel;
            set
            {
                cafeStaffViewModel = value;
                OnPropertyChanged();
            }
        }
    }
}