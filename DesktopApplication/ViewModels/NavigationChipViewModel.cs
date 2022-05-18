using CSWBManagementApplication.Resources;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class NavigationChipViewModel : ViewModelBase
    {
        private bool activated;
        public bool Activated
        {
            get { return activated; }
            set
            {
                activated = value;
                if (activated)
                {
                    Foreground = DarkTheme.LinearMain;
                    BorderBrush = Foreground;
                } else
                {
                    Foreground = DarkTheme.SolidLight;
                    BorderBrush = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                }
                OnPropertyChanged();
            }
        }

        private string content;
        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                OnPropertyChanged();
            }
        }

        private PackIconKind icon;
        public PackIconKind Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                OnPropertyChanged();
            }
        }

        private Brush foreground;
        public Brush Foreground
        {
            get { return foreground; }
            set
            {
                foreground = value;
                OnPropertyChanged();
            }
        }

        private Brush borderBrush;
        public Brush BorderBrush
        {
            get { return borderBrush; }
            set
            {
                borderBrush = value;
                OnPropertyChanged();
            }
        }

        private ICommand command;
        public ICommand Command
        {
            get { return command; }
            set
            {
                command = value;
                OnPropertyChanged();
            }
        }

        public NavigationChipViewModel(string content, PackIconKind icon, Brush foreground, ICommand command, bool activated = false)
        {
            Content = content;
            Icon = icon;
            Foreground = foreground;
            Command = command;
            Activated = activated;
        }
    }
}
