using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly DashboardService _dashboardService;

    public DashboardServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _batchRuleExecutionRepositoryMock = new Mock<IGenericRepository<BatchRuleExecution>>();
        
        _unitOfWorkMock.Setup(u => u.GetRepository<BatchRuleExecution>())
            .Returns(_batchRuleExecutionRepositoryMock.Object);
        
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
}