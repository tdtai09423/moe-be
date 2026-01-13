using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MOE_System.Application.Common.Dashboard;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs.Dashboard.Response;
using MOE_System.Application.Services.Dashboard;
using MOE_System.Domain.Entities;
using Xunit;

namespace MOE_System.Application.Tests.DashboardServiceTests;

public class DashboardServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<BatchRuleExecution>> _batchRuleExecutionRepositoryMock;
    private readonly Mock<IGenericRepository<EducationAccount>> _educationAccountRepositoryMock;
    private readonly Mock<IGenericRepository<Enrollment>> _enrollmentRepositoryMock;
    private readonly DashboardService _dashboardService;

    public DashboardServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _batchRuleExecutionRepositoryMock = new Mock<IGenericRepository<BatchRuleExecution>>();
        _educationAccountRepositoryMock = new Mock<IGenericRepository<EducationAccount>>();
        _enrollmentRepositoryMock = new Mock<IGenericRepository<Enrollment>>();
        
        _unitOfWorkMock.Setup(u => u.GetRepository<BatchRuleExecution>())
            .Returns(_batchRuleExecutionRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<EducationAccount>())
            .Returns(_educationAccountRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Enrollment>())
            .Returns(_enrollmentRepositoryMock.Object);
        
        _dashboardService = new DashboardService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithBatchType_ReturnsFutureScheduledTopUps()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<BatchRuleExecution>
        {
            new()
            {
                TopupRule = new TopupRule { RuleName = "Future Rule 1", TopupAmount = 1000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(2), Status = "Scheduled" }
            },
            new()
            {
                TopupRule = new TopupRule { RuleName = "Future Rule 2", TopupAmount = 2000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(5), Status = "Scheduled" }
            }
        };

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.All(x => x.ScheduledTime > now).Should().BeTrue();
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithBatchType_FiltersPastScheduledTime()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<BatchRuleExecution>
        {
            new()
            {
                TopupRule = new TopupRule { RuleName = "Past Rule", TopupAmount = 1000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(-1), Status = "Scheduled" }
            },
            new()
            {
                TopupRule = new TopupRule { RuleName = "Future Rule", TopupAmount = 2000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(2), Status = "Scheduled" }
            }
        };

        // Service should filter and only return future scheduled
        var filteredData = mockData.Where(x => x.BatchExecution!.ScheduledTime >= now).ToList();

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(filteredData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].Name.Should().Be("Future Rule");
        result.All(x => x.ScheduledTime > now).Should().BeTrue();
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithBatchType_ReturnsEmptyWhenAllPast()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<BatchRuleExecution>
        {
            new()
            {
                TopupRule = new TopupRule { RuleName = "Past Rule 1", TopupAmount = 1000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(-5), Status = "Scheduled" }
            },
            new()
            {
                TopupRule = new TopupRule { RuleName = "Past Rule 2", TopupAmount = 2000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(-2), Status = "Scheduled" }
            }
        };

        var filteredData = mockData.Where(x => x.BatchExecution!.ScheduledTime >= now).ToList();

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(filteredData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithBatchType_OrdersByScheduledTimeAscending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<BatchRuleExecution>
        {
            new()
            {
                TopupRule = new TopupRule { RuleName = "Rule 3", TopupAmount = 3000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(3), Status = "Scheduled" }
            },
            new()
            {
                TopupRule = new TopupRule { RuleName = "Rule 1", TopupAmount = 1000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(1), Status = "Scheduled" }
            },
            new()
            {
                TopupRule = new TopupRule { RuleName = "Rule 2", TopupAmount = 2000m },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(2), Status = "Scheduled" }
            }
        }.OrderBy(x => x.BatchExecution!.ScheduledTime).ToList();

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Should().BeInAscendingOrder(x => x.ScheduledTime);
        result[0].Name.Should().Be("Rule 1");
        result[1].Name.Should().Be("Rule 2");
        result[2].Name.Should().Be("Rule 3");
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithBatchType_LimitTo5Results()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = Enumerable.Range(1, 10)
            .Select(i => new BatchRuleExecution
            {
                TopupRule = new TopupRule { RuleName = $"Rule {i}", TopupAmount = 1000m * i },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(i), Status = "Scheduled" }
            })
            .OrderBy(x => x.BatchExecution!.ScheduledTime)
            .Take(5)
            .ToList();

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Count.Should().BeLessThanOrEqualTo(5);
        result.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithBatchType_ReturnsCorrectDataMapping()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var scheduledTime = now.AddHours(3);
        var mockData = new List<BatchRuleExecution>
        {
            new()
            {
                TopupRule = new TopupRule { RuleName = "Test Rule", TopupAmount = 5000m },
                BatchExecution = new BatchExecution { ScheduledTime = scheduledTime, Status = "Scheduled" }
            }
        };

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result[0].Name.Should().Be("Test Rule");
        result[0].TopUpAmount.Should().Be(5000m);
        result[0].ScheduledTime.Should().Be(scheduledTime);
        result[0].Status.Should().Be("Scheduled");
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithNonBatchType_ThrowsNotImplementedException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(
            () => _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Individual, CancellationToken.None));

        exception.Message.Should().Contain("not implemented");
    }

    #region GetRecentActivitiesAsync Tests

    [Fact]
    public async Task GetRecentActivitiesAsync_WithAccountsType_ReturnsLatestAccounts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<EducationAccount>
        {
            new()
            {
                CreatedAt = now.AddDays(-1),
                AccountHolder = new AccountHolder { FirstName = "John", LastName = "Doe", Email = "john@test.com" }
            },
            new()
            {
                CreatedAt = now.AddDays(-2),
                AccountHolder = new AccountHolder { FirstName = "Jane", LastName = "Smith", Email = "jane@test.com" }
            }
        };

        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(RecentActivityTypes.Accounts, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result[0].Name.Should().Contain("John");
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_WithAccountsType_OrdersByCreatedAtDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<EducationAccount>
        {
            new()
            {
                CreatedAt = now,
                AccountHolder = new AccountHolder { FirstName = "Latest", LastName = "Account", Email = "latest@test.com" }
            },
            new()
            {
                CreatedAt = now.AddDays(-5),
                AccountHolder = new AccountHolder { FirstName = "Older", LastName = "Account", Email = "older@test.com" }
            }
        }.OrderByDescending(x => x.CreatedAt).ToList();

        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(RecentActivityTypes.Accounts, CancellationToken.None);

        // Assert
        result.Should().BeInDescendingOrder(x => x.ActivityDate);
        result[0].Name.Should().Contain("Latest");
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_WithAccountsType_LimitTo10Results()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = Enumerable.Range(1, 15)
            .Select(i => new EducationAccount
            {
                CreatedAt = now.AddDays(-i),
                AccountHolder = new AccountHolder { FirstName = $"Account{i}", LastName = "Test", Email = $"account{i}@test.com" }
            })
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToList();

        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(RecentActivityTypes.Accounts, CancellationToken.None);

        // Assert
        result.Count.Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_WithAccountsType_ReturnsEmptyWhenNoAccounts()
    {
        // Arrange
        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(new List<EducationAccount>());

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(RecentActivityTypes.Accounts, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_WithEnrollmentsType_ReturnsLatestEnrollments()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<Enrollment>
        {
            new()
            {
                EnrollDate = now.AddDays(-1),
                Course = new Course { CourseName = "Math 101" },
                EducationAccount = new EducationAccount
                {
                    AccountHolder = new AccountHolder { FirstName = "Student", LastName = "One", Email = "student1@test.com" }
                }
            },
            new()
            {
                EnrollDate = now.AddDays(-2),
                Course = new Course { CourseName = "English 101" },
                EducationAccount = new EducationAccount
                {
                    AccountHolder = new AccountHolder { FirstName = "Student", LastName = "Two", Email = "student2@test.com" }
                }
            }
        };

        _enrollmentRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<Enrollment, bool>>>(),
                It.IsAny<Func<IQueryable<Enrollment>, IQueryable<Enrollment>>>(),
                It.IsAny<Func<IQueryable<Enrollment>, IOrderedQueryable<Enrollment>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(RecentActivityTypes.Enrollments, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result[0].CourseName.Should().Be("Math 101");
        result[0].StudentName.Should().Contain("Student");
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_WithEnrollmentsType_OrdersByEnrollDateDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<Enrollment>
        {
            new()
            {
                EnrollDate = now,
                Course = new Course { CourseName = "Latest Course" },
                EducationAccount = new EducationAccount
                {
                    AccountHolder = new AccountHolder { FirstName = "Latest", LastName = "Student" }
                }
            },
            new()
            {
                EnrollDate = now.AddDays(-5),
                Course = new Course { CourseName = "Older Course" },
                EducationAccount = new EducationAccount
                {
                    AccountHolder = new AccountHolder { FirstName = "Older", LastName = "Student" }
                }
            }
        }.OrderByDescending(x => x.EnrollDate).ToList();

        _enrollmentRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<Enrollment, bool>>>(),
                It.IsAny<Func<IQueryable<Enrollment>, IQueryable<Enrollment>>>(),
                It.IsAny<Func<IQueryable<Enrollment>, IOrderedQueryable<Enrollment>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(RecentActivityTypes.Enrollments, CancellationToken.None);

        // Assert
        result.Should().BeInDescendingOrder(x => x.ActivityDate);
        result[0].CourseName.Should().Be("Latest Course");
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_WithEnrollmentsType_LimitTo10Results()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = Enumerable.Range(1, 15)
            .Select(i => new Enrollment
            {
                EnrollDate = now.AddDays(-i),
                Course = new Course { CourseName = $"Course {i}" },
                EducationAccount = new EducationAccount
                {
                    AccountHolder = new AccountHolder { FirstName = $"Student{i}", LastName = "Test" }
                }
            })
            .OrderByDescending(x => x.EnrollDate)
            .Take(10)
            .ToList();

        _enrollmentRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<Enrollment, bool>>>(),
                It.IsAny<Func<IQueryable<Enrollment>, IQueryable<Enrollment>>>(),
                It.IsAny<Func<IQueryable<Enrollment>, IOrderedQueryable<Enrollment>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(RecentActivityTypes.Enrollments, CancellationToken.None);

        // Assert
        result.Count.Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_WithEnrollmentsType_ReturnsEmptyWhenNoEnrollments()
    {
        // Arrange
        _enrollmentRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<Enrollment, bool>>>(),
                It.IsAny<Func<IQueryable<Enrollment>, IQueryable<Enrollment>>>(),
                It.IsAny<Func<IQueryable<Enrollment>, IOrderedQueryable<Enrollment>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(new List<Enrollment>());

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(RecentActivityTypes.Enrollments, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_WithUnsupportedType_ThrowsNotImplementedException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotImplementedException>(
            () => _dashboardService.GetRecentActivitiesAsync((RecentActivityTypes)999, CancellationToken.None));

        exception.Message.Should().Contain("not implemented");
    }

    #endregion
}