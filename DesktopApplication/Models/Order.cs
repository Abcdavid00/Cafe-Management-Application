using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;

namespace CSWBManagementApplication.Models
{
    [FirestoreData]
    internal class OrderedProduct
    {
        [FirestoreProperty]
        public string ProductID { get; set; }

        [FirestoreProperty]
        public int Size { get; set; }

        public OrderedProduct()
        {
            ProductID = "";
            Size = 0;
        }

        public OrderedProduct(string productID, int size)
        {
            ProductID = productID;
            Size = size;
        }

        public static bool operator==(OrderedProduct a, OrderedProduct b)
        {
            return a.ProductID == b.ProductID && a.Size == b.Size;
        }

        public static bool operator!=(OrderedProduct a, OrderedProduct b)
        {
            return a.ProductID != b.ProductID || a.Size != b.Size;
        }
    }

    internal class NestedOrderedProduct
    {
        public string ProductID { get; set; }
        
        public int Size { get; set; }

        public int Count { get; set; }

        public NestedOrderedProduct()
        {
            ProductID = "";
            Size = 0;
            Count = 0;
        }

        public NestedOrderedProduct(string productID, int size)
        {
            ProductID = productID;
            Size = size;
            Count = 1;
        }
    }
    
    [FirestoreData]
    internal class ActiveOrder
    {
        [FirestoreDocumentId]
        public string OrderID { get; set; }

        [FirestoreProperty]
        public string CafeID { get; set; }
        
        [FirestoreProperty]
        public int FloorNumber { get; set; }

        [FirestoreProperty]
        public Position Table { get; set; }

        [FirestoreProperty]
        public List<OrderedProduct> OrderedProducts { get; set; }

        public ActiveOrder()
        {

        }

        public ActiveOrder(string cafeID ,int floorNumber, Position table)
        {
            this.CafeID = cafeID;
            this.FloorNumber = floorNumber;
            this.Table = table;
            OrderedProducts = new List<OrderedProduct>();
        }

        public void AddOrderedProduct(OrderedProduct orderedProduct)
        {
            if (OrderedProducts == null)
            {
                OrderedProducts = new List<OrderedProduct>();
            }
            OrderedProducts.Add(orderedProduct);
            OrderedProductsChanged?.Invoke(this, new EventArgs());
        }

        public void RemoveOrderProduct(OrderedProduct orderedProduct)
        {
            if (OrderedProducts == null)
            {
                OrderedProducts = new List<OrderedProduct>();
                return;
            }
            int index = OrderedProducts.FindLastIndex((op) => op == orderedProduct);
            if (index!=-1)
            {
                OrderedProducts.RemoveAt(index);
            }
            OrderedProductsChanged?.Invoke(this, new EventArgs());
        }

        public List<NestedOrderedProduct> NestedOrderedProducts
        {
            get
            {
                List<NestedOrderedProduct> nestedOrderedProducts = new List<NestedOrderedProduct>();
                foreach (var orderedProduct in OrderedProducts)
                {
                    NestedOrderedProduct existNestedOrderedProduct = nestedOrderedProducts.Find((op) => op.ProductID == orderedProduct.ProductID && op.Size == orderedProduct.Size);
                    if (existNestedOrderedProduct == null)
                    {
                        nestedOrderedProducts.Add(new NestedOrderedProduct(orderedProduct.ProductID, orderedProduct.Size));
                    }
                    else
                    {
                        existNestedOrderedProduct.Count++;
                    }
                }
                return nestedOrderedProducts;
            }
        }

        public event EventHandler OrderedProductsChanged;
    }

    [FirestoreData]
    internal class Order
    {
        [FirestoreDocumentId]
        public string OrderID { get; set; }

        [FirestoreProperty]
        public string CafeID { get; set; }

        [FirestoreProperty]
        public string StaffID { get; set; }

        private DateTime time;
        public DateTime Time
        {
            get => time;
            set
            {
                time = value;
            }
        }
        [FirestoreProperty]
        public long BinaryTime
        {
            get => time.ToBinary();
            set => time = DateTime.FromBinary(value);
        }

        [FirestoreProperty]
        public long Total { get; set; }

        [FirestoreProperty]
        public List<OrderedProduct> OrderedProducts { get; set; }

        public List<NestedOrderedProduct> NestedOrderedProducts
        {
            get
            {
                List<NestedOrderedProduct> nestedOrderedProducts = new List<NestedOrderedProduct>();
                foreach (var orderedProduct in OrderedProducts)
                {
                    NestedOrderedProduct existNestedOrderedProduct = nestedOrderedProducts.Find((op) => op.ProductID == orderedProduct.ProductID && op.Size == orderedProduct.Size);
                    if (existNestedOrderedProduct == null)
                    {
                        nestedOrderedProducts.Add(new NestedOrderedProduct(orderedProduct.ProductID, orderedProduct.Size));
                    }
                    else
                    {
                        existNestedOrderedProduct.Count++;
                    }
                }
                return nestedOrderedProducts;
            }
        }

        public Order()
        {

        }

        public Order(string cafeID ,string staffID ,DateTime time, long total, List<OrderedProduct> orderedProducts)
        {
            this.CafeID = cafeID;
            this.StaffID = staffID;
            this.Time = time;
            this.Total = total;
            this.OrderedProducts = new List<OrderedProduct>(orderedProducts);
        }
    }
}