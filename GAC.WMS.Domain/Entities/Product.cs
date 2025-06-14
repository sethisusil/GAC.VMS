namespace GAC.WMS.Domain.Entities
{
    public class Product : Entity
    {
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int DimensionsId { get; set; }
        public Dimensions Dimensions { get; set; } = default!;
    }
}
