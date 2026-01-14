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
using MOE_System.Application.Services;
using MOE_System.Domain.Entities;
using Xunit;

namespace MOE_System.Application.Tests.DashboardServiceTests;

public class DashboardServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<BatchRuleExecution>> _batchRuleExecutionRepositoryMock;
    private readonly Mock<IGenericRepository<EducationAccount>> _educationAccountRepositoryMock;
    private readonly DashboardService _dashboardService;

    public DashboardServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _batchRuleExecutionRepositoryMock = new Mock<IGenericRepository<BatchRuleExecution>>();
        _educationAccountRepositoryMock = new Mock<IGenericRepository<EducationAccount>>();
        
        _unitOfWorkMock.Setup(u => u.GetRepository<BatchRuleExecution>())
            .Returns(_batchRuleExecutionRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<EducationAccount>())
            .Returns(_educationAccountRepositoryMock.Object);
        
        _dashboardService = new DashboardService(_unitOfWorkMock.Object);
    }

    #region GetTopUpTypesAsync Tests

    [Fact]
    public async Task GetTopUpTypesAsync_WithBatchType_ReturnsFutureScheduledTopUps()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<BatchRuleExecution>
        {
            new()
            {
                TopupRule = new TopupRule 
                { 
                    RuleName = "Future Rule 1", 
                    TopupAmount = 1000m,
                    RuleTargetType = "BATCH"
                },
                BatchExecution = new BatchExecution 
                { 
                    ScheduledTime = now.AddHours(2), 
                    Status = "SCHEDULED" 
                }
            },
            new()
            {
                TopupRule = new TopupRule 
                { 
                    RuleName = "Future Rule 2", 
                    TopupAmount = 2000m,
                    RuleTargetType = "BATCH"
                },
                BatchExecution = new BatchExecution 
                { 
                    ScheduledTime = now.AddHours(5), 
                    Status = "SCHEDULED" 
                }
            }
        };

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result.All(x => x.ScheduledTime > now).Should().BeTrue();
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithBatchType_ReturnsEmptyWhenNoPastOrFuture()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<BatchRuleExecution>();

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

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
                TopupRule = new TopupRule { RuleName = "Rule 1", TopupAmount = 1000m, RuleTargetType = "BATCH" },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(1), Status = "SCHEDULED" }
            },
            new()
            {
                TopupRule = new TopupRule { RuleName = "Rule 2", TopupAmount = 2000m, RuleTargetType = "BATCH" },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(2), Status = "SCHEDULED" }
            },
            new()
            {
                TopupRule = new TopupRule { RuleName = "Rule 3", TopupAmount = 3000m, RuleTargetType = "BATCH" },
                BatchExecution = new BatchExecution { ScheduledTime = now.AddHours(3), Status = "SCHEDULED" }
            }
        };

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
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
        var mockData = Enumerable.Range(1, 5)
            .Select(i => new BatchRuleExecution
            {
                TopupRule = new TopupRule 
                { 
                    RuleName = $"Rule {i}", 
                    TopupAmount = 1000m * i,
                    RuleTargetType = "BATCH"
                },
                BatchExecution = new BatchExecution 
                { 
                    ScheduledTime = now.AddHours(i), 
                    Status = "SCHEDULED" 
                }
            })
            .ToList();

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Count.Should().BeLessThanOrEqualTo(5);
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
                TopupRule = new TopupRule 
                { 
                    RuleName = "Test Rule", 
                    TopupAmount = 5000m,
                    RuleTargetType = "BATCH"
                },
                BatchExecution = new BatchExecution 
                { 
                    ScheduledTime = scheduledTime, 
                    Status = "SCHEDULED" 
                }
            }
        };

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Batch, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result[0].Name.Should().Be("Test Rule");
        result[0].TopUpAmount.Should().Be(5000m);
        result[0].ScheduledTime.Should().Be(scheduledTime);
        result[0].Status.Should().Be("SCHEDULED");
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithIndividualType_ReturnsFutureScheduledIndividualTopUps()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<BatchRuleExecution>
        {
            new()
            {
                TopupRule = new TopupRule 
                { 
                    RuleTargetType = "INDIVIDUAL",
                    TopupAmount = 1000m,
                    TargetEducationAccount = new EducationAccount
                    {
                        AccountHolder = new AccountHolder
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        }
                    }
                },
                BatchExecution = new BatchExecution 
                { 
                    ScheduledTime = now.AddHours(2), 
                    Status = "SCHEDULED" 
                }
            }
        };

        _batchRuleExecutionRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<BatchRuleExecution, bool>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IQueryable<BatchRuleExecution>>>(),
                It.IsAny<Func<IQueryable<BatchRuleExecution>, IOrderedQueryable<BatchRuleExecution>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetTopUpTypesAsync(ScheduledTopUpTypes.Individual, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(1);
        result[0].Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetTopUpTypesAsync_WithInvalidType_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _dashboardService.GetTopUpTypesAsync((ScheduledTopUpTypes)999, CancellationToken.None));
    }

    #endregion

    #region GetRecentActivitiesAsync Tests

    [Fact]
    public async Task GetRecentActivitiesAsync_ReturnsLatestEducationAccounts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<EducationAccount>
        {
            new()
            {
                CreatedAt = now.AddDays(-1),
                AccountHolder = new AccountHolder 
                { 
                    FirstName = "John", 
                    LastName = "Doe", 
                    Email = "john@test.com" 
                }
            },
            new()
            {
                CreatedAt = now.AddDays(-2),
                AccountHolder = new AccountHolder 
                { 
                    FirstName = "Jane", 
                    LastName = "Smith", 
                    Email = "jane@test.com" 
                }
            }
        };

        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
        result[0].Name.Should().Contain("John");
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_OrdersByCreatedAtDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<EducationAccount>
        {
            new()
            {
                CreatedAt = now,
                AccountHolder = new AccountHolder 
                { 
                    FirstName = "Latest", 
                    LastName = "Account", 
                    Email = "latest@test.com" 
                }
            },
            new()
            {
                CreatedAt = now.AddDays(-5),
                AccountHolder = new AccountHolder 
                { 
                    FirstName = "Older", 
                    LastName = "Account", 
                    Email = "older@test.com" 
                }
            }
        };

        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(CancellationToken.None);

        // Assert
        result.Should().BeInDescendingOrder(x => x.CreatedAt);
        result[0].Name.Should().Contain("Latest");
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_LimitTo10Results()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = Enumerable.Range(1, 10)
            .Select(i => new EducationAccount
            {
                CreatedAt = now.AddDays(-i),
                AccountHolder = new AccountHolder 
                { 
                    FirstName = $"Account{i}", 
                    LastName = "Test", 
                    Email = $"account{i}@test.com" 
                }
            })
            .ToList();

        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(CancellationToken.None);

        // Assert
        result.Count.Should().BeLessThanOrEqualTo(10);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_ReturnsEmptyWhenNoAccounts()
    {
        // Arrange
        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<EducationAccount>());

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_MapsAccountHolderDetailsCorrectly()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockData = new List<EducationAccount>
        {
            new()
            {
                CreatedAt = now,
                AccountHolder = new AccountHolder 
                { 
                    FirstName = "Test", 
                    LastName = "User", 
                    Email = "test@example.com" 
                }
            }
        };

        _educationAccountRepositoryMock.Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<EducationAccount, bool>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IQueryable<EducationAccount>>>(),
                It.IsAny<Func<IQueryable<EducationAccount>, IOrderedQueryable<EducationAccount>>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        // Act
        var result = await _dashboardService.GetRecentActivitiesAsync(CancellationToken.None);

        // Assert
        result[0].Name.Should().Be("Test User");
        result[0].Email.Should().Be("test@example.com");
        result[0].CreatedAt.Should().Be(now);
    }

    #endregion
}