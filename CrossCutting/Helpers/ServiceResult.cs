namespace Domain.DTO;

public class ServiceResult<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }

    public static ServiceResult<T> MakeErrorResult(string message) => new()
    {
        Success = false,
        ErrorMessage = message,
        Data = default
    };

    public static ServiceResult<T> MakeSuccessResult(T data) => new()
    {
        Success = true,
        Data = data,
        ErrorMessage = string.Empty
    };
}
