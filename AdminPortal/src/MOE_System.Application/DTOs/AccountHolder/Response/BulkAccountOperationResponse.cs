namespace MOE_System.Application.DTOs.AccountHolder.Response;

public class BulkAccountOperationResponse
{
    public int TotalProcessed { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> SuccessfulIds { get; set; } = new List<string>();
    public List<FailedAccountOperation> FailedOperations { get; set; } = new List<FailedAccountOperation>();
}

public class FailedAccountOperation
{
    public string AccountId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
