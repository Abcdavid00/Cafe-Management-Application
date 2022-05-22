namespace CSWBManagementApplication.ViewModels
{
    internal class StaffViewModel : ViewModelBase
    {
        public string ViewName
        {
            get => "StaffView";
        }

        private MainViewModel mainViewModel;

        public StaffViewModel(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
        }
    }
}