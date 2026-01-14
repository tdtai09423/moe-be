using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MOE_System.Domain.Entities;

namespace MOE_System.EService.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountHolder> AccountHolders { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<BatchExecution> BatchExecutions { get; set; }

    public virtual DbSet<BatchRuleExecution> BatchRuleExecutions { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<EducationAccount> EducationAccounts { get; set; }

    public virtual DbSet<Enrollment> Enrollments { get; set; }

    public virtual DbSet<HistoryOfChange> HistoryOfChanges { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<TopupRule> TopupRules { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Integrated Security=True;Initial Catalog=MOE_DB;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;");
        }
    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountHolder>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_AccountHolders_Email").IsUnique();

            entity.HasIndex(e => e.Nric, "IX_AccountHolders_NRIC").IsUnique();

            entity.Property(e => e.Address).HasDefaultValue("");
            entity.Property(e => e.CitizenId).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MailingAddress).HasDefaultValue("");
            entity.Property(e => e.Nric)
                .HasMaxLength(50)
                .HasColumnName("NRIC");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<BatchExecution>(entity =>
        {
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<BatchRuleExecution>(entity =>
        {
            entity.HasIndex(e => e.BatchId, "IX_BatchRuleExecutions_BatchID");

            entity.HasIndex(e => e.RuleId, "IX_BatchRuleExecutions_RuleID");

            entity.Property(e => e.BatchId).HasColumnName("BatchID");
            entity.Property(e => e.RuleId).HasColumnName("RuleID");

            entity.HasOne(d => d.Batch).WithMany(p => p.BatchRuleExecutions)
                .HasForeignKey(d => d.BatchId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Rule).WithMany(p => p.BatchRuleExecutions)
                .HasForeignKey(d => d.RuleId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(e => e.ProviderId, "IX_Courses_ProviderId");

            entity.Property(e => e.CourseCode).HasMaxLength(50);
            entity.Property(e => e.CourseName).HasMaxLength(200);
            entity.Property(e => e.FeeAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentType).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("");
            entity.Property(e => e.TermName)
                .HasMaxLength(100)
                .HasDefaultValue("");

            entity.HasOne(d => d.Provider).WithMany(p => p.Courses)
                .HasForeignKey(d => d.ProviderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EducationAccount>(entity =>
        {
            entity.HasIndex(e => e.AccountHolderId, "IX_EducationAccounts_AccountHolderId").IsUnique();

            entity.Property(e => e.Balance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Password).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(100);

            entity.HasOne(d => d.AccountHolder).WithOne(p => p.EducationAccount)
                .HasForeignKey<EducationAccount>(d => d.AccountHolderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasIndex(e => e.CourseId, "IX_Enrollments_CourseId");

            entity.HasIndex(e => e.EducationAccountId, "IX_Enrollments_EducationAccountId");

            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EducationAccount).WithMany(p => p.Enrollments)
                .HasForeignKey(d => d.EducationAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<HistoryOfChange>(entity =>
        {
            entity.HasIndex(e => e.EducationAccountId, "IX_HistoryOfChanges_EducationAccountId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.EducationAccount).WithMany(p => p.HistoryOfChanges)
                .HasForeignKey(d => d.EducationAccountId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasIndex(e => e.EnrollmentId, "IX_Invoices_EnrollmentID");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EnrollmentId).HasColumnName("EnrollmentID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<TopupRule>(entity =>
        {
            entity.Property(e => e.RuleName).HasMaxLength(200);
            entity.Property(e => e.TopupAmount).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasIndex(e => e.InvoiceId, "IX_Transactions_InvoiceId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BalanceAfter).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.BalanceBefore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Invoice).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
