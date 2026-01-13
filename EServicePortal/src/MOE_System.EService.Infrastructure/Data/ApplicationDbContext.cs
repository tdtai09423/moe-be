using System;
using System.Collections.Generic;
using MOE_System.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MOE_System.EService.Infrastructure.Data;

public partial class ApplicationDbContext : DbContext
{
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountHolder>(entity =>
        {
            entity.Property(e => e.Address).HasDefaultValue("");
            entity.Property(e => e.MailingAddress).HasDefaultValue("");
        });

        modelBuilder.Entity<BatchRuleExecution>(entity =>
        {
            entity.HasOne(d => d.BatchExecution).WithMany(p => p.BatchRuleExecutions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TopupRule).WithMany(p => p.BatchRuleExecutions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasOne(d => d.Provider).WithMany(p => p.Courses).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EducationAccount>(entity =>
        {
            entity.HasOne(d => d.AccountHolder).WithOne(p => p.EducationAccount).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasOne(d => d.Course).WithMany(p => p.Enrollments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EducationAccount).WithMany(p => p.Enrollments).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<HistoryOfChange>(entity =>
        {
            entity.HasOne(d => d.EducationAccount).WithMany(p => p.HistoryOfChanges).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasOne(d => d.Enrollment).WithMany(p => p.Invoices).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasOne(d => d.Invoice).WithMany(p => p.Transactions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
