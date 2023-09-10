namespace Domain.Entities;

public class ChatCompletion
{
    public string Id { get; set; }
    public string Object { get; set; }
    public string Created { get; set; }
    public string Model { get; set; }
    public Usage Usage { get; set; }
    public List<Choices> Choices { get; set; }
}
