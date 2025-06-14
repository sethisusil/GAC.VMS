namespace GAC.WMS.Core.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int DimensionsId { get; set; }
        public DimensionsDto? Dimensions { get; set; }
    }
}
