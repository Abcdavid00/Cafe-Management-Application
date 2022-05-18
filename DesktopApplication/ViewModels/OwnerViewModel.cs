﻿using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CSWBManagementApplication.Service;
using System.Windows.Input;
using CSWBManagementApplication.Commands;

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
                new NavigationChipViewModel("Home", PackIconKind.Home, new SolidColorBrush(Color.FromArgb(255,255,69,0)),
                                            new Commands.CommandBase(()=>SelectedIndex=0), true),
                new NavigationChipViewModel("Cafes", PackIconKind.Coffee, new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
                                            new Commands.CommandBase(()=>SelectedIndex=1), false),
                new NavigationChipViewModel("Staff", PackIconKind.Person, new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
                                            new Commands.CommandBase(()=>SelectedIndex=2), false),
                new NavigationChipViewModel("Products", PackIconKind.Silverware, new SolidColorBrush(Color.FromArgb(255, 245, 245, 245)),
                                            new Commands.CommandBase(()=>SelectedIndex=3), false),
            };
        }
    }
}
