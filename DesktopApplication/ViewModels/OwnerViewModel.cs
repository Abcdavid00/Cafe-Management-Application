using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Service;
using CSWBManagementApplication.Services;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class OwnerViewModel : ViewModelBase
    {
        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex == value)
                {
                    return;
                }
                NavigationChips[selectedIndex].Activated = false;
                selectedIndex = MiscFunctions.CappedSetter(value, 0, 3);
                NavigationChips[selectedIndex].Activated = true;
                OnPropertyChanged();
            }
        }

        private List<NavigationChipViewModel> navigationChips;

        public List<NavigationChipViewModel> NavigationChips
        {
            get => navigationChips;
            private set
            {
                navigationChips = value;
                OnPropertyChanged();
            }
        }

        private ICommand logoutCommand;

        public ICommand LogoutCommand
        {
            get => logoutCommand;
            private set
            {
                logoutCommand = value;
                OnPropertyChanged();
            }
        }

        private MainViewModel mainViewModel;

        public OwnerViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            LogoutCommand = new CommandBase(() =>
            {
                mainViewModel.UserLink = null;
            });
            NavigationChips = new List<NavigationChipViewModel>
            {
                new NavigationChipViewModel("Home", PackIconKind.Home, new Commands.CommandBase(()=>SelectedIndex=0), true),
                new NavigationChipViewModel("Cafes", PackIconKind.Coffee, new Commands.CommandBase(()=>SelectedIndex=1), false),
                new NavigationChipViewModel("Staff", PackIconKind.Person, new Commands.CommandBase(()=>SelectedIndex=2), false),
                new NavigationChipViewModel("Products", PackIconKind.Silverware, new Commands.CommandBase(()=>SelectedIndex=3), false),
            };
            Initialize();
        }

        private async void Initialize()
        {
            List<Cafe> cafes = (await Database.GetAllCafes()).ToList();
            fullCafesList = new List<CafeCardViewModel>();
            cafesList = new List<CafeCardViewModel>();
            foreach (Cafe cafe in cafes)
            {
                CafeCardViewModel cafeCardViewModel = new CafeCardViewModel(cafe);
                cafeCardViewModel.PressCommand = new CommandBase(() =>
                {
                    this.CafeDetailsViewModel = new CafeDetailsViewModel(cafe);
                });
                fullCafesList.Add(cafeCardViewModel);
            }
            SearchText = "";
        }

        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                searchText = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(searchText))
                {
                    CafesList.Clear();
                    CafesList = new List<CafeCardViewModel>(fullCafesList.Where(c => c.Address.ToLower().Contains(searchText.ToLower())).OrderBy(c => c.Address));
                }
                else
                {
                    CafesList.Clear();
                    CafesList = new List<CafeCardViewModel>(fullCafesList);
                }
            }
        }

        private List<CafeCardViewModel> fullCafesList;

        private List<CafeCardViewModel> cafesList;

        public List<CafeCardViewModel> CafesList
        {
            get => cafesList;
            set
            {
                cafesList = value;
                OnPropertyChanged();
            }
        }

        #region CafeView

        private int cafeViewIndex;

        public int CafeViewIndex
        {
            get => cafeViewIndex;
            set
            {
                cafeViewIndex = value;
                OnPropertyChanged();
            }
        }

        private CafeDetailsViewModel cafeDetailsViewModel;

        public CafeDetailsViewModel CafeDetailsViewModel
        {
            get => cafeDetailsViewModel;
            set
            {
                cafeDetailsViewModel = value;
                OnPropertyChanged();
                if (cafeDetailsViewModel != null)
                {
                    CafeViewIndex = 1;
                }
                else
                {
                    CafeViewIndex = 0;
                }
            }
        }

        #endregion CafeView
    }
}