using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

        public ICommand AddCommmand
        {
            get => new CommandBase(() =>
            {
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

        private async void RefreshList()
        {
            FullStaffList = (from staff in ((await Database.GetAllStaffsExclude(cafe.CafeID)).ToList())
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
                string filter = searchText.Trim().ToLower();
                DisplayStaffs = new ObservableCollection<FindStaffItemViewModel>(FullStaffList.Where(x => x.Email.ToLower().Contains(filter)));
            }
            AddStaffPlaceholderButtonVisibility = (DisplayStaffs.Count == 0 ? Visibility.Visible : Visibility.Collapsed);
            
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