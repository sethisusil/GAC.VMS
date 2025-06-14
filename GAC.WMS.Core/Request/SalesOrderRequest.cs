using GAC.WMS.Core.Dtos;

namespace GAC.WMS.Core.Request
{
    public class SalesOrderRequest
    {
        public DateTime ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public CustomerRequest? Customer { get; set; }
        public AddressRequest? ShipmentAddress { get; set; }
        public ICollection<OrderItemDto>? Products { get; set; }
    }
}
