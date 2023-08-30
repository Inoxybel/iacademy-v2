namespace Domain.DTO;

public class ServiceResult<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}
