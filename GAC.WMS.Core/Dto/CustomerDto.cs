namespace GAC.WMS.Core.Dtos
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int AddressId { get; set; }
        public AddressDto? Address { get; set; }
    }
}
