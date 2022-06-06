using CSWBManagementApplication.Models;
using CSWBManagementApplication.Resources;
using CSWBManagementApplication.Services;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    new SolidColorBrush(Colors.Cyan) :
                    new SolidColorBrush(Colors.LightPink)) :
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
                get => (hasInfo ? Birthdate.ToString("yyyy-MM-dd") : "---");
            }
            

            public StaffDetailViewModel(Staff staff, bool isManager = false)
            {
                this.isManager = isManager;
                hasInfo = true;
                Name = staff.Name;
                Email = staff.Email;
                Phone = staff.Phone;
                IsMale = staff.IsMale;
                Birthdate = staff.Birthdate;
            }

            public StaffDetailViewModel(StaffPlaceholder staffPlaceHolder)
            {
                isManager = false;
                hasInfo = false;
                Email = staffPlaceHolder.Email;
            }
        }
        
        private Cafe cafe;

        public CafeStaffViewModel(Cafe cafe)
        {
            this.cafe = cafe;
            
            Initialize();
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
                if (isViewingStaffDetails)
                {
                    return StaffDetailsViewModel;
                } else
                {
                    return FindStaffViewModel;
                }
            }
        }

        private StaffDetailsViewModel staffDetailsViewModel;
        private StaffDetailsViewModel StaffDetailsViewModel
        {
            get => StaffDetailsViewModel;
            set
            {
                StaffDetailsViewModel = value;
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
                }
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

        private async void Initialize()
        {
            List<StaffPlaceholder> staffPlaceholders = (await Database.GetStaffPlaceholders(cafe.CafeID)).ToList();
            await cafe.GetCafeStaffsInfo();
            StaffInfos?.Clear();
            StaffInfos = new ObservableCollection<StaffDetailViewModel>();
            
            if (cafe.Manager != null)
            {
                StaffInfos.Add(new StaffDetailViewModel(cafe.Manager, true));
            }

            foreach (Staff staff in cafe.Staffs.Values)
            {
                StaffInfos.Add(new StaffDetailViewModel(staff));
            }

            foreach (StaffPlaceholder staffPlaceholder in staffPlaceholders)
            {
                StaffInfos.Add(new StaffDetailViewModel(staffPlaceholder));
            }

#if DEBUG
            StaffInfos.Add(new StaffDetailViewModel(new Staff()
            {
                CafeID = cafe.CafeID,
                Name = $"Vu Viet Huy",
                Email = "hduykhang100@gmail.com",
                Phone = "1234567890",
                IsMale = true,
                Birthdate = DateTime.Today
            }, true));

            for (int i = 0; i<10; i++)
            {
                StaffInfos.Add(new StaffDetailViewModel(new Staff()
                {
                    CafeID = cafe.CafeID,
                    Name = $"Huynh Duy Khang {i}",
                    Email = $"hduykhang0{i}@gmail.com",
                    Phone = "1234567890",
                    IsMale = i % 2 == 0,
                    Birthdate = DateTime.Today
                }));
            }

            for (int i = 0; i < 10; i++)
            {
                StaffInfos.Add(new StaffDetailViewModel(new StaffPlaceholder()
                {
                    CafeID = cafe.CafeID,
                    Email = $"hduykhang0{i}@gmail.com",
                }));
            }
#endif
        }
    }
}
