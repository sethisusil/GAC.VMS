using GAC.WMS.Core.Request;
using System.Xml.Serialization;

namespace GAC.WMS.Worker.Models
{
    [XmlRoot("Customers")]
    public class CustomerList
    {
        [XmlElement("Customer")]
        public List<CustomerRequest>? Customers { get; set; }
    }
}
