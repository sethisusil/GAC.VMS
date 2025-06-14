namespace GAC.WMS.Domain.Entities
{
    public class SalesOrder:Entity
    {
        public DateTime ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int ShipmentAddressId { get; set; }
        public Address? ShipmentAddress { get; set; }
        public ICollection<OrderItem>? Products { get; set; }
    }
}
