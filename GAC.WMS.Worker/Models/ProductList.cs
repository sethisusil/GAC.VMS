using GAC.WMS.Core.Request;
using System.Xml.Serialization;

namespace GAC.WMS.Worker.Models
{
    [XmlRoot("Products")]
    public class ProductList
    {
        [XmlElement("Product")]
        public List<ProductRequest>? Products { get; set; }
    }
}
