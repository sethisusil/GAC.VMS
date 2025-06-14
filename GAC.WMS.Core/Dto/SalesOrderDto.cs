namespace GAC.WMS.Core.Dtos
{
    public class SalesOrderDto
    {
        public int Id { get; set; }
        public DateTime ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public int ShipmentAddressId { get; set; }
        public AddressDto? ShipmentAddress { get; set; }
        public ICollection<OrderItemDto>? Products { get; set; }
    }
}
