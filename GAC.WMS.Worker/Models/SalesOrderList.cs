using System.Xml.Serialization;

namespace GAC.WMS.Worker.Models
{
    [XmlRoot("SalesOrders")]
    public class SalesOrderList
    {
        [XmlElement("SalesOrder")]
        public List<SalesOrder>? SalesOrders { get; set; }
    }

    public class SalesOrder
    {
        public string? ProcessingDate { get; set; }

        public Customer? Customer { get; set; }

        public Address? ShipmentAddress { get; set; }

        [XmlArray("Products")]
        [XmlArrayItem("Product")]
        public List<Product>? Products { get; set; }
    }
}
