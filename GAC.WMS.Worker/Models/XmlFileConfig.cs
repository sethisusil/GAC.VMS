namespace GAC.WMS.Worker.Models
{
    public class XmlFileConfig
    {
        public required string BasePath { get; set; }
        public required string CustomerFileName { get; set; }
        public required string ProductFileName { get; set; }
        public required string PurchaseOrderFileName { get; set; }
        public required string SalesOrderFileName { get; set; }
        public required string ProcessedPath { get; set; }
    }
}
