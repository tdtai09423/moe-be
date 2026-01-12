using System;
using System.Collections.Generic;

namespace MOE_System.Domain.Entities;

public partial class AccountHolder
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string RegisteredAddress { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string Nric { get; set; } = null!;

    public string CitizenId { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string ContLearningStatus { get; set; } = null!;

    public string EducationLevel { get; set; } = null!;

    public string SchoolingStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    public string MailingAddress { get; set; } = null!;

    public string Address { get; set; } = null!;

    public virtual EducationAccount? EducationAccount { get; set; }
}
