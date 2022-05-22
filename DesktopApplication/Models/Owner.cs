using Google.Cloud.Firestore;

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