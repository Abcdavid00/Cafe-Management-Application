using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CSWBManagementApplication.Service
{
    internal sealed class MiscFunctions
    {
        /// <summary>
        /// This function cap the value to min and max
        /// </summary>
        /// <param name="value">The value that need to be capped</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <returns>Capped value</returns>
        public static int CappedSetter(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        public class SolidColorPulsar
        {
            private int refreshRate;
            public int RefreshRate
            {
                get { return refreshRate; }
                set { refreshRate = value; }
            }
            
            private int Interval;
            
            private SolidColorBrush color1;
            public SolidColorBrush Color1
            {
                get { return color1; }
                set { color1 = value; }
            }

            private SolidColorBrush color2;
            public SolidColorBrush Color2
            {
                get { return color2; }
                set { color2 = value; }
            }

            private List<SolidColorBrush> colorsList;

            private bool isPulsing;

            public event EventHandler<SolidColorBrush> OnColorChanged;

            private void Initialize()
            {
                Interval = 1000/refreshRate;
                colorsList.Clear();
                
                colorsList = new List<SolidColorBrush>();
                colorsList.Add(color1);
                colorsList.Add(color2);
            }

            public void StartPulsing()
            {
                if (!isPulsing)
                {
                    isPulsing = true;
                    Task.Run(() =>
                    {
                        while (isPulsing)
                        {
                            if (OnColorChanged != null)
                            {
                                OnColorChanged(this, colorBrush1);
                            }
                            System.Threading.Thread.Sleep(RefreshRate);
                            if (OnColorChanged != null)
                            {
                                OnColorChanged(this, colorBrush2);
                            }
                            System.Threading.Thread.Sleep(RefreshRate);
                        }
                    });
                }
            }

        }
    }
}
