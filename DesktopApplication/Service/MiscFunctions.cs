using System;
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

        public static string IntToPrice(int p)
        {
            if (p == 0)
            {
                return "---";
            }
            return p.ToString("n0") + "VND";
        }

        public static void ValidateDate(ref int d, int m, int y)
        {
            switch (m)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    d = CappedSetter(d, 1, 31);
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    d = CappedSetter(d, 1, 30);
                    break;
                case 2:
                    if (y % 4 == 0)
                    {
                        d = CappedSetter(d, 1, 29);
                    }
                    else
                    {
                        d = CappedSetter(d, 1, 28);
                    }
                    break;
                default:
                    break;
            }
        }

        public static long MinDate(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,0,0,0).ToBinary();
        }

        public static long MaxDate(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59).ToBinary();
        }

        public class SolidColorPulsar
        {
            private int refreshRate;

            public int RefreshRate
            {
                get { return refreshRate; }
                set
                {
                    if (refreshRate == value || value < 1)
                    {
                        return;
                    }
                    refreshRate = value;
                    Initialize();
                }
            }

            private int duration;

            public int Duration
            {
                get { return duration; }
                set
                {
                    if (duration == value || value < 1)
                    {
                        return;
                    }
                    duration = value;
                    Initialize();
                }
            }

            private int interval;

            private int stepsCount;

            private double rStep;
            private double gStep;
            private double bStep;

            private Color color1;

            public Color Color1
            {
                get { return color1; }
                set
                {
                    if (color1 == value)
                    {
                        return;
                    }
                    color1 = value;
                    Initialize();
                }
            }

            private Color color2;

            public Color Color2
            {
                get { return color2; }
                set
                {
                    if (color2 == value)
                    {
                        return;
                    }
                    color2 = value;
                    Initialize();
                }
            }

            private SolidColorBrush currentColor;

            public SolidColorBrush CurrentColor
            {
                get => currentColor;
                private set
                {
                    currentColor = value;
                    OnColorChanged?.Invoke(this, currentColor);
                }
            }

            private bool isPulsing;

            public event EventHandler<SolidColorBrush> OnColorChanged;

            public SolidColorPulsar(Color color1, Color color2, int refreshRate, int duration)
            {
                this.color1 = color1;
                this.color2 = color2;
                this.refreshRate = refreshRate;
                this.duration = duration;
                Initialize();
            }

            private void Initialize()
            {
                isPulsing = false;
                interval = 1000 / refreshRate;
                stepsCount = duration / interval;
                rStep = ((double)(color2.R - color1.R)) / (double)stepsCount;
                gStep = ((double)(color2.G - color1.G)) / (double)stepsCount;
                bStep = ((double)(color2.B - color1.B)) / (double)stepsCount;
                CurrentColor = new SolidColorBrush(color1);
            }

            public void StartPulsing()
            {
                if (!isPulsing)
                {
                    isPulsing = true;
                    Pulse();
                }
            }

            public void StopPulsing()
            {
                isPulsing = false;
            }

            private async Task Pulse()
            {
                while (isPulsing)
                {
                    for (int i = 0; i < stepsCount; i++)
                    {
                        if (!isPulsing)
                        {
                            break;
                        }
                        await Task.Delay(interval);
                        CurrentColor = new SolidColorBrush(new Color()
                        {
                            A = color1.A,
                            R = (byte)(color1.R + rStep * i),
                            G = (byte)(color1.G + gStep * i),
                            B = (byte)(color1.B + bStep * i)
                        });
                    }

                    for (int i = 0; i < stepsCount; i++)
                    {
                        if (!isPulsing)
                        {
                            break;
                        }
                        await Task.Delay(interval);
                        CurrentColor = new SolidColorBrush(new Color()
                        {
                            A = color2.A,
                            R = (byte)(color2.R - rStep * i),
                            G = (byte)(color2.G - gStep * i),
                            B = (byte)(color2.B - bStep * i)
                        });
                    }
                }
            }
        }

        
}
}