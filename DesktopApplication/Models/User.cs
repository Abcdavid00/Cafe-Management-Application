using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Models
{
    internal abstract class User
    {
        private string _username;
        public string UserName
        {
            get { return _username; }
        }

        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private string _password;

        private void ChangeUsername()
        {
            throw new NotImplementedException();
        }

        private void ChangePassword()
        {
            throw new NotImplementedException();
        }
    }
}
