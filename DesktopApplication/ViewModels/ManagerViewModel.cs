using CSWBManagementApplication.Commands;
using CSWBManagementApplication.Models;
using CSWBManagementApplication.Services;
using System.Windows.Input;

namespace CSWBManagementApplication.ViewModels
{
    internal class ManagerViewModel : ViewModelBase
    {

        private MainViewModel mainViewModel;

        private CafeDetailsViewModel cafeDetailsViewModel;

        public CafeDetailsViewModel CafeDetailsViewModel
        {
            get => cafeDetailsViewModel;
            set
            {
                cafeDetailsViewModel = value;
                OnPropertyChanged();
            }
        }


        public ManagerViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            LoadCafeDetails();
        }

        private async void LoadCafeDetails()
        {
            Cafe cafe = await Database.GetCafe((mainViewModel.CurrentUser as Staff).CafeID);
            CafeDetailsViewModel = new CafeDetailsViewModel(cafe, Privilege.Manager);
        }

        public ICommand LogoutCommand
        {
            get => new CommandBase(() =>
            {
                mainViewModel.UserLink = null;
            });
        }
    }
}