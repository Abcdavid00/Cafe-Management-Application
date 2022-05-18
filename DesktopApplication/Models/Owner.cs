using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class Owner : User
    {
        public Owner()
        {
            isOwner = true;
        }
        
        public Owner(string uid, string email) : base(uid, email)
        {
            isOwner = true;
        }

    }
}
