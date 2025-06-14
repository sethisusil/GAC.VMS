namespace GAC.WMS.Domain.Entities
{
    public class OrderItem:Entity
    {
        // Optional navigation for PurchaseOrder
        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder? PurchaseOrder { get; set; }

        // Optional navigation for SalesOrder
        public int? SalesOrderId { get; set; }

        public SalesOrder? SalesOrder { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
    }
}
