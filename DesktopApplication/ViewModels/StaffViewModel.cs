using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Service;
using CSWBManagementApplication.Services;
using System;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{

    internal class StaffProfileViewModel : ViewModelBase
    {
        private Staff staff;

        public StaffProfileViewModel(Staff staff)
        {
            this.staff = staff;
            LoadCafeDetails();
            Email = staff.Email;
            Name = staff.Name;
            Birthdate = staff.Birthdate;
            Phone = staff.Phone;
            IsMale = staff.IsMale;
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private string cafeAddress;
        public string CafeAddress
        {
            get => cafeAddress;
            private set
            {
                cafeAddress = value;
                OnPropertyChanged();
            }
        }

        private DateTime birthdate;
        public DateTime Birthdate
        {
            get => birthdate;
            set
            {
                birthdate = value;
                OnPropertyChanged();
            }
        }

        private int sex;
        public int Sex
        {
            get => sex;
            set
            {
                sex = value;
                OnPropertyChanged(nameof(IsMale));
                OnPropertyChanged();
            }
        }
        public bool IsMale
        {
            get => sex == 0;
            set
            {
                sex = (value) ? 0 : 1;
                OnPropertyChanged(nameof(Sex));
                OnPropertyChanged();
            }
        }

        private string phone;
        public string Phone
        {
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged();
            }
        }

        private string email;
        public string Email
        {
            get => email;
            private set { email = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand => new CommandBase(()=> 
        {
            SaveChanges();
        });

        public void SaveChanges()
        {
            staff.Name = Name;
            staff.Birthdate = Birthdate;
            staff.IsMale = IsMale;
            staff.Phone = Phone;
            staff.UpdateInfo();
        }

        public ICommand DiscardCommand => new CommandBase(()=>
        {
            Name = staff.Name;
            Birthdate = staff.Birthdate;
            Phone = staff.Phone;
            IsMale = staff.IsMale;
        });

        private async void LoadCafeDetails()
        {
            Cafe cafe = await Database.GetCafe(staff.CafeID);
            CafeAddress = cafe.Address;
            
        }
    }

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