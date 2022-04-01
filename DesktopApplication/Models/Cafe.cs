using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Models
{
    internal class Cafe
    {
        readonly ulong CafeID;

        readonly string Address;

        private int _tableCount;
        public int TableCount
        {
            get { return _tableCount; }
        }


    }
}
