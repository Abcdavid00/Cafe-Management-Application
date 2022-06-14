using CSWBManagementApplication.Resources;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static CSWBManagementApplication.Service.MiscFunctions;

namespace CSWBManagementApplication.ViewModels
{
    internal class FloorTileViewModel : ViewModelBase
    {
        public Brush OuterBackground
        {
            get
            {
                if (HasTile)
                {
                    return DarkTheme.LinearLightBackground;
                }
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Brush OuterBorder
        {
            get
            {
                if (HasBorder)
                {
                    return DarkTheme.SolidLight;
                }
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public Brush InnerBackground
        {
            get
            {
                if (HasTable)
                {
                    if (Activated)
                    {
                        return DarkTheme.SolidMain;
                    } else
                    {
                        return DarkTheme.SolidLight;
                    }
                }
                return new SolidColorBrush(Colors.Transparent);
            }
        }
        
        public Brush Foreground
        {
            get => (Activated ? DarkTheme.SolidLight : DarkTheme.SolidDark);
        }

        private bool activated;
        public bool Activated
        {
            get => activated;
            set
            {
                activated = value;
                OnPropertyChanged(nameof(InnerBackground));
                OnPropertyChanged(nameof(Foreground));
                OnPropertyChanged();
            }
        }

        private bool hasTable;

        public bool HasTable
        {
            get => hasTable;
            set
            {
                if (value == hasTable)
                {
                    return;
                }
                hasTable = value;
                OnPropertyChanged(nameof(InnerBackground));
                OnPropertyChanged();
            }
        }

        private bool hasTile;

        public bool HasTile
        {
            get => hasTile;
            set
            {
                if (value == hasTile)
                {
                    return;
                }
                hasTile = value;
                if (!hasTile)
                {
                    HasTable = false;
                }
                OnPropertyChanged(nameof(OuterBackground));
                OnPropertyChanged();
            }
        }

        private bool hasBorder;

        public bool HasBorder
        {
            get => hasBorder;
            set
            {
                hasBorder = value;
                OnPropertyChanged(nameof(OuterBorder));
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

        public Visibility Visibility
        {
            get => (HasBorder || HasTable) ? Visibility.Visible : Visibility.Collapsed;
        }

        private ICommand command;

        public ICommand Command
        {
            get => command;
            set
            {
                command = value;
                OnPropertyChanged();
            }
        }

        public FloorTileViewModel(bool hasTile, bool hasTable, bool hasBorder, ICommand command,string content, bool activated = false)
        {
            HasTile = hasTile;
            HasTable = hasTable;
            HasBorder = hasBorder;
            Content = content;
            Command = command;
            Activated = activated;
        }

        public FloorTileViewModel()
        {
            HasTile = false;
            HasTable = false;
            HasBorder = false;
            Command = null;
            Activated = false;
        }

        public FloorTileViewModel(bool hasTile, bool hasTable, bool hasBorder = false)
        {
            HasTile = hasTile;
            HasTable = hasTable;
            HasBorder = hasBorder;
            Command = null;
            Activated = false;
        }

        public FloorTileViewModel(ICommand command)
        {
            HasTile = false;
            HasTable = false;
            HasBorder = true;
            Command = command;
            Activated = false;
        }
    }
}