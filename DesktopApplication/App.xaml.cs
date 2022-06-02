﻿using CSWBManagementApplication.Resources;
using CSWBManagementApplication.ViewModels;
using System.Windows;

namespace CSWBManagementApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const int CAFE_CARD_PER_ROW = 5;
        public const int FLOOR_TILES_PER_FLOOR_LINE = 15;

        protected override void OnStartup(StartupEventArgs e)
        {
            SetDynamicResources();

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel()
            };

            MainWindow.Show();

            base.OnStartup(e);
        }

        private void SetDynamicResources()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double ownerViewMainAreaWidth = screenWidth * 7d / 8d;

            DarkTheme.SetTheme(this);
            this.Resources["MediumFontSize"] = screenWidth * 0.01 + 1.12;

            double cafeCardWidth = ((ownerViewMainAreaWidth * 23 / 25) - (double)CAFE_CARD_PER_ROW * 10) / (double)CAFE_CARD_PER_ROW;
            this.Resources["CafeCardWidth"] = cafeCardWidth;
            this.Resources["CafeCardHeight"] = cafeCardWidth * 1.3;

            this.Resources["FloorTilesPerFloorLine"] = FLOOR_TILES_PER_FLOOR_LINE;
        }
    }
}