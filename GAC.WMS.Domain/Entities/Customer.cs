namespace GAC.WMS.Domain.Entities
{
    public class Customer: Entity
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public int AddressId { get; set; }
        public Address? Address { get; set; }
    }
}
