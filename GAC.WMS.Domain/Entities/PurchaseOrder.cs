namespace GAC.WMS.Domain.Entities
{
    public class PurchaseOrder:Entity
    {
        public DateTime ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public ICollection<OrderItem>? Products { get; set; }
    }
}
