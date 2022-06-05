using CSWBManagementApplication.Resources;
using System.Windows.Media;

namespace CSWBManagementApplication.ViewModels
{
    internal class ViewOnlyFloorTileViewModel : ViewModelBase
    {
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

        public ViewOnlyFloorTileViewModel(bool hasTable = false)
        {
            HasTable = hasTable;
        }
    }
}