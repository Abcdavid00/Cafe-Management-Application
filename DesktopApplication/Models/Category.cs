using CSWBManagementApplication.Services;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class Category
    {
        [FirestoreDocumentId]
        public string CategoryID { get; set; }

        [FirestoreProperty]
        public string Name { get; set; }

        public Category()
        {
            
        }

        public async void UpdateCategoryName(string name)
        {
            Name = name;
            await Database.UpdateCategoryAsync(this);
            CategoryNameUpdated?.Invoke(this, Name);
        }

        public event EventHandler<string> CategoryNameUpdated;
        
        public event EventHandler ProductListUpdated;

        private List<Product> products;
        public List<Product> Products
        {
            get => products;
            set
            {
                products = value;
                ProductListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public async void GetProducts()
        {
            Products?.Clear();
            Products = (await Database.GetProductsAsync(CategoryID)).ToList();
        }
        
        public async Task AddProduct(Product product)
        {
            await Database.AddProductToCategoryAsync(CategoryID, product.ProductID);
            GetProducts();
        }

        public async Task RemoveProduct(Product product)
        {
            await Database.RemoveProductFromCategoryAsync(product.ProductID);
            GetProducts();
        }
    }
}
