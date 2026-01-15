using MOE_System.Domain.Enums;
using System.Collections.Generic;

namespace MOE_System.Application.DTOs.AccountHolder.Request;

public class AccountHolderFilterParams
{
    public string? Search { get; set; }
    public List<EducationLevel>? EducationLevels { get; set; }
    public string? SchoolingStatus { get; set; }
    public List<ResidentialStatus>? ResidentialStatuses { get; set; }
    public decimal? MinBlance { get; set; }
    public decimal? MaxBlance { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public bool IsActive { get; set; } = true;
    // Sorting
    public SortBy? SortBy { get; set; }
    public bool? SortDescending { get; set; }
}
