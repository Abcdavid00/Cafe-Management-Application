using CSWBManagementApplication.Models;
using CSWBManagementApplication.Resources;
using System;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class StaffDetailsViewModel : ViewModelBase
    {
        private bool isManager;

        public bool IsManager
        {
            get => isManager;
        }

        public Brush Background
        {
            get => (isManager ? DarkTheme.LinearMain : DarkTheme.SolidLight);
        }

        public Brush Foreground
        {
            get => (isManager ? DarkTheme.SolidLight : DarkTheme.SolidDark);
        }

        public string Name
        {
            get => (!IsPlaceholder ? staff.Name : "---");
        }

        public string Email
        {
            get => (!isEmpty ? "---" : (!IsPlaceholder ? staff.Email : staffPlaceholder.Email));
        }

        public string Phone
        {
            get => (!IsPlaceholder ? staff.Phone : "---");
        }

        public string Sex
        {
            get => (!IsPlaceholder ? (staff.IsMale ? "Male" : "Female") : "---");
        }

        public string Birthdate
        {
            get => (!IsPlaceholder ? Staff.Birthdate.ToString("yyyy-MM-dd") : "---");
        }

        private Staff staff;

        public Staff Staff
        {
            get => staff;
            private set
            {
                staffPlaceholder = null;
                staff = value;
                OnPropertyChanged(nameof(Background));
                OnPropertyChanged(nameof(Foreground));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Phone));
                OnPropertyChanged(nameof(Sex));
                OnPropertyChanged(nameof(Birthdate));
            }
        }

        public bool IsPlaceholder
        {
            get => (staff == null);
        }

        private bool isEmpty;

        public bool IsEmpty
        {
            get => isEmpty;
        }

        private StaffPlaceholder staffPlaceholder;

        public StaffPlaceholder StaffPlaceholder
        {
            get => staffPlaceholder;
            private set
            {
                staff = null;
                staffPlaceholder = value;
                OnPropertyChanged(nameof(Background));
                OnPropertyChanged(nameof(Foreground));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Email));
                OnPropertyChanged(nameof(Phone));
                OnPropertyChanged(nameof(Sex));
                OnPropertyChanged(nameof(Birthdate));
            }
        }

        public event EventHandler OnInfoUpdate;

        public void UpdateInfo(Staff staff, bool isMananger = false)
        {
            if (this.Staff != staff)
            {
                this.Staff = staff;
                staffPlaceholder = null;
                isEmpty = false;
                this.isManager = isMananger;
                OnInfoUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateInfo(StaffPlaceholder staffPlaceholder)
        {
            if (this.StaffPlaceholder != staffPlaceholder)
            {
                this.StaffPlaceholder = staffPlaceholder;
                this.Staff = null;
                isEmpty = false;
                this.isManager = false;
                OnInfoUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Clear()
        {
            if (this.StaffPlaceholder != null || this.Staff != null)
            {
                this.Staff = null;
                this.StaffPlaceholder = null;
                isEmpty = true;
                this.isManager = false;
                OnInfoUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        public StaffDetailsViewModel()
        {
            isEmpty = true;
        }

        public StaffDetailsViewModel(Staff staff, bool isManager = false)
        {
            UpdateInfo(staff, isManager);
        }

        public StaffDetailsViewModel(StaffPlaceholder staffPlaceholder)
        {
            UpdateInfo(staffPlaceholder);
        }
    }
}