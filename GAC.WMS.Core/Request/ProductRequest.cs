namespace GAC.WMS.Core.Request
{
    public class ProductRequest
    {
        public string? Code { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DimensionsRequest? Dimensions { get; set; }
    }
}
