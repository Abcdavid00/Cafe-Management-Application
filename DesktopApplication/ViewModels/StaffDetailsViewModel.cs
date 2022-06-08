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
            get => (!isPlaceholder ? staff.Name : "---");
        }

        public string Email
        {
            get => (isEmpty ? "---" : (!isPlaceholder ? staff.Email : staffPlaceholder.Email));
        }

        public string Phone
        {
            get => (!isPlaceholder ? staff.Phone : "---");
        }

        public string Sex
        {
            get => (!isPlaceholder ? (staff.IsMale ? "Male" : "Female") : "---");
        }

        public string Birthdate
        {
            get => (isPlaceholder ? Staff.Birthdate.ToString("yyyy-MM-dd") : "---");
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

        public bool isPlaceholder
        {
            get => (staff == null);
        }

        private bool isEmpty;

        public bool IsEmpty
        {
            get => isEmpty;
        }

        private StaffPlaceholder staffPlaceholder;

        public StaffPlaceholder StaffPlaceHolder
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
            if (this.StaffPlaceHolder != staffPlaceholder)
            {
                this.StaffPlaceHolder = staffPlaceholder;
                this.Staff = null;
                isEmpty = false;
                this.isManager = false;
                OnInfoUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateInfo()
        {
            if (this.StaffPlaceHolder != null || this.Staff != null)
            {
                this.Staff = null;
                this.StaffPlaceHolder = null;
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