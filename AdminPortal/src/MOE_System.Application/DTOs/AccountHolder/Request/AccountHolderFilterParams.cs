namespace MOE_System.Application.DTOs.AccountHolder.Request;

public class AccountHolderFilterParams
{
    public string? Search { get; set; }
    public string? EducationLevel { get; set; }
    public string? SchoolingStatus { get; set; }
    public string? ResidentialStatus { get; set; }
    public decimal? MinBlance { get; set; }
    public decimal? MaxBlance { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
}
