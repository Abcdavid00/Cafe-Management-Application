namespace CSWBManagementApplication.Models
{
    internal class Order
    {
        private int orderId;

        public int OrderId
        {
            get { return orderId; }
            set { orderId = value; }
        }

        private int customerId;

        public int CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        private int productId;

        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
    }
}