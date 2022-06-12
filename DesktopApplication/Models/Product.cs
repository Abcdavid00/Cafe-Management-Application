using CSWBManagementApplication.Services;
using Google.Cloud.Firestore;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class Product
    {
        [FirestoreDocumentId]
        public string ProductID { get; set; }

        [FirestoreProperty]
        public string CategoryID { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        [FirestoreProperty]
        public int SPrice { get; set; }

        [FirestoreProperty]
        public int MPrice { get; set; }

        [FirestoreProperty]
        public int LPrice { get; set; }

        public Product() { }

        public async void RemoveProductFromCategory()
        {
            if (!string.IsNullOrEmpty(CategoryID))
            {
                await Database.RemoveProductFromCategoryAsync(ProductID);
                CategoryID = "";
            }
        }
    }
}