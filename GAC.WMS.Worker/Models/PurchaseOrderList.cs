using GAC.WMS.Core.Dtos;
using System.Xml.Serialization;

namespace GAC.WMS.Worker.Models
{
    [XmlRoot("PurchaseOrders")]
    public class PurchaseOrderList
    {
        [XmlElement("PurchaseOrder")]
        public List<PurchaseOrder> PurchaseOrders { get; set; }
    }

    public class PurchaseOrder
    {
        public string? ProcessingDate { get; set; }  // You can change to DateTime if needed

        public Customer? Customer { get; set; }

        [XmlArray("Products")]
        [XmlArrayItem("Product")]
        public List<Product>? Products { get; set; }
    }

    public class Customer
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public Address? Address { get; set; }
    }

    public class Address
    {
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
    }

    public class Product
    {
        public string? ProductCode { get; set; }
        public int Quantity { get; set; }
    }
}
