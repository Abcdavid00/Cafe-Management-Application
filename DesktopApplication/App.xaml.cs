using CSWBManagementApplication.Resources;
using CSWBManagementApplication.ViewModels;
using System.Windows;

namespace CSWBManagementApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            DarkTheme.SetTheme(this);
            this.Resources["MediumFontSize"] = screenWidth * 0.01 + 1.12;

            MainWindow = new MainWindow()
            {
                DataContext = new MainViewModel()
            };

            MainWindow.Show();

            base.OnStartup(e);
        }
    }
}