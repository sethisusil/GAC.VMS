namespace GAC.WMS.Domain.Entities
{
    public class Entity<T> where T : struct
    {
        public T Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string? DeletedBy { get; set; }
    }
    public class Entity : Entity<int>
    {

    }
}
