using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
