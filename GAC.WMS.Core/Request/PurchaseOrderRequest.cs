using GAC.WMS.Core.Dtos;
using System.Xml.Serialization;

namespace GAC.WMS.Core.Request
{
    public class PurchaseOrderRequest
    {
        public DateTime? ProcessingDate { get; set; }
        public int? CustomerId { get; set; }
        public CustomerRequest? Customer { get; set; }
        [XmlArray("Products")]
        [XmlArrayItem("Product")]
        public ICollection<OrderItemDto>? Products { get; set; }
    }
}
