namespace GAC.WMS.Domain.Entities
{
    public class Address: Entity
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string ZipCode { get; set; }
        public required string Country { get; set; }
    }
}
