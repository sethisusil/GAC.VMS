namespace GAC.WMS.Core.Request
{
    public class CustomerRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public AddressRequest? Address { get; set; }
    }
}
