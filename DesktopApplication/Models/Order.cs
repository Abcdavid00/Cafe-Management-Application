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

    [FirestoreData]
    internal class ActiveOrder
    {
        [FirestoreDocumentId]
        public string OrderID { get; set; }

        [FirestoreProperty]
        public int FloorNumber { get; set; }

        [FirestoreProperty]
        public Position Table { get; set; }

        [FirestoreProperty]
        public List<OrderedProduct> OrderedProducts { get; set; }

        public ActiveOrder()
        {

        }

        public ActiveOrder(int floorNumber, Position table)
        {
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
        }
    }

    [FirestoreData]
    internal class Order
    {
        [FirestoreDocumentId]
        public string OrderID { get; set; }

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

        public Order()
        {

        }

        public Order(DateTime time, long total, List<OrderedProduct> orderedProducts)
        {
            this.Time = time;
            this.Total = total;
            this.OrderedProducts = new List<OrderedProduct>(orderedProducts);
        }
    }
}