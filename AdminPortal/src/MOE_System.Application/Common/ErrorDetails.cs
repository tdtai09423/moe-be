namespace MOE_System.Application.Common;

public class ErrorDetails
{

    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public Dictionary<string, List<string>>? FieldErrors { get; set; }

    public object? Details { get; set; }
}
