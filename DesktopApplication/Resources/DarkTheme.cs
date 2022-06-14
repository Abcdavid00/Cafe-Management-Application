using System.Windows;
using System.Windows.Media;

namespace CSWBManagementApplication.Resources
{
    internal static class DarkTheme
    {
        public static readonly Brush LinearMain = new LinearGradientBrush(Color.FromRgb(255, 161, 19),
                                                                          Color.FromRgb(255, 0, 0), 0);

        public static readonly Brush LinearPrimary = new LinearGradientBrush(Color.FromRgb(19, 200, 19),
                                                                              Color.FromRgb(200, 200, 0), 0);

        public static readonly Brush LinearBackground = new LinearGradientBrush(Color.FromRgb(108, 112, 118),
                                                                                Color.FromRgb(33, 36, 41), 90);

        public static readonly Brush LinearLightBackground = new LinearGradientBrush(Color.FromRgb(56, 61, 70),
                                                                                     Color.FromRgb(108, 112, 118), 90);

        public static readonly Brush LinearDarkBackground = new LinearGradientBrush(Color.FromRgb(49, 54, 58),
                                                                                    Color.FromArgb(51, 19, 23, 26), 90);

        public static readonly Brush SolidLight = new SolidColorBrush(Color.FromRgb(245, 245, 245));

        public static readonly Brush SolidMain = new SolidColorBrush(Color.FromRgb(255, 69, 0));

        public static readonly Brush SolidDark = new SolidColorBrush(Color.FromRgb(33, 36, 42));

        public static void SetTheme(Application app)
        {
            foreach (var Property in typeof(DarkTheme).GetProperties())
            {
                if (Property.PropertyType == typeof(Brush))
                {
                    app.Resources[Property.Name] = Property.GetValue(null);
                }
            }
        }
    }
}