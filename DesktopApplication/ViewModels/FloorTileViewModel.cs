using CSWBManagementApplication.Resources;
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
                if (isPulsing)
                {
                    return Pulsar.CurrentColor;
                }
                if (HasTile)
                {
                    return DarkTheme.LinearLightBackground;
                }
                return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
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
                    return DarkTheme.LinearMain;
                }
                return new SolidColorBrush(Colors.Transparent);
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
            private set
            {
                hasBorder = value;
                OnPropertyChanged();
            }
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

        private bool isPulsing;

        private SolidColorPulsar pulsar;

        private SolidColorPulsar Pulsar
        {
            get
            {
                if (pulsar == null)
                {
                    pulsar = new SolidColorPulsar((DarkTheme.SolidDark as SolidColorBrush).Color, (DarkTheme.SolidLight as SolidColorBrush).Color, 60, 1000);
                    pulsar.OnColorChanged += (object sender, SolidColorBrush e) =>
                    {
                        OnPropertyChanged(nameof(OuterBackground));
                    };
                }
                return pulsar;
            }
        }

        public void StartPulsing()
        {
            if (!isPulsing)
            {
                isPulsing = true;
                Pulsar.StartPulsing();
            }
        }

        public void StopPulsing()
        {
            if (isPulsing)
            {
                isPulsing = false;
                Pulsar.StopPulsing();
            }
        }

        public FloorTileViewModel(bool hasTile, bool hasTable, bool hasBorder, ICommand command)
        {
            HasTile = hasTile;
            HasTable = hasTable;
            HasBorder = hasBorder;
            Command = command;
        }

        public FloorTileViewModel(bool hasTile, bool hasTable, bool hasBorder = false)
        {
            HasTile = hasTile;
            HasTable = hasTable;
            HasBorder = hasBorder;
            Command = null;
        }

        public FloorTileViewModel(ICommand command)
        {
            HasTile = false;
            HasTable = false;
            HasBorder = true;
            Command = command;
        }
    }
}