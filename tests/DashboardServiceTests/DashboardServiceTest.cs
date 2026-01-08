using System;
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
    private readonly Mock<IGenericRepository<HistoryOfChange>> _historyRepositoryMock;
    private readonly Mock<IGenericRepository<Transaction>> _transactionRepositoryMock;
    private readonly Mock<IGenericRepository<Invoice>> _invoiceRepositoryMock;
    private readonly DashboardService _dashboardService;

    public DashboardServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _historyRepositoryMock = new Mock<IGenericRepository<HistoryOfChange>>();
        _transactionRepositoryMock = new Mock<IGenericRepository<Transaction>>();
        _invoiceRepositoryMock = new Mock<IGenericRepository<Invoice>>();
        
        _unitOfWorkMock.Setup(u => u.GetRepository<HistoryOfChange>()).Returns(_historyRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Transaction>()).Returns(_transactionRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.GetRepository<Invoice>()).Returns(_invoiceRepositoryMock.Object);
        
        _dashboardService = new DashboardService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithValidData_ReturnsCorrectOverview()
    {
        // Arrange
        var expectedTotalDisbursed = 3000m;
        var expectedTotalCollected = 1500m;
        var expectedOutstandingPayments = 800m;

        SetupRepositoryMocks(expectedTotalDisbursed, expectedTotalCollected, expectedOutstandingPayments);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            MOE_System.Application.Common.Dashboard.DateRangeType.AllTime,
            null,
            null);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(expectedTotalDisbursed);
        result.TotalCollected.Should().Be(expectedTotalCollected);
        result.OutstandingPayments.Should().Be(expectedOutstandingPayments);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithZeroAmounts_ReturnsZeroValues()
    {
        // Arrange
        SetupRepositoryMocks(0m, 0m, 0m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            MOE_System.Application.Common.Dashboard.DateRangeType.AllTime,
            null,
            null);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(0m);
        result.TotalCollected.Should().Be(0m);
        result.OutstandingPayments.Should().Be(0m);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithDecimalValues_HandlesCorrectly()
    {
        // Arrange
        var expectedTotalDisbursed = 1234.56m;
        var expectedTotalCollected = 987.65m;
        var expectedOutstandingPayments = 543.21m;

        SetupRepositoryMocks(expectedTotalDisbursed, expectedTotalCollected, expectedOutstandingPayments);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            MOE_System.Application.Common.Dashboard.DateRangeType.AllTime,
            null,
            null);

        // Assert
        result.TotalDisbursed.Should().Be(expectedTotalDisbursed);
        result.TotalCollected.Should().Be(expectedTotalCollected);
        result.OutstandingPayments.Should().Be(expectedOutstandingPayments);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_CallsAllRepositories()
    {
        // Arrange
        SetupRepositoryMocks(100m, 200m, 300m);

        // Act
        await _dashboardService.GetDashboardOverviewAsync(
            MOE_System.Application.Common.Dashboard.DateRangeType.AllTime,
            null,
            null);

        // Assert - Verify all repositories are called
        _unitOfWorkMock.Verify(u => u.GetRepository<HistoryOfChange>(), Times.Once);
        _unitOfWorkMock.Verify(u => u.GetRepository<Transaction>(), Times.Once);
        _unitOfWorkMock.Verify(u => u.GetRepository<Invoice>(), Times.Once);
        
        _historyRepositoryMock.Verify(
            r => r.SumAsync(
                It.IsAny<Expression<Func<HistoryOfChange, bool>>>(),
                It.IsAny<Expression<Func<HistoryOfChange, decimal>>>()), 
            Times.Once);
            
        _transactionRepositoryMock.Verify(
            r => r.SumAsync(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<Expression<Func<Transaction, decimal>>>()), 
            Times.Once);
            
        _invoiceRepositoryMock.Verify(
            r => r.SumAsync(
                It.IsAny<Expression<Func<Invoice, bool>>>(),
                It.IsAny<Expression<Func<Invoice, decimal>>>()), 
            Times.Once);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_ReturnsCorrectType()
    {
        // Arrange
        SetupRepositoryMocks(100m, 200m, 300m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            MOE_System.Application.Common.Dashboard.DateRangeType.AllTime,
            null,
            null);

        // Assert
        result.Should().BeOfType<DashboardOverviewResponse>();
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WhenRepositoryThrowsException_ShouldPropagate()
    {
        // Arrange
        _historyRepositoryMock.Setup(r => r.SumAsync(
            It.IsAny<Expression<Func<HistoryOfChange, bool>>>(),
            It.IsAny<Expression<Func<HistoryOfChange, decimal>>>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _dashboardService.GetDashboardOverviewAsync(
                MOE_System.Application.Common.Dashboard.DateRangeType.AllTime,
                null,
                null));
        exception.Message.Should().Be("Database error");
    }

    #region Date Filter Tests

    [Fact]
    public async Task GetDashboardOverviewAsync_WithAllTimeFilter_DoesNotFilterByDate()
    {
        // Arrange
        SetupRepositoryMocks(1000m, 500m, 200m);

        // Act
        await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.AllTime,
            null,
            null);

        // Assert - Verify predicates don't include date filtering
        _historyRepositoryMock.Verify(
            r => r.SumAsync(
                It.Is<Expression<Func<HistoryOfChange, bool>>>(expr => 
                    expr.ToString().Contains("Type") && expr.ToString().Contains("Top-Up")),
                It.IsAny<Expression<Func<HistoryOfChange, decimal>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithThisMonthFilter_FiltersCurrentMonth()
    {
        // Arrange
        SetupRepositoryMocks(2000m, 1000m, 300m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.ThisMonth,
            null,
            null);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(2000m);
        result.TotalCollected.Should().Be(1000m);
        
        // Verify date filtering is applied
        _historyRepositoryMock.Verify(
            r => r.SumAsync(
                It.IsAny<Expression<Func<HistoryOfChange, bool>>>(),
                It.IsAny<Expression<Func<HistoryOfChange, decimal>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithLastMonthFilter_FiltersLastMonth()
    {
        // Arrange
        SetupRepositoryMocks(1500m, 800m, 250m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.LastMonth,
            null,
            null);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(1500m);
        result.TotalCollected.Should().Be(800m);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithLast30DaysFilter_Filters30Days()
    {
        // Arrange
        SetupRepositoryMocks(3500m, 2200m, 600m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.Last30Days,
            null,
            null);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(3500m);
        result.TotalCollected.Should().Be(2200m);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithLast3MonthsFilter_Filters3Months()
    {
        // Arrange
        SetupRepositoryMocks(8000m, 4500m, 1200m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.Last3Months,
            null,
            null);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(8000m);
        result.TotalCollected.Should().Be(4500m);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithLast6MonthsFilter_Filters6Months()
    {
        // Arrange
        SetupRepositoryMocks(15000m, 9000m, 2500m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.Last6Months,
            null,
            null);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(15000m);
        result.TotalCollected.Should().Be(9000m);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithCustomFilter_ValidDates_FiltersCustomRange()
    {
        // Arrange
        var fromDate = new DateOnly(2026, 1, 1);
        var toDate = new DateOnly(2026, 1, 31);
        SetupRepositoryMocks(5000m, 3000m, 800m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.Custom,
            fromDate,
            toDate);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(5000m);
        result.TotalCollected.Should().Be(3000m);
        result.OutstandingPayments.Should().Be(800m);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithCustomFilter_MissingFromDate_ThrowsException()
    {
        // Arrange
        var toDate = new DateOnly(2026, 1, 31);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _dashboardService.GetDashboardOverviewAsync(
                DateRangeType.Custom,
                null,
                toDate));

        exception.Message.Should().Contain("Invalid date range type or missing from/to dates for custom range");
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithCustomFilter_MissingToDate_ThrowsException()
    {
        // Arrange
        var fromDate = new DateOnly(2026, 1, 1);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _dashboardService.GetDashboardOverviewAsync(
                DateRangeType.Custom,
                fromDate,
                null));

        exception.Message.Should().Contain("Invalid date range type or missing from/to dates for custom range");
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithCustomFilter_MissingBothDates_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => _dashboardService.GetDashboardOverviewAsync(
                DateRangeType.Custom,
                null,
                null));

        exception.Message.Should().Contain("Invalid date range type or missing from/to dates for custom range");
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithCustomFilter_SingleDay_IncludesFullDay()
    {
        // Arrange - Same from and to date should include the entire day
        var date = new DateOnly(2026, 1, 15);
        SetupRepositoryMocks(1200m, 600m, 150m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.Custom,
            date,
            date);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(1200m);
        result.TotalCollected.Should().Be(600m);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_FiltersOnlyTopUpTransactions()
    {
        // Arrange
        SetupRepositoryMocks(2500m, 1500m, 400m);

        // Act
        await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.AllTime,
            null,
            null);

        // Assert - Verify Type == "Top-Up" filter is applied
        _historyRepositoryMock.Verify(
            r => r.SumAsync(
                It.Is<Expression<Func<HistoryOfChange, bool>>>(expr => 
                    expr.ToString().Contains("Top-Up")),
                It.IsAny<Expression<Func<HistoryOfChange, decimal>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_FiltersOnlyCompletedTransactions()
    {
        // Arrange
        SetupRepositoryMocks(3000m, 2000m, 500m);

        // Act
        await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.AllTime,
            null,
            null);

        // Assert - Verify Status == "Completed" filter is applied
        _transactionRepositoryMock.Verify(
            r => r.SumAsync(
                It.Is<Expression<Func<Transaction, bool>>>(expr => 
                    expr.ToString().Contains("Completed")),
                It.IsAny<Expression<Func<Transaction, decimal>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_OutstandingPayments_AlwaysUnpaidOnly()
    {
        // Arrange
        SetupRepositoryMocks(5000m, 3000m, 1500m);

        // Act - Even with date filter, outstanding payments should only filter by Unpaid status
        await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.ThisMonth,
            null,
            null);

        // Assert - Verify Status == "Unpaid" filter is applied without date filtering
        _invoiceRepositoryMock.Verify(
            r => r.SumAsync(
                It.Is<Expression<Func<Invoice, bool>>>(expr => 
                    expr.ToString().Contains("Unpaid")),
                It.IsAny<Expression<Func<Invoice, decimal>>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_PassesCancellationToken()
    {
        // Arrange
        SetupRepositoryMocks(1000m, 500m, 200m);
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.AllTime,
            null,
            null,
            cancellationToken);

        // Assert - Verify cancellation token is passed to all repositories
        _historyRepositoryMock.Verify(
            r => r.SumAsync(
                It.IsAny<Expression<Func<HistoryOfChange, bool>>>(),
                It.IsAny<Expression<Func<HistoryOfChange, decimal>>>(),
                cancellationToken),
            Times.Once);

        _transactionRepositoryMock.Verify(
            r => r.SumAsync(
                It.IsAny<Expression<Func<Transaction, bool>>>(),
                It.IsAny<Expression<Func<Transaction, decimal>>>(),
                cancellationToken),
            Times.Once);

        _invoiceRepositoryMock.Verify(
            r => r.SumAsync(
                It.IsAny<Expression<Func<Invoice, bool>>>(),
                It.IsAny<Expression<Func<Invoice, decimal>>>(),
                cancellationToken),
            Times.Once);
    }

    [Theory]
    [InlineData(DateRangeType.ThisMonth)]
    [InlineData(DateRangeType.LastMonth)]
    [InlineData(DateRangeType.Last30Days)]
    [InlineData(DateRangeType.Last3Months)]
    [InlineData(DateRangeType.Last6Months)]
    public async Task GetDashboardOverviewAsync_AllPredefinedFilters_ReturnValidResponse(DateRangeType dateRangeType)
    {
        // Arrange
        SetupRepositoryMocks(1000m, 600m, 300m);

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            dateRangeType,
            null,
            null);

        // Assert
        result.Should().NotBeNull();
        result.TotalDisbursed.Should().Be(1000m);
        result.TotalCollected.Should().Be(600m);
        result.OutstandingPayments.Should().Be(300m);
    }

    [Fact]
    public async Task GetDashboardOverviewAsync_WithDateFilter_ResponseIncludesTimestamp()
    {
        // Arrange
        SetupRepositoryMocks(2000m, 1200m, 400m);
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = await _dashboardService.GetDashboardOverviewAsync(
            DateRangeType.ThisMonth,
            null,
            null);

        var afterCall = DateTime.UtcNow;

        // Assert
        result.Should().NotBeNull();
        result.OutstandingAsOfUtc.Should().BeOnOrAfter(beforeCall);
        result.OutstandingAsOfUtc.Should().BeOnOrBefore(afterCall);
    }

    #endregion

    private void SetupRepositoryMocks(decimal totalDisbursed, decimal totalCollected, decimal outstandingPayments)
    {
        _historyRepositoryMock.Setup(r => r.SumAsync(
            It.IsAny<Expression<Func<HistoryOfChange, bool>>>(),
            It.IsAny<Expression<Func<HistoryOfChange, decimal>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalDisbursed);

        _transactionRepositoryMock.Setup(r => r.SumAsync(
            It.IsAny<Expression<Func<Transaction, bool>>>(),
            It.IsAny<Expression<Func<Transaction, decimal>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalCollected);

        _invoiceRepositoryMock.Setup(r => r.SumAsync(
            It.IsAny<Expression<Func<Invoice, bool>>>(),
            It.IsAny<Expression<Func<Invoice, decimal>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(outstandingPayments);
    }
}