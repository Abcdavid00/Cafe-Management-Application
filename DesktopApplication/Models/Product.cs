using CSWBManagementApplication.Services;
using Google.Cloud.Firestore;
using System;

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

        public async void UpdateProductInfo(string name, int sPrice, int mPrice, int lPrice)
        {
            if (!string.IsNullOrEmpty(name) && sPrice >= 0 && mPrice >= 0 && lPrice >= 0)
            {
                this.Name = name;
                this.SPrice = sPrice;
                this.MPrice = mPrice;
                this.LPrice = lPrice;
                await Database.UpdateProductAsync(this);
                ProductInfoChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler ProductInfoChanged;
    }
}