using CSWBManagementApplication.ViewModels;
using System;

namespace CSWBManagementApplication.Service
{
    internal static class MainNavigation
    {
        public static event EventHandler MainNavigationEvent;

        private static ViewModelBase currentViewModel;
    }
}