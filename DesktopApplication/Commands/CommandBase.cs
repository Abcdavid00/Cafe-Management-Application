﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSWBManagementApplication.Commands
{
    internal class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public Action action;

        public CommandBase(Action action)
        {
            this.action = action;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            action.Invoke();
        }
    }
}
