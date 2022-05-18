using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
