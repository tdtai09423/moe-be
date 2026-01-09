using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.EService.Infrastructure.Data.Entities;

[Index("Email", Name = "IX_AccountHolders_Email", IsUnique = true)]
[Index("Nric", Name = "IX_AccountHolders_NRIC", IsUnique = true)]
public partial class AccountHolder
{
    [Key]
    public string Id { get; set; } = null!;

    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string RegisteredAddress { get; set; } = null!;

    [StringLength(256)]
    public string Email { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    [Column("NRIC")]
    [StringLength(50)]
    public string Nric { get; set; } = null!;

    [StringLength(50)]
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

    [InverseProperty("AccountHolder")]
    public virtual EducationAccount? EducationAccount { get; set; }
}
