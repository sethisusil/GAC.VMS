using System.Diagnostics.CodeAnalysis;

namespace GAC.WMS.Worker.Models
{
    [ExcludeFromCodeCoverage]
    public class GacWmsApiConfig
    {
        public required string BaseUrl { get; set; }
        public required string UploadCustomerUrl { get; set; }
        public required string UploadProductUrl { get; set; }
        public required string UploadPurchaseOrderUrl { get; set; }
        public required string UploadSalesOrderUrl { get; set; }
    }
}
