using CSWBManagementApplication.Resources;
using MaterialDesignThemes.Wpf;
using System.Windows;
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
                }
                else
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

        private Visibility visibility;
        public Visibility Visibility
        {
            get => visibility;
            set
            {
                visibility = value;
                OnPropertyChanged();
            }
        }

        public NavigationChipViewModel(string content, PackIconKind icon, ICommand command, bool activated, bool isVisible=true)
        {
            Content = content;
            Icon = icon;
            Command = command;
            Activated = activated;
            Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}