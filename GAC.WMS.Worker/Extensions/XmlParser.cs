using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace GAC.WMS.Worker.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class XmlParser
    {
        // Deserialize single object from XML string
        public static T Deserialize<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("XML input cannot be null or empty.");

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StringReader reader = new StringReader(xml))
            {
                return (T)serializer.Deserialize(reader);
            }
        }

        // Deserialize list of objects from XML string
        public static List<T> DeserializeList<T>(string xml, string rootElementName)
        {
            if (string.IsNullOrWhiteSpace(xml))
                throw new ArgumentException("XML input cannot be null or empty.");

            XmlRootAttribute root = new XmlRootAttribute(rootElementName);
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>), root);

            using (StringReader reader = new StringReader(xml))
            {
                return (List<T>)serializer.Deserialize(reader);
            }
        }

        public static T DeserializeFromFile<T>(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(filePath))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
