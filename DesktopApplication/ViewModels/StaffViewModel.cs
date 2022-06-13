using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Service;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class StaffViewModel : ViewModelBase
    {
        private OrderInterfaceViewModel orderInterfaceViewModel;
        public OrderInterfaceViewModel OrderInterfaceViewModel
        {
            get => orderInterfaceViewModel;
            set
            {
                orderInterfaceViewModel = value;
                OnPropertyChanged();
            }
        }

        private StaffProfileViewModel staffProfileViewModel;
        public StaffProfileViewModel StaffProfileViewModel
        {
            get => staffProfileViewModel;
            set
            {
                staffProfileViewModel = value;
                OnPropertyChanged();
            }
        }

        private Staff staff;

        private MainViewModel mainViewModel;

        public StaffViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.staff = this.mainViewModel.CurrentUser as Staff;
            OrderInterfaceViewModel = new OrderInterfaceViewModel(this.staff);
            StaffProfileViewModel = new StaffProfileViewModel(this.staff);
        }

        public ICommand LogoutCommand
        {
            get => new CommandBase(() => { mainViewModel.UserLink = null; });
        }

        private int selectedIndex;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = MiscFunctions.CappedSetter(value,0,1);
                OnPropertyChanged(nameof(IsOrderSelected));
                OnPropertyChanged(nameof(IsStaffProfileSelected));
                OnPropertyChanged();
            }
        }

        public bool IsOrderSelected { get => SelectedIndex == 0; }     

        public ICommand OrderButtonPressed
        {
            get => new CommandBase(() => { SelectedIndex=0; });
        }

        public bool IsStaffProfileSelected { get => SelectedIndex == 1; }

        public ICommand StaffProfileButtonPressed
        {
            get => new CommandBase(() => { SelectedIndex = 1; });
        }
    }
}