namespace Business.Models;

public class ServiceResult
{
    public bool Succeeded { get; set; }
    public int StatusCode { get; set; }
    public string? Error { get; set; }
}
