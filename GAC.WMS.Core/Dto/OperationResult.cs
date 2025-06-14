namespace GAC.WMS.Core.Dtos
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
    public class OperationResult<T>: OperationResult
    {
        public T Data { get; set; }
    }
}
