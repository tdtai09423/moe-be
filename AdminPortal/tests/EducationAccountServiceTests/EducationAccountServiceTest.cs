using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.Interfaces.Services;
using MOE_System.Application.Services;
using MOE_System.Domain.Entities;
using Xunit;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.Application.Tests.EducationAccountServiceTests;

public class EducationAccountServiceTest
{
    #region Setup and Mocks

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<AccountHolder>> _accountHolderRepositoryMock;
    private readonly Mock<IClock> _clockMock;
    private readonly Mock<IOptions<AccountClosureOptions>> _optionsMock;
    private readonly IEducationAccountService _service;

    public EducationAccountServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _accountHolderRepositoryMock = new Mock<IGenericRepository<AccountHolder>>();
        _clockMock = new Mock<IClock>();
        _optionsMock = new Mock<IOptions<AccountClosureOptions>>();

        _unitOfWorkMock.Setup(u => u.GetRepository<AccountHolder>())
            .Returns(_accountHolderRepositoryMock.Object);

        // Initialize service with default - will be re-created in tests that need different options
        var defaultOptions = new AccountClosureOptions
        {
            Enabled = true,
            AgeThreshold = 30,
            ProcessingDay = 1,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        _optionsMock.Setup(o => o.Value).Returns(defaultOptions);
        _clockMock.Setup(c => c.TodayInTimeZone(It.IsAny<string>()))
            .Returns(new DateOnly(2026, 1, 15));

        _service = new EducationAccountService(_unitOfWorkMock.Object, _clockMock.Object, _optionsMock.Object);
    }

    private IEducationAccountService CreateService(AccountClosureOptions options, DateOnly? today = null)
    {
        var optionsMock = new Mock<IOptions<AccountClosureOptions>>();
        optionsMock.Setup(o => o.Value).Returns(options);

        var clockMock = new Mock<IClock>();
        clockMock.Setup(c => c.TodayInTimeZone(It.IsAny<string>()))
            .Returns(today ?? new DateOnly(2026, 1, 15));

        return new EducationAccountService(_unitOfWorkMock.Object, clockMock.Object, optionsMock.Object);
    }

    private AccountHolder CreateAccountHolder(
        string nric = "S1234567A",
        bool hasEducationAccount = true,
        bool isActive = true,
        DateTime? closedDate = null,
        DateTime? dateOfBirth = null)
    {
        var holder = new AccountHolder
        {
            Id = Guid.NewGuid().ToString(),
            FirstName = "John",
            LastName = "Doe",
            NRIC = nric,
            DateOfBirth = dateOfBirth ?? new DateTime(2000, 1, 15),
            Email = "john.doe@email.com",
            ContactNumber = "91234567",
            SchoolingStatus = "In School",
            EducationLevel = "Secondary",
            CreatedAt = DateTime.UtcNow
        };

        if (hasEducationAccount)
        {
            holder.EducationAccount = new EducationAccount
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "johndoe",
                Password = "hashedpassword",
                Balance = 500m,
                IsActive = isActive,
                ClosedDate = closedDate,
                AccountHolderId = holder.Id,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow.AddDays(-1)
            };
        }

        return holder;
    }

    #endregion

    #region CloseEducationAccountManuallyAsync Tests

    [Fact]
    public async Task CloseEducationAccountManuallyAsync_WithValidNRIC_ClosesAccount()
    {
        // Arrange
        var nric = "S1234567A";
        var holder = CreateAccountHolder(nric: nric, isActive: true);

        _accountHolderRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(holder);

        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.CloseEducationAccountManuallyAsync(nric, CancellationToken.None);

        // Assert
        holder.EducationAccount!.IsActive.Should().BeFalse();
        holder.EducationAccount.ClosedDate.Should().NotBeNull();
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CloseEducationAccountManuallyAsync_WithInvalidNRIC_ThrowsNotFoundException()
    {
        // Arrange
        var nric = "S1234567A";
        
        _accountHolderRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccountHolder?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CloseEducationAccountManuallyAsync(nric, CancellationToken.None));

        exception.Message.Should().Contain("Education account holder not found");
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CloseEducationAccountManuallyAsync_WithoutEducationAccount_ThrowsNotFoundException()
    {
        // Arrange
        var nric = "S1234567A";
        var holder = CreateAccountHolder(nric: nric, hasEducationAccount: false);

        _accountHolderRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(holder);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.CloseEducationAccountManuallyAsync(nric, CancellationToken.None));

        exception.Message.Should().Contain("Education account holder not found");
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task CloseEducationAccountManuallyAsync_WithAlreadyClosedAccount_SetClosedDateAgain()
    {
        // Arrange
        var nric = "S1234567A";
        var previousClosedDate = DateTime.UtcNow.AddDays(-30);
        var holder = CreateAccountHolder(nric: nric, isActive: false, closedDate: previousClosedDate);

        _accountHolderRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(holder);

        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.CloseEducationAccountManuallyAsync(nric, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task CloseEducationAccountManuallyAsync_WithCancellationToken_CancelsOperation()
    {
        // Arrange
        var nric = "S1234567A";
        var cancellationToken = new CancellationToken(canceled: true);

        _accountHolderRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<bool>(),
                cancellationToken))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _service.CloseEducationAccountManuallyAsync(nric, cancellationToken));
    }

    #endregion

    #region AutoCloseEducationAccountsAsync Tests

    [Fact]
    public async Task AutoCloseEducationAccountsAsync_WhenDisabled_DoesNothing()
    {
        // Arrange
        var options = new AccountClosureOptions
        {
            Enabled = false,
            AgeThreshold = 30,
            ProcessingDay = 1,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        var service = CreateService(options);

        // Act
        await service.AutoCloseEducationAccountsAsync(CancellationToken.None);

        // Assert
        _accountHolderRepositoryMock.Verify(
            r => r.ToListAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IOrderedQueryable<AccountHolder>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task AutoCloseEducationAccountsAsync_WhenBeforeScheduledDate_DoesNothing()
    {
        // Arrange
        var today = new DateOnly(2026, 1, 10);
        var options = new AccountClosureOptions
        {
            Enabled = true,
            AgeThreshold = 30,
            ProcessingDay = 15,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        var service = CreateService(options, today);

        // Act
        await service.AutoCloseEducationAccountsAsync(CancellationToken.None);

        // Assert
        _accountHolderRepositoryMock.Verify(
            r => r.ToListAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IOrderedQueryable<AccountHolder>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Never);
    }

    [Fact]
    public async Task AutoCloseEducationAccountsAsync_WhenScheduledDateArrived_ProcessesEligibleAccounts()
    {
        // Arrange
        var today = new DateOnly(2026, 1, 15);
        var options = new AccountClosureOptions
        {
            Enabled = true,
            AgeThreshold = 30,
            ProcessingDay = 15,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        var service = CreateService(options, today);

        // Create account holders with different ages
        // Eligible: born in 1996, will be 30 in 2026
        var eligibleHolder = CreateAccountHolder(
            nric: "S9601001A",
            dateOfBirth: new DateTime(1996, 1, 15),
            isActive: true
        );

        // Ineligible: born in 1997, will be 29 in 2026
        var ineligibleHolder = CreateAccountHolder(
            nric: "S9701001B",
            dateOfBirth: new DateTime(1997, 1, 15),
            isActive: true
        );

        // Eligible but already closed
        var closedHolder = CreateAccountHolder(
            nric: "S9501001C",
            dateOfBirth: new DateTime(1995, 1, 15),
            isActive: false,
            closedDate: DateTime.UtcNow.AddDays(-30)
        );

        var holders = new List<AccountHolder> { eligibleHolder, ineligibleHolder, closedHolder };

        _accountHolderRepositoryMock
            .Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IOrderedQueryable<AccountHolder>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(holders);

        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        await service.AutoCloseEducationAccountsAsync(CancellationToken.None);

        // Assert
        eligibleHolder.EducationAccount!.IsActive.Should().BeFalse();
        eligibleHolder.EducationAccount.ClosedDate.Should().NotBeNull();

        ineligibleHolder.EducationAccount!.IsActive.Should().BeTrue();
        ineligibleHolder.EducationAccount.ClosedDate.Should().BeNull();

        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AutoCloseEducationAccountsAsync_WhenNoEligibleAccounts_DoesNothing()
    {
        // Arrange
        var today = new DateOnly(2026, 1, 15);
        var options = new AccountClosureOptions
        {
            Enabled = true,
            AgeThreshold = 30,
            ProcessingDay = 15,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        var service = CreateService(options, today);

        var ineligibleHolder = CreateAccountHolder(
            nric: "S0501001A",
            dateOfBirth: new DateTime(2005, 1, 15),
            isActive: true
        );

        _accountHolderRepositoryMock
            .Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IOrderedQueryable<AccountHolder>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AccountHolder> { ineligibleHolder });

        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        await service.AutoCloseEducationAccountsAsync(CancellationToken.None);

        // Assert
        ineligibleHolder.EducationAccount!.IsActive.Should().BeTrue();
        ineligibleHolder.EducationAccount.ClosedDate.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AutoCloseEducationAccountsAsync_WithMultipleEligibleAccounts_CloseAll()
    {
        // Arrange
        var today = new DateOnly(2026, 1, 15);
        var options = new AccountClosureOptions
        {
            Enabled = true,
            AgeThreshold = 30,
            ProcessingDay = 15,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        var service = CreateService(options, today);

        var holders = new List<AccountHolder>();
        // Create 5 holders all aged 30+ (born in 1996 or earlier)
        for (int i = 0; i < 5; i++)
        {
            holders.Add(CreateAccountHolder(
                nric: $"S{1996 - i:D4}001{(char)('A' + i)}",
                dateOfBirth: new DateTime(1996 - i, 1, 15),
                isActive: true
            ));
        }

        _accountHolderRepositoryMock
            .Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IOrderedQueryable<AccountHolder>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(holders);

        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        await service.AutoCloseEducationAccountsAsync(CancellationToken.None);

        // Assert
        foreach (var holder in holders)
        {
            holder.EducationAccount!.IsActive.Should().BeFalse();
            holder.EducationAccount.ClosedDate.Should().NotBeNull();
        }
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AutoCloseEducationAccountsAsync_WithEmptyList_SavesWithoutProcessing()
    {
        // Arrange
        var today = new DateOnly(2026, 1, 15);
        var options = new AccountClosureOptions
        {
            Enabled = true,
            AgeThreshold = 30,
            ProcessingDay = 15,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        var service = CreateService(options, today);

        _accountHolderRepositoryMock
            .Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IOrderedQueryable<AccountHolder>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AccountHolder>());

        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        await service.AutoCloseEducationAccountsAsync(CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AutoCloseEducationAccountsAsync_OnScheduledDate_ProcessesCorrectly()
    {
        // Arrange
        var today = new DateOnly(2026, 1, 15);
        var options = new AccountClosureOptions
        {
            Enabled = true,
            AgeThreshold = 30,
            ProcessingDay = 15,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        var service = CreateService(options, today);

        var holder = CreateAccountHolder(
            nric: "S9601001A",
            dateOfBirth: new DateTime(1996, 1, 15),
            isActive: true
        );

        _accountHolderRepositoryMock
            .Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IOrderedQueryable<AccountHolder>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AccountHolder> { holder });

        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        await service.AutoCloseEducationAccountsAsync(CancellationToken.None);

        // Assert
        holder.EducationAccount!.IsActive.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    [Fact]
    public async Task AutoCloseEducationAccountsAsync_WithMixedStates_OnlyClosesEligibleActive()
    {
        // Arrange
        var today = new DateOnly(2026, 1, 15);
        var options = new AccountClosureOptions
        {
            Enabled = true,
            AgeThreshold = 30,
            ProcessingDay = 15,
            ProcessingMonth = 1,
            TimeZone = "Singapore Standard Time"
        };
        var service = CreateService(options, today);

        // Eligible and active - should close (born 1996, age 30)
        var activeEligible = CreateAccountHolder(
            nric: "S9601001A",
            dateOfBirth: new DateTime(1996, 1, 15),
            isActive: true
        );

        // Eligible but inactive - should skip (born 1995, age 31, but already closed)
        var inactiveEligible = CreateAccountHolder(
            nric: "S9501001B",
            dateOfBirth: new DateTime(1995, 6, 15),
            isActive: false,
            closedDate: DateTime.UtcNow.AddDays(-10)
        );

        // Eligible and active with ClosedDate null - should close (born 1996, age 30)
        var activeWithNullClosed = CreateAccountHolder(
            nric: "S9601001C",
            dateOfBirth: new DateTime(1996, 3, 15),
            isActive: true,
            closedDate: null
        );

        var holders = new List<AccountHolder> { activeEligible, inactiveEligible, activeWithNullClosed };

        _accountHolderRepositoryMock
            .Setup(r => r.ToListAsync(
                It.IsAny<Expression<Func<AccountHolder, bool>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IQueryable<AccountHolder>>>(),
                It.IsAny<Func<IQueryable<AccountHolder>, IOrderedQueryable<AccountHolder>>>(),
                It.IsAny<int>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(holders);

        _unitOfWorkMock.Setup(u => u.SaveAsync()).Returns(Task.CompletedTask);

        // Act
        await service.AutoCloseEducationAccountsAsync(CancellationToken.None);

        // Assert
        activeEligible.EducationAccount!.IsActive.Should().BeFalse();
        activeWithNullClosed.EducationAccount!.IsActive.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
    }

    #endregion
}
