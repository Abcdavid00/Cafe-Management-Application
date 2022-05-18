using CSWBManagementApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Service
{
    internal static class MainNavigation
    {
        
        public static event EventHandler MainNavigationEvent;

        private static ViewModelBase currentViewModel;
        
    }
}
