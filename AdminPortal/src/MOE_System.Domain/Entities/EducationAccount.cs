using MOE_System.Domain.Common;

namespace MOE_System.Domain.Entities;

public class EducationAccount : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ClosedDate { get; set; }

    // Foreign key
    public string AccountHolderId { get; set; } = string.Empty;

    // Navigation properties
    public AccountHolder? AccountHolder { get; set; }
    public ICollection<HistoryOfChange> HistoryOfChanges { get; set; } = new List<HistoryOfChange>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public void CloseAccount()
    {
        if (!IsActive) return;
        
        IsActive = false;
        ClosedDate = DateTime.UtcNow;
    }
}
