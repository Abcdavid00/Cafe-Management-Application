using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Resources;
using CSWBManagementApplication.Services;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class CafeStaffViewModel : ViewModelBase
    {
        internal class StaffDetailViewModel : ViewModelBase
        {
            private bool hasInfo;
            private bool isManager;

            public Brush Background
            {
                get => (isManager ? DarkTheme.LinearMain : DarkTheme.SolidLight);
            }

            public Brush Foreground
            {
                get => (isManager ? DarkTheme.SolidLight : DarkTheme.SolidDark);
            }

            private string name;

            public string Name
            {
                get => (hasInfo ? name : "---");
                set
                {
                    name = value;
                    OnPropertyChanged();
                }
            }

            private string email;

            public string Email
            {
                get => email;
                set
                {
                    email = value;
                    OnPropertyChanged();
                }
            }

            private string phone;

            public string Phone
            {
                get => (hasInfo ? phone : "---");
                set
                {
                    phone = value;
                    OnPropertyChanged();
                }
            }

            private bool isMale;

            public bool IsMale
            {
                get => (hasInfo ? isMale : false);
                set
                {
                    if (isMale != value)
                    {
                        isMale = value;
                        OnPropertyChanged(nameof(SexIcon));
                        OnPropertyChanged(nameof(SexColor));
                        OnPropertyChanged();
                    }
                }
            }

            public PackIconKind SexIcon
            {
                get => (hasInfo ? (isMale ? PackIconKind.GenderMale : PackIconKind.GenderFemale) : PackIconKind.None);
            }

            public Brush SexColor
            {
                get => (hasInfo ? (IsMale ?
                    new SolidColorBrush(Color.FromArgb(255, 40, 100, 250)) :
                    new SolidColorBrush(Color.FromArgb(255, 205, 60, 90))) :
                    new SolidColorBrush(Colors.Transparent));
            }

            private DateTime birthdate;

            public DateTime Birthdate
            {
                get => birthdate;
                set => birthdate = value;
            }

            public string BirthdateText
            {
                get => (hasInfo ? Birthdate.ToString("dd-MM-yyyy") : "---");
            }

            private ICommand command;

            public ICommand Command
            {
                get => command;
            }

            private Staff staff;

            public Staff Staff
            {
                get => staff;
            }

            public StaffDetailViewModel(Staff staff, ICommand command, bool isManager = false)
            {
                this.isManager = isManager;
                this.staff = staff;
                hasInfo = true;
                Name = staff.Name;
                Email = staff.Email;
                Phone = staff.Phone;
                IsMale = staff.IsMale;
                Birthdate = staff.Birthdate;
                this.command = command;
            }

            public StaffDetailViewModel(StaffPlaceholder staffPlaceHolder, ICommand command)
            {
                isManager = false;
                hasInfo = false;
                Email = staffPlaceHolder.Email;
                this.command = command;
            }
        }

        private Cafe cafe;

        private Privilege privilege;

        private bool initiallized;

        public CafeStaffViewModel(Cafe cafe, Privilege privilege)
        {
            this.cafe = cafe;
            initiallized = false;
            this.isViewingStaffDetails = false;
            this.privilege = privilege;
            GetDatabaseData();
            FindStaffViewModel = new FindStaffViewModel(cafe);
            this.cafe.OnCafeStaffListChanged += ((object sender, EventArgs e )=>RefreshList() );
        }

        private ObservableCollection<StaffDetailViewModel> staffInfos;

        public ObservableCollection<StaffDetailViewModel> StaffInfos
        {
            get => staffInfos;
            set
            {
                staffInfos = value;
                OnPropertyChanged();
            }
        }

        public ViewModelBase AlternativeViewModel
        {
            get
            {
                if (IsViewingStaffDetails)
                {
                    return StaffDetailsViewModel;
                }
                else
                {
                    return FindStaffViewModel;
                }
            }
        }

        private StaffDetailsViewModel staffDetailsViewModel;

        private StaffDetailsViewModel StaffDetailsViewModel
        {
            get => staffDetailsViewModel;
            set
            {
                staffDetailsViewModel = value;
                OnPropertyChanged();
            }
        }

        private bool isViewingStaffDetails;

        private bool IsViewingStaffDetails
        {
            get => isViewingStaffDetails;
            set
            {
                if (isViewingStaffDetails != value)
                {
                    isViewingStaffDetails = value;
                    OnPropertyChanged(nameof(AlternativeViewModel));
                    OnPropertyChanged(nameof(IsViewingStaffDetails));
                }
                OnPropertyChanged(nameof(RemoveStaffButtonVisibility));
                OnPropertyChanged(nameof(RemoveStaffPlaceholderButtonVisibility));
                OnPropertyChanged(nameof(MakeManagerButtonVisibility));
            }
        }

        private FindStaffViewModel findStaffViewModel;

        private FindStaffViewModel FindStaffViewModel
        {
            get => findStaffViewModel;
            set
            {
                findStaffViewModel = value;
                OnPropertyChanged();
            }
        }

        private async void RefreshList()
        {
            
            List<StaffPlaceholder> staffPlaceholders = (await Database.GetStaffPlaceholders(cafe.CafeID)).ToList();
            this.StaffDetailsViewModel = new StaffDetailsViewModel();
            StaffInfos?.Clear();
            StaffInfos = new ObservableCollection<StaffDetailViewModel>();

            if (cafe.Manager != null)
            {
                StaffInfos.Add(new StaffDetailViewModel(cafe.Manager, new CommandBase(() => StaffDetailsViewModel.UpdateInfo(cafe.Manager, true)), true));
            }

            foreach (Staff staff in cafe.Staffs.Values)
            {
                StaffInfos.Add(new StaffDetailViewModel(staff, new CommandBase(() => StaffDetailsViewModel.UpdateInfo(staff))));
            }

            foreach (StaffPlaceholder staffPlaceholder in staffPlaceholders)
            {
                StaffInfos.Add(new StaffDetailViewModel(staffPlaceholder, new CommandBase(() => StaffDetailsViewModel.UpdateInfo(staffPlaceholder))));
            }

            if (cafe.Manager != null)
            {
                this.StaffDetailsViewModel.UpdateInfo(cafe.Manager, true);
            }
            else if (cafe.Staffs.Count > 0)
            {
                this.StaffDetailsViewModel.UpdateInfo(cafe.Staffs.Values.First());
            }
            else if (staffPlaceholders.Count > 0)
            {
                this.StaffDetailsViewModel.UpdateInfo(staffPlaceholders.First());
            }

            this.StaffDetailsViewModel.OnInfoUpdate += StaffDetailsViewModel_OnInfoUpdate;

            #region Debug
#if DEBUG
            //Staff debugManager = new Staff()
            //{
            //    CafeID = cafe.CafeID,
            //    Name = $"Vu Viet Huy",
            //    Email = "hduykhang100@gmail.com",
            //    Phone = "1234567890",
            //    IsMale = true,
            //    Birthdate = DateTime.Today
            //};

            //StaffInfos.Add(new StaffDetailViewModel(debugManager, new CommandBase(() => StaffDetailsViewModel.UpdateInfo(debugManager, true)), true));

            //for (int i = 0; i < 10; i++)
            //{
            //    Staff debugStaff = new Staff()
            //    {
            //        CafeID = cafe.CafeID,
            //        Name = $"Huynh Duy Khang Khung Khang {i}",
            //        Email = $"hduykhang0{i}@gmail.com",
            //        Phone = "1234567890",
            //        IsMale = i % 2 == 0,
            //        Birthdate = DateTime.Today
            //    };
            //    StaffInfos.Add(new StaffDetailViewModel(debugStaff, new CommandBase(() => StaffDetailsViewModel.UpdateInfo(debugStaff))));
            //}

            //for (int i = 0; i < 10; i++)
            //{
            //    StaffPlaceholder debugStaffPlaceholder = new StaffPlaceholder()
            //    {
            //        CafeID = cafe.CafeID,
            //        Email = $"hduykhang0{i}@gmail.com",
            //    };
            //    StaffInfos.Add(new StaffDetailViewModel(debugStaffPlaceholder, new CommandBase(() => StaffDetailsViewModel.UpdateInfo(debugStaffPlaceholder))));
            //}
#endif
            #endregion
            
            initiallized = true;
        }

        private async void GetDatabaseData()
        {          
            await cafe.GetCafeStaffsInfo();
            RefreshList();
            
        }

        private void StaffDetailsViewModel_OnInfoUpdate(object sender, EventArgs e)
        {
            IsViewingStaffDetails = !StaffDetailsViewModel.IsEmpty;
        }

        public Visibility RemoveStaffButtonVisibility
        {
            get
            {
                if (!initiallized)
                {
                    return Visibility.Collapsed;
                }
                return ((IsViewingStaffDetails && !StaffDetailsViewModel.IsPlaceholder) ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public Visibility RemoveStaffPlaceholderButtonVisibility
        {
            get
            {
                if (!initiallized)
                {
                    return Visibility.Collapsed;
                }
                return ((IsViewingStaffDetails && StaffDetailsViewModel.IsPlaceholder) ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public Visibility MakeManagerButtonVisibility
        {
            get
            {
                if (!initiallized)
                {
                    return Visibility.Collapsed;
                }
                return ((IsViewingStaffDetails && !StaffDetailsViewModel.IsPlaceholder && !StaffDetailsViewModel.IsManager && privilege is Privilege.Owner) ? Visibility.Visible : Visibility.Collapsed);
            }
        }

        public ICommand AddStaffCommand
        {
            get => new CommandBase(() =>
            {
                StaffDetailsViewModel.Clear();
            });
        }

        public ICommand RemoveStaffCommand
        {
            get => new CommandBase(() =>
            {
                if (isViewingStaffDetails && !StaffDetailsViewModel.IsPlaceholder)
                {
                    cafe.RemoveStaff(StaffDetailsViewModel.Staff);
                }                
            });
        }

        public ICommand RemoveStaffPlaceholderCommand
        {
            get => new CommandBase(() =>
            {
                if (isViewingStaffDetails && StaffDetailsViewModel.IsPlaceholder)
                {
                    cafe.RemoveStaffPlaceholder(StaffDetailsViewModel.StaffPlaceholder);
                }
            });
        }

        public ICommand MakeManagerCommand
        {
            get => new CommandBase(() =>
            {

            });
        }
    }
}