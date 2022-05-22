using CSWBManagementApplication.Services;
using Google.Cloud.Firestore;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal abstract class User
    {
        public enum Roles
        {
            None = 0,
            Owner = 1,
            Manager = 2,
            Staff = 3
        }

        protected string uid;

        [FirestoreDocumentId]
        public string UID
        {
            get { return uid; }
            set { uid = value; }
        }

        protected string email;

        [FirestoreProperty]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        protected bool isOwner;

        [FirestoreProperty]
        public bool IsOwner
        {
            get { return isOwner; }
            set { isOwner = value; }
        }

        protected User()
        {
            this.uid = "";
            this.email = "";
        }

        protected User(string uid, string mail)
        {
            this.uid = uid;
            this.email = mail;
        }

        public DocumentReference UserReference
        {
            get => Database.UserDocument(UID);
        }

        public Roles Role
        {
            get => (isOwner ? Roles.Owner : Database.UserRole(UserReference).Result);
        }
    }
}