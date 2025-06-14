namespace GAC.WMS.Domain.Entities
{
    public class Dimensions : Entity
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
    }
}
