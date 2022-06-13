using CSWBManagementApplication.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class FloorButtonViewModel : ViewModelBase
    {
        private bool isActive;

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                OnPropertyChanged(nameof(Background));
                OnPropertyChanged(nameof(Foreground));
                OnPropertyChanged();
            }
        }

        public Brush Foreground
        {
            get => IsActive ? DarkTheme.SolidDark : DarkTheme.SolidMain;
        }

        public Brush Background
        {
            get => IsActive ? DarkTheme.SolidMain : new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        private ICommand command;

        public ICommand Command
        {
            get => command;
            private set
            {
                command = value;
                OnPropertyChanged();
            }
        }

        private string content;

        public string Content
        {
            get => content;
            set
            {
                content = value;
                OnPropertyChanged();
            }
        }

        private string toolTip;

        public string ToolTip
        {
            get => toolTip;
            set
            {
                toolTip = value;
                OnPropertyChanged();
            }
        }

        public FloorButtonViewModel(string content, string toolTip, ICommand command, bool isActive = false)
        {
            this.content = content;
            this.toolTip = toolTip;
            this.command = command;
            this.isActive = isActive;
        }
    }
}
