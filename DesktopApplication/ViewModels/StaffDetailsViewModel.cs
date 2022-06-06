using CSWBManagementApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.ViewModels
{
    internal class StaffDetailsViewModel : ViewModelBase
    {
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
            get => phone;
            set
            {
                phone = value;
                OnPropertyChanged();
            }
        }

        private bool isMale;
        public bool IsMale
        {
            get => isMale;
            set
            {
                isMale = value;
                OnPropertyChanged(nameof(Sex));
                OnPropertyChanged();
            }
        }

        public string Sex
        {
            get => (isMale ? "Male" : "FeMale");
        }

        private DateTime birthDate;
        public DateTime BirthDate
        {
            get => birthDate;
            set
            {
                birthDate = value;
                OnPropertyChanged(nameof(BirthDateText));
                OnPropertyChanged();
            }
        }
        public string BirthDateText
        {
            get => birthDate.ToString("yyyy-MM-dd");
        }

        public void UpdateInfo(Staff staff)
        {
            Name = staff.Name;
            Email = staff.Email;
            Phone = staff.Phone;
            IsMale = staff.IsMale;
            BirthDate = staff.Birthdate;
        }

        public StaffDetailsViewModel(Staff staff)
        {
            UpdateInfo(staff);
        }

    }
}
