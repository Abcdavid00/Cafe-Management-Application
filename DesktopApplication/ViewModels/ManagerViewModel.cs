namespace CSWBManagementApplication.ViewModels
{
    internal class ManagerViewModel : ViewModelBase
    {
        public string ViewName
        {
            get => "ManagerView";
        }

        private MainViewModel mainViewModel;

        public ManagerViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }
    }
}