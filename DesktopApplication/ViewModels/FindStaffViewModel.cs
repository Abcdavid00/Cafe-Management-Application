using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class FindStaffItemViewModel : ViewModelBase
    {
        public Staff staff;

        public string Name
        {
            get => staff.Name;
        }

        public string Email
        {
            get => staff.Email;
        }

        public string State
        {
            get => (string.IsNullOrEmpty(staff.CafeID) ? "Unassigned" : "Assigned");
        }

        public Brush StateColor
        {
            get => (string.IsNullOrEmpty(staff.CafeID) ?
                new SolidColorBrush(Colors.Green) :
                new SolidColorBrush(Colors.Red));
        }

        public Visibility AddButtonVisibility
        {
            get => (string.IsNullOrEmpty(staff.CafeID) ? Visibility.Visible : Visibility.Collapsed);
        }

        public ICommand AddCommand
        {
            get => new CommandBase(() =>
            {
                if (string.IsNullOrEmpty(staff.CafeID))
                {
                    destinationCafe.AddStaff(staff);
                }
            });
        }

        private Cafe destinationCafe;

        public FindStaffItemViewModel(Staff staff, Cafe cafe)
        {
            this.staff = staff;
            this.destinationCafe = cafe;
        }
    }

    internal class FindStaffViewModel : ViewModelBase
    {
        public Cafe cafe;

        private ObservableCollection<FindStaffItemViewModel> displayStaffs;

        public ObservableCollection<FindStaffItemViewModel> DisplayStaffs
        {
            get => displayStaffs;
            set
            {
                displayStaffs = value;
                OnPropertyChanged();
            }
        }

        private List<FindStaffItemViewModel> FullStaffList;

        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
                filter();
            }
        }

        public FindStaffViewModel(Cafe cafe)
        {
            this.cafe = cafe;
            RefreshList();
        }

        public ICommand RefreshCommand
        {
            get => new CommandBase(() => { RefreshList(); });
        }

        public async void RefreshList()
        {
            FullStaffList = (from staff in ((await Database.GetAllStaffsAsync()).ToList())
                             select new FindStaffItemViewModel(staff, cafe)).ToList();
            filter();
        }

        private void filter()
        {
            DisplayStaffs?.Clear();
            if (string.IsNullOrEmpty(searchText))
            {
                DisplayStaffs = new ObservableCollection<FindStaffItemViewModel>(FullStaffList);
            }
            else
            {
                DisplayStaffs = new ObservableCollection<FindStaffItemViewModel>(FullStaffList.Where(x => x.Email.Contains(searchText)));
            }
            AddStaffPlaceholderButtonVisibility = ((FullStaffList.Where(x => x.Email == SearchText).Count() == 0 &&
                !string.IsNullOrEmpty(searchText)) ?
                Visibility.Visible :
                Visibility.Collapsed);
        }

        private Visibility addStaffPlaceholderButtonVisibility;

        public Visibility AddStaffPlaceholderButtonVisibility
        {
            get => addStaffPlaceholderButtonVisibility;
            private set
            {
                addStaffPlaceholderButtonVisibility = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddStaffPlaceholderCommand
        {
            get => new CommandBase(() =>
            {
                cafe.AddStaffPlaceholder(SearchText);
            });
        }
    }
}