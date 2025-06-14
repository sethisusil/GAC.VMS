namespace GAC.WMS.Core.Dtos
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }
        public DateTime? ProcessingDate { get; set; }
        public int? CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public ICollection<OrderItemDto>? Products { get; set; }
    }
}
