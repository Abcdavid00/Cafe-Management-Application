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
            get => ((!IsPlaceholder && !IsEmpty) ? staff.Name : "---");
        }

        public string Email
        {
            get => (IsEmpty ? "---" : (!IsPlaceholder ? staff.Email : staffPlaceholder.Email));
        }

        public string Role
        {
            get => (!IsPlaceholder ? (IsManager ? "Manager" : "Staff") : "Placeholder");
        }

        public string Phone
        {
            get => ((!IsPlaceholder && !IsEmpty) ? staff.Phone : "---");
        }

        public string Sex
        {
            get => ((!IsPlaceholder && !IsEmpty) ? (staff.IsMale ? "Male" : "Female") : "---");
        }

        public string Birthdate
        {
            get => ((!IsPlaceholder && !IsEmpty) ? Staff.Birthdate.ToString("yyyy-MM-dd") : "---");
        }

        private Staff staff;

        public Staff Staff
        {
            get => staff;
            private set
            {
                staff = value;          
                Refresh();
            }
        }

        public bool IsPlaceholder
        {
            get => (staff == null && staffPlaceholder != null);
        }

        public bool IsEmpty
        {
            get => (staff == null && staffPlaceholder == null);
        }

        private StaffPlaceholder staffPlaceholder;

        public StaffPlaceholder StaffPlaceholder
        {
            get => staffPlaceholder;
            private set
            {
                staffPlaceholder = value;            
                Refresh();
            }
        }

        private void Refresh()
        {
            OnPropertyChanged(nameof(Background));
            OnPropertyChanged(nameof(Foreground));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Role));
            OnPropertyChanged(nameof(Phone));
            OnPropertyChanged(nameof(Sex));
            OnPropertyChanged(nameof(Birthdate));
        }

        public event EventHandler OnInfoUpdate;

        public void UpdateInfo(Staff staff, bool isMananger = false)
        {
            if (this.Staff != staff)
            {
                this.isManager = isMananger;
                this.Staff = staff;
                this.staffPlaceholder = null;
                //isEmpty = false;            
                OnInfoUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateInfo(StaffPlaceholder staffPlaceholder)
        {
            if (this.StaffPlaceholder != staffPlaceholder)
            {
                this.isManager = false;
                this.StaffPlaceholder = staffPlaceholder;
                this.Staff = null;
                               
                OnInfoUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Clear()
        {
            if (this.StaffPlaceholder != null || this.Staff != null)
            {
                this.Staff = null;
                this.StaffPlaceholder = null;
                
                this.isManager = false;
                OnInfoUpdate?.Invoke(this, EventArgs.Empty);
            }
        }

        public StaffDetailsViewModel()
        {
            
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