﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CSWBManagementApplication.ViewModels
{
    internal class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum Privilege
        {
            Owner,
            Manager,
            Staff
        }
    }
}