﻿using CSWBManagementApplication.Services;
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

        public async void UpdateCategory()
        {
            await Database.UpdateCategoryAsync(this);
        }

        public event EventHandler ProductListUpdated;

        private List<Product> products;
        public List<Product> Products
        {
            get => products;
            private set
            {
                products = value;
                ProductListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
