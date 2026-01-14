using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MOE_System.Domain.Entities;

namespace MOE_System.Infrastructure.Data.Seeding;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting comprehensive database seeding...");
            
            // Clear all existing data first
            await ClearAllDataAsync();
            
            // Seed in order of dependencies
            await SeedAdminsAsync();
            await SeedAccountHoldersAsync();
            await SeedEducationAccountsAsync();
            await SeedProvidersAsync();
            await SeedCoursesAsync();
            await SeedEnrollmentsAsync();
            await SeedInvoicesAsync();
            await SeedTransactionsAsync();
            await SeedHistoryOfChangesAsync();
            await SeedTopupRulesAsync();
            await SeedBatchExecutionsAsync();
            await SeedBatchRuleExecutionsAsync();
            await SeedResidentsAsync(50);
            
            _logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task ClearAllDataAsync()
    {
        _logger.LogInformation("Clearing all existing data from database...");

        // Delete in reverse order of dependencies to avoid FK constraint issues
        _context.BatchRuleExecutions.RemoveRange(_context.BatchRuleExecutions);
        _context.BatchExecutions.RemoveRange(_context.BatchExecutions);
        _context.TopupRules.RemoveRange(_context.TopupRules);
        _context.HistoryOfChanges.RemoveRange(_context.HistoryOfChanges);
        _context.Transactions.RemoveRange(_context.Transactions);
        _context.Invoices.RemoveRange(_context.Invoices);
        _context.Enrollments.RemoveRange(_context.Enrollments);
        _context.Courses.RemoveRange(_context.Courses);
        _context.Providers.RemoveRange(_context.Providers);
        _context.EducationAccounts.RemoveRange(_context.EducationAccounts);
        _context.AccountHolders.RemoveRange(_context.AccountHolders);
        _context.Admins.RemoveRange(_context.Admins);
        _context.Set<Resident>().RemoveRange(_context.Set<Resident>());

        await _context.SaveChangesAsync();
        _logger.LogInformation("All data cleared successfully.");
    }

    private async Task SeedAdminsAsync()
    {
        _logger.LogInformation("Seeding admins...");

        var admins = new List<Admin>
        {
            new Admin { Id = "admin-001", UserName = "admin", Password = BCrypt.Net.BCrypt.HashPassword("Admin@123") },
            new Admin { Id = "admin-002", UserName = "superadmin", Password = BCrypt.Net.BCrypt.HashPassword("Super@123") },
            new Admin { Id = "admin-003", UserName = "moderator", Password = BCrypt.Net.BCrypt.HashPassword("Mod@123") }
        };

        await _context.Admins.AddRangeAsync(admins);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} admins", admins.Count);
    }

    private async Task SeedAccountHoldersAsync()
    {
        _logger.LogInformation("Seeding account holders...");

        var accountHolders = new List<AccountHolder>
        {
            new AccountHolder
            {
                Id = "holder-001",
                FirstName = "John",
                LastName = "Tan",
                DateOfBirth = new DateTime(1990, 5, 15),
                RegisteredAddress = "Blk 123 Ang Mo Kio Ave 3 #05-123",
                MailingAddress = "Blk 123 Ang Mo Kio Ave 3 #05-123",
                Address = "Blk 123 Ang Mo Kio Ave 3 #05-123",
                Email = "john.tan@email.com",
                ContactNumber = "91234567",
                NRIC = "S9005123A",
                CitizenId = "S9005123A",
                Gender = "Male",
                ContLearningStatus = "Active",
                EducationLevel = "PostSecondary", // Enum: PostSecondary
                SchoolingStatus = "NotInSchool", // Enum: NotInSchool
                ResidentialStatus = "SingaporeCitizen" // Enum: SingaporeCitizen
            },
            new AccountHolder
            {
                Id = "holder-002",
                FirstName = "Mary",
                LastName = "Lim",
                DateOfBirth = new DateTime(1985, 8, 20),
                RegisteredAddress = "Blk 456 Bedok North St 1 #10-456",
                MailingAddress = "Blk 456 Bedok North St 1 #10-456",
                Address = "Blk 456 Bedok North St 1 #10-456",
                Email = "mary.lim@email.com",
                ContactNumber = "92345678",
                NRIC = "S8508234B",
                CitizenId = "S8508234B",
                Gender = "Female",
                ContLearningStatus = "Active",
                EducationLevel = "Tertiary", // Enum: Tertiary (Degree)
                SchoolingStatus = "NotInSchool", // Enum: NotInSchool
                ResidentialStatus = "SingaporeCitizen" // Enum: SingaporeCitizen
            },
            new AccountHolder
            {
                Id = "holder-003",
                FirstName = "Ahmad",
                LastName = "Ibrahim",
                DateOfBirth = new DateTime(1992, 3, 10),
                RegisteredAddress = "Blk 789 Jurong West St 52 #03-789",
                MailingAddress = "Blk 789 Jurong West St 52 #03-789",
                Address = "Blk 789 Jurong West St 52 #03-789",
                Email = "ahmad.ibrahim@email.com",
                ContactNumber = "93456789",
                NRIC = "S9203345C",
                CitizenId = "S9203345C",
                Gender = "Male",
                ContLearningStatus = "Active",
                EducationLevel = "Secondary", // Enum: Secondary
                SchoolingStatus = "NotInSchool", // Enum: NotInSchool
                ResidentialStatus = "PermanentResident" // Enum: PermanentResident
            },
            new AccountHolder
            {
                Id = "holder-004",
                FirstName = "Priya",
                LastName = "Raj",
                DateOfBirth = new DateTime(1988, 11, 25),
                RegisteredAddress = "Blk 321 Clementi Ave 2 #08-321",
                MailingAddress = "Blk 321 Clementi Ave 2 #08-321",
                Address = "Blk 321 Clementi Ave 2 #08-321",
                Email = "priya.raj@email.com",
                ContactNumber = "94567890",
                NRIC = "S8811456D",
                CitizenId = "S8811456D",
                Gender = "Female",
                ContLearningStatus = "Active",
                EducationLevel = "PostSecondary", // Enum: PostSecondary (Diploma)
                SchoolingStatus = "NotInSchool", // Enum: NotInSchool
                ResidentialStatus = "SingaporeCitizen" // Enum: SingaporeCitizen
            },
            new AccountHolder
            {
                Id = "holder-005",
                FirstName = "David",
                LastName = "Wong",
                DateOfBirth = new DateTime(1995, 7, 8),
                RegisteredAddress = "Blk 654 Tampines St 61 #12-654",
                MailingAddress = "Blk 654 Tampines St 61 #12-654",
                Address = "Blk 654 Tampines St 61 #12-654",
                Email = "david.wong@email.com",
                ContactNumber = "95678901",
                NRIC = "S9507567E",
                CitizenId = "S9507567E",
                Gender = "Male",
                ContLearningStatus = "Active",
                EducationLevel = "Tertiary", // Enum: Tertiary (Degree)
                SchoolingStatus = "InSchool", // Enum: InSchool
                ResidentialStatus = "SingaporeCitizen" // Enum: SingaporeCitizen
            }
        };

        await _context.AccountHolders.AddRangeAsync(accountHolders);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} account holders", accountHolders.Count);
    }

    private async Task SeedEducationAccountsAsync()
    {
        _logger.LogInformation("Seeding education accounts...");

        var educationAccounts = new List<EducationAccount>
        {
            new EducationAccount
            {
                Id = "edu-001",
                AccountHolderId = "holder-001",
                UserName = "john.tan",
                Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                Balance = 5000.00m
            },
            new EducationAccount
            {
                Id = "edu-002",
                AccountHolderId = "holder-002",
                UserName = "mary.lim",
                Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                Balance = 3500.00m
            },
            new EducationAccount
            {
                Id = "edu-003",
                AccountHolderId = "holder-003",
                UserName = "ahmad.ibrahim",
                Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                Balance = 2000.00m
            },
            new EducationAccount
            {
                Id = "edu-004",
                AccountHolderId = "holder-004",
                UserName = "priya.raj",
                Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                Balance = 4200.00m
            },
            new EducationAccount
            {
                Id = "edu-005",
                AccountHolderId = "holder-005",
                UserName = "david.wong",
                Password = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                Balance = 6000.00m
            }
        };

        await _context.EducationAccounts.AddRangeAsync(educationAccounts);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} education accounts", educationAccounts.Count);
    }

    private async Task SeedCoursesAsync()
    {
        _logger.LogInformation("Seeding courses...");

        var providers = await _context.Providers.ToListAsync();

        var courses = new List<Course>
        {
            new Course
            {
                Id = "course-001",
                CourseName = "Web Development Fundamentals",
                CourseCode = "WD101",
                FeeAmount = 1500.00m,
                DurationByMonth = 3,
                ProviderId = providers[0].Id,
                PaymentType = "Monthly",
                BillingCycle = "Monthly",
                LearningType = "Online",
                TermName = "Term 1 2026",
                StartDate = new DateTime(2026, 2, 1),
                EndDate = new DateTime(2026, 4, 30),
                Status = "Active"
            },
            new Course
            {
                Id = "course-002",
                CourseName = "Data Science and Analytics",
                CourseCode = "DS201",
                FeeAmount = 2500.00m,
                DurationByMonth = 6,
                ProviderId = providers[0].Id,
                PaymentType = "Monthly",
                BillingCycle = "Monthly",
                LearningType = "Hybrid",
                TermName = "Term 1 2026",
                StartDate = new DateTime(2026, 2, 1),
                EndDate = new DateTime(2026, 7, 31),
                Status = "Active"
            },
            new Course
            {
                Id = "course-003",
                CourseName = "Digital Marketing Essentials",
                CourseCode = "DM101",
                FeeAmount = 1200.00m,
                DurationByMonth = 2,
                ProviderId = providers.Count > 1 ? providers[1].Id : providers[0].Id,
                PaymentType = "One-Time",
                BillingCycle = null,
                LearningType = "Online",
                TermName = "Term 1 2026",
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 4, 30),
                Status = "Active"
            },
            new Course
            {
                Id = "course-004",
                CourseName = "Business Analytics",
                CourseCode = "BA301",
                FeeAmount = 3000.00m,
                DurationByMonth = 4,
                ProviderId = providers.Count > 1 ? providers[1].Id : providers[0].Id,
                PaymentType = "Monthly",
                BillingCycle = "Monthly",
                LearningType = "In-Person",
                TermName = "Term 1 2026",
                StartDate = new DateTime(2026, 2, 15),
                EndDate = new DateTime(2026, 6, 15),
                Status = "Active"
            },
            new Course
            {
                Id = "course-005",
                CourseName = "Cybersecurity Fundamentals",
                CourseCode = "CS101",
                FeeAmount = 1800.00m,
                DurationByMonth = 3,
                ProviderId = providers.Count > 2 ? providers[2].Id : providers[0].Id,
                PaymentType = "Monthly",
                BillingCycle = "Monthly",
                LearningType = "Online",
                TermName = "Term 1 2026",
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 5, 31),
                Status = "Active"
            }
        };

        await _context.Courses.AddRangeAsync(courses);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} courses", courses.Count);
    }

    private async Task SeedEnrollmentsAsync()
    {
        _logger.LogInformation("Seeding enrollments...");

        var enrollments = new List<Enrollment>
        {
            new Enrollment
            {
                Id = "enroll-001",
                CourseId = "course-001",
                EducationAccountId = "edu-001",
                EnrollDate = new DateTime(2026, 1, 15),
                Status = "Active"
            },
            new Enrollment
            {
                Id = "enroll-002",
                CourseId = "course-002",
                EducationAccountId = "edu-002",
                EnrollDate = new DateTime(2026, 1, 20),
                Status = "Active"
            },
            new Enrollment
            {
                Id = "enroll-003",
                CourseId = "course-003",
                EducationAccountId = "edu-003",
                EnrollDate = new DateTime(2026, 2, 1),
                Status = "Active"
            },
            new Enrollment
            {
                Id = "enroll-004",
                CourseId = "course-004",
                EducationAccountId = "edu-004",
                EnrollDate = new DateTime(2026, 2, 5),
                Status = "Active"
            },
            new Enrollment
            {
                Id = "enroll-005",
                CourseId = "course-005",
                EducationAccountId = "edu-005",
                EnrollDate = new DateTime(2026, 2, 10),
                Status = "Active"
            },
            new Enrollment
            {
                Id = "enroll-006",
                CourseId = "course-001",
                EducationAccountId = "edu-003",
                EnrollDate = new DateTime(2026, 1, 18),
                Status = "Completed"
            }
        };

        await _context.Enrollments.AddRangeAsync(enrollments);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} enrollments", enrollments.Count);
    }

    private async Task SeedInvoicesAsync()
    {
        _logger.LogInformation("Seeding invoices...");

        var invoices = new List<Invoice>
        {
            new Invoice
            {
                Id = "inv-001",
                EnrollmentID = "enroll-001",
                Amount = 500.00m,
                DueDate = new DateTime(2026, 2, 15),
                Status = "Paid"
            },
            new Invoice
            {
                Id = "inv-002",
                EnrollmentID = "enroll-001",
                Amount = 500.00m,
                DueDate = new DateTime(2026, 3, 15),
                Status = "Pending"
            },
            new Invoice
            {
                Id = "inv-003",
                EnrollmentID = "enroll-002",
                Amount = 416.67m,
                DueDate = new DateTime(2026, 2, 15),
                Status = "Paid"
            },
            new Invoice
            {
                Id = "inv-004",
                EnrollmentID = "enroll-003",
                Amount = 1200.00m,
                DueDate = new DateTime(2026, 3, 1),
                Status = "Pending"
            },
            new Invoice
            {
                Id = "inv-005",
                EnrollmentID = "enroll-004",
                Amount = 750.00m,
                DueDate = new DateTime(2026, 3, 1),
                Status = "Paid"
            },
            new Invoice
            {
                Id = "inv-006",
                EnrollmentID = "enroll-005",
                Amount = 600.00m,
                DueDate = new DateTime(2026, 3, 1),
                Status = "Overdue"
            }
        };

        await _context.Invoices.AddRangeAsync(invoices);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} invoices", invoices.Count);
    }

    private async Task SeedTransactionsAsync()
    {
        _logger.LogInformation("Seeding transactions...");

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = "txn-001",
                InvoiceId = "inv-001",
                Amount = 500.00m,
                TransactionAt = new DateTime(2026, 2, 10),
                PaymentMethod = "SkillsFuture Credit",
                Status = "Success",
                BalanceBefore = 5000.00m,
                BalanceAfter = 4500.00m
            },
            new Transaction
            {
                Id = "txn-002",
                InvoiceId = "inv-003",
                Amount = 416.67m,
                TransactionAt = new DateTime(2026, 2, 12),
                PaymentMethod = "SkillsFuture Credit",
                Status = "Success",
                BalanceBefore = 3500.00m,
                BalanceAfter = 3083.33m
            },
            new Transaction
            {
                Id = "txn-003",
                InvoiceId = "inv-005",
                Amount = 750.00m,
                TransactionAt = new DateTime(2026, 2, 28),
                PaymentMethod = "SkillsFuture Credit",
                Status = "Success",
                BalanceBefore = 4200.00m,
                BalanceAfter = 3450.00m
            }
        };

        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} transactions", transactions.Count);
    }

    private async Task SeedHistoryOfChangesAsync()
    {
        _logger.LogInformation("Seeding history of changes...");

        var histories = new List<HistoryOfChange>
        {
            new HistoryOfChange
            {
                Id = "hist-001",
                EducationAccountId = "edu-001",
                Amount = 5000.00m,
                Type = "Initial Topup",
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new HistoryOfChange
            {
                Id = "hist-002",
                EducationAccountId = "edu-001",
                Amount = -500.00m,
                Type = "Course Payment",
                CreatedAt = new DateTime(2026, 2, 10)
            },
            new HistoryOfChange
            {
                Id = "hist-003",
                EducationAccountId = "edu-002",
                Amount = 3500.00m,
                Type = "Initial Topup",
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new HistoryOfChange
            {
                Id = "hist-004",
                EducationAccountId = "edu-002",
                Amount = -416.67m,
                Type = "Course Payment",
                CreatedAt = new DateTime(2026, 2, 12)
            },
            new HistoryOfChange
            {
                Id = "hist-005",
                EducationAccountId = "edu-003",
                Amount = 2000.00m,
                Type = "Initial Topup",
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new HistoryOfChange
            {
                Id = "hist-006",
                EducationAccountId = "edu-004",
                Amount = 4200.00m,
                Type = "Initial Topup",
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new HistoryOfChange
            {
                Id = "hist-007",
                EducationAccountId = "edu-004",
                Amount = -750.00m,
                Type = "Course Payment",
                CreatedAt = new DateTime(2026, 2, 28)
            },
            new HistoryOfChange
            {
                Id = "hist-008",
                EducationAccountId = "edu-005",
                Amount = 6000.00m,
                Type = "Initial Topup",
                CreatedAt = new DateTime(2026, 1, 1)
            }
        };

        await _context.HistoryOfChanges.AddRangeAsync(histories);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} history records", histories.Count);
    }

    private async Task SeedTopupRulesAsync()
    {
        _logger.LogInformation("Seeding topup rules...");

        var topupRules = new List<TopupRule>
        {
            new TopupRule
            {
                Id = "rule-001",
                RuleName = "Low Balance Alert - Individual",
                AgeCondition = ">=21",
                BalanceCondition = "<1000",
                EduLevelCond = "Any",
                TopupAmount = 500.00m,
                RuleTargetType = "individual",
                TargetEducationAccountId = "edu-003"
            },
            new TopupRule
            {
                Id = "rule-002",
                RuleName = "Annual Batch Topup",
                AgeCondition = ">=21",
                BalanceCondition = "Any",
                EduLevelCond = "Any",
                TopupAmount = 1000.00m,
                RuleTargetType = "batch",
                TargetEducationAccountId = null
            },
            new TopupRule
            {
                Id = "rule-003",
                RuleName = "Graduate Bonus - Individual",
                AgeCondition = ">=25",
                BalanceCondition = "Any",
                EduLevelCond = "Degree",
                TopupAmount = 750.00m,
                RuleTargetType = "individual",
                TargetEducationAccountId = "edu-002"
            },
            new TopupRule
            {
                Id = "rule-004",
                RuleName = "Mid-Year Batch Topup",
                AgeCondition = ">=18",
                BalanceCondition = "Any",
                EduLevelCond = "Any",
                TopupAmount = 600.00m,
                RuleTargetType = "batch",
                TargetEducationAccountId = null
            }
        };

        await _context.TopupRules.AddRangeAsync(topupRules);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} topup rules", topupRules.Count);
    }

    private async Task SeedBatchExecutionsAsync()
    {
        _logger.LogInformation("Seeding batch executions...");

        var batchExecutions = new List<BatchExecution>
        {
            new BatchExecution
            {
                Id = "batch-001",
                ScheduledTime = new DateTime(2026, 1, 1, 0, 0, 0),
                ExecutedTime = new DateTime(2026, 1, 1, 0, 5, 23),
                Status = "Completed"
            },
            new BatchExecution
            {
                Id = "batch-002",
                ScheduledTime = new DateTime(2026, 2, 1, 0, 0, 0),
                ExecutedTime = new DateTime(2026, 2, 1, 0, 4, 12),
                Status = "Completed"
            },
            new BatchExecution
            {
                Id = "batch-003",
                ScheduledTime = new DateTime(2026, 3, 1, 0, 0, 0),
                ExecutedTime = null,
                Status = "Pending"
            },
            // Future scheduled executions for testing
            new BatchExecution
            {
                Id = "batch-004",
                ScheduledTime = new DateTime(2026, 1, 20, 10, 0, 0),
                ExecutedTime = null,
                Status = "SCHEDULED"
            },
            new BatchExecution
            {
                Id = "batch-005",
                ScheduledTime = new DateTime(2026, 1, 25, 14, 30, 0),
                ExecutedTime = null,
                Status = "SCHEDULED"
            },
            new BatchExecution
            {
                Id = "batch-006",
                ScheduledTime = new DateTime(2026, 2, 15, 9, 0, 0),
                ExecutedTime = null,
                Status = "SCHEDULED"
            }
        };

        await _context.BatchExecutions.AddRangeAsync(batchExecutions);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} batch executions", batchExecutions.Count);
    }

    private async Task SeedBatchRuleExecutionsAsync()
    {
        _logger.LogInformation("Seeding batch rule executions...");

        var batchRuleExecutions = new List<BatchRuleExecution>
        {
            // Historical batch executions
            new BatchRuleExecution
            {
                Id = "batchrule-001",
                BatchID = "batch-001",
                RuleID = "rule-002"
            },
            new BatchRuleExecution
            {
                Id = "batchrule-002",
                BatchID = "batch-001",
                RuleID = "rule-004"
            },
            new BatchRuleExecution
            {
                Id = "batchrule-003",
                BatchID = "batch-002",
                RuleID = "rule-002"
            },
            new BatchRuleExecution
            {
                Id = "batchrule-004",
                BatchID = "batch-003",
                RuleID = "rule-002"
            },
            // Future scheduled INDIVIDUAL topups for testing Individual type (type=2)
            new BatchRuleExecution
            {
                Id = "batchrule-005",
                BatchID = "batch-004",
                RuleID = "rule-001" // Low Balance Alert - Individual (Ahmad)
            },
            new BatchRuleExecution
            {
                Id = "batchrule-006",
                BatchID = "batch-005",
                RuleID = "rule-003" // Graduate Bonus - Individual (Mary)
            },
            // Future scheduled BATCH topup for testing Batch type (type=1)
            new BatchRuleExecution
            {
                Id = "batchrule-007",
                BatchID = "batch-006",
                RuleID = "rule-002" // Annual Batch Topup
            }
        };

        await _context.BatchRuleExecutions.AddRangeAsync(batchRuleExecutions);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} batch rule executions", batchRuleExecutions.Count);
    }

    public async Task SeedProvidersAsync()
    {
        _logger.LogInformation("Seeding providers...");

        var providersData = ProviderSeedData.GetProviders();
        var providers = providersData.Select(kvp => new Provider
        {
            Id = kvp.Key,
            Name = kvp.Value
        }).ToList();

        await _context.Providers.AddRangeAsync(providers);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} providers successfully", providers.Count);
    }

    public async Task SeedResidentsAsync(int count = 50)
    {
        _logger.LogInformation("Seeding residents ({Count})...", count);

        var residents = ResidentSeedData.GetResidents(count);

        await _context.Set<Resident>().AddRangeAsync(residents);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded {Count} residents successfully", residents.Count);
    }

    public async Task<object> GetSeedStatusAsync()
    {
        var adminsCount = await _context.Admins.CountAsync();
        var accountHoldersCount = await _context.AccountHolders.CountAsync();
        var educationAccountsCount = await _context.EducationAccounts.CountAsync();
        var providersCount = await _context.Providers.CountAsync();
        var coursesCount = await _context.Courses.CountAsync();
        var enrollmentsCount = await _context.Enrollments.CountAsync();
        var invoicesCount = await _context.Invoices.CountAsync();
        var transactionsCount = await _context.Transactions.CountAsync();
        var historyCount = await _context.HistoryOfChanges.CountAsync();
        var topupRulesCount = await _context.TopupRules.CountAsync();
        var batchExecutionsCount = await _context.BatchExecutions.CountAsync();
        var batchRuleExecutionsCount = await _context.BatchRuleExecutions.CountAsync();
        var residentsCount = await _context.Set<Resident>().CountAsync();

        return new
        {
            admins = new { count = adminsCount, isSeeded = adminsCount > 0 },
            accountHolders = new { count = accountHoldersCount, isSeeded = accountHoldersCount > 0 },
            educationAccounts = new { count = educationAccountsCount, isSeeded = educationAccountsCount > 0 },
            providers = new { count = providersCount, isSeeded = providersCount > 0 },
            courses = new { count = coursesCount, isSeeded = coursesCount > 0 },
            enrollments = new { count = enrollmentsCount, isSeeded = enrollmentsCount > 0 },
            invoices = new { count = invoicesCount, isSeeded = invoicesCount > 0 },
            transactions = new { count = transactionsCount, isSeeded = transactionsCount > 0 },
            historyOfChanges = new { count = historyCount, isSeeded = historyCount > 0 },
            topupRules = new { count = topupRulesCount, isSeeded = topupRulesCount > 0 },
            batchExecutions = new { count = batchExecutionsCount, isSeeded = batchExecutionsCount > 0 },
            batchRuleExecutions = new { count = batchRuleExecutionsCount, isSeeded = batchRuleExecutionsCount > 0 },
            residents = new { count = residentsCount, isSeeded = residentsCount > 0 }
        };
    }
}
