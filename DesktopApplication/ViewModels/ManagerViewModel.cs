using CSWBManagementApplication.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
