using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Models
{
    internal class Staff : User
    {
        public readonly ulong StaffID;

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
        }

        public Staff(string username, string password, ulong staffID, string name, string address) : base(username, password)
        {
            StaffID = staffID;
            _name = name;
            _address = address;
        }

        public ulong Salary()
        {
            throw new NotImplementedException();
        }
    }
}
