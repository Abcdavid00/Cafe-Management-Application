using Google.Cloud.Firestore;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class StaffPlaceholder
    {
        private string placeHolderID;

        [FirestoreDocumentId]
        public string PlaceHolderID
        {
            get => placeHolderID;
            set => placeHolderID = value;
        }

        private string cafeID;

        [FirestoreProperty]
        public string CafeID
        {
            get => cafeID;
            set => cafeID = value;
        }

        private string email;

        [FirestoreProperty]
        public string Email
        {
            get => email;
            set => email = value;
        }

        public StaffPlaceholder()
        {
        }
    }
}