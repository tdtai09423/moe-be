using FluentAssertions;
using Moq;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs.AccountHolder.Request;
using MOE_System.Application.DTOs.AccountHolder.Response;
using MOE_System.Application.Interfaces;
using MOE_System.Application.Services;
using MOE_System.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.Admin.UnitTest;

public class AccountHolderServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<IGenericRepository<AccountHolder>> _mockAccountHolderRepo;
    private readonly Mock<IGenericRepository<EducationAccount>> _mockEducationAccountRepo;
    private readonly AccountHolderService _service;

    public AccountHolderServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockAccountHolderRepo = new Mock<IGenericRepository<AccountHolder>>();
        _mockEducationAccountRepo = new Mock<IGenericRepository<EducationAccount>>();

        _mockUnitOfWork.Setup(u => u.GetRepository<AccountHolder>())
            .Returns(_mockAccountHolderRepo.Object);
        _mockUnitOfWork.Setup(u => u.GetRepository<EducationAccount>())
            .Returns(_mockEducationAccountRepo.Object);

        _service = new AccountHolderService(_mockUnitOfWork.Object, _mockPasswordService.Object);
    }

    #region GetAccountHolderDetailAsync Tests

    [Fact]
    public async Task GetAccountHolderDetailAsync_WhenAccountHolderExists_ReturnsAccountHolderDetail()
    {
        // Arrange
        var accountHolderId = Guid.NewGuid().ToString();
        var accountHolder = new AccountHolder
        {
            Id = accountHolderId,
            FirstName = "John",
            LastName = "Doe",
            NRIC = "S1234567A",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "john.doe@email.com",
            ContactNumber = "91234567",
            SchoolingStatus = "In School",
            EducationLevel = "Secondary",
            CreatedAt = DateTime.UtcNow,
            EducationAccount = new EducationAccount
            {
                Balance = 1000,
                Enrollments = new List<Enrollment>()
            }
        };

        _mockAccountHolderRepo.Setup(r => r.GetByIdAsync(accountHolderId))
            .ReturnsAsync(accountHolder);

        // Act
        var result = await _service.GetAccountHolderDetailAsync(accountHolderId);

        // Assert
        result.Should().NotBeNull();
        result.Balance.Should().Be(1000);
        result.CourseCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAccountHolderDetailAsync_WhenAccountHolderNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var accountHolderId = Guid.NewGuid().ToString();
        _mockAccountHolderRepo.Setup(r => r.GetByIdAsync(accountHolderId))
            .ReturnsAsync((AccountHolder?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetAccountHolderDetailAsync(accountHolderId));

        exception.Message.Should().Contain("Account holder with ID");
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetAccountHolderDetailAsync_WhenNoEducationAccount_ReturnsZeroBalance()
    {
        // Arrange
        var accountHolderId = Guid.NewGuid().ToString();
        var accountHolder = new AccountHolder
        {
            Id = accountHolderId,
            FirstName = "Jane",
            LastName = "Smith",
            NRIC = "S7654321B",
            DateOfBirth = new DateTime(1995, 5, 15),
            EducationAccount = null
        };

        _mockAccountHolderRepo.Setup(r => r.GetByIdAsync(accountHolderId))
            .ReturnsAsync(accountHolder);

        // Act
        var result = await _service.GetAccountHolderDetailAsync(accountHolderId);

        // Assert
        result.Balance.Should().Be(0);
        result.CourseCount.Should().Be(0);
        result.OutstandingFees.Should().Be(0);
    }

    #endregion

    #region GetAccountHoldersAsync Tests

    [Fact]
    public async Task GetAccountHoldersAsync_ReturnsAllAccountHolders()
    {
        // Arrange
        var accountHolders = new List<AccountHolder>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "John",
                LastName = "Doe",
                NRIC = "S1234567A",
                DateOfBirth = new DateTime(1990, 1, 1),
                SchoolingStatus = "In School",
                EducationLevel = "Secondary",
                CreatedAt = DateTime.UtcNow,
                EducationAccount = new EducationAccount { Balance = 500, Enrollments = new List<Enrollment>() }
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Jane",
                LastName = "Smith",
                NRIC = "S7654321B",
                DateOfBirth = new DateTime(1995, 5, 15),
                SchoolingStatus = "Not in School",
                EducationLevel = "Tertiary",
                CreatedAt = DateTime.UtcNow,
                EducationAccount = new EducationAccount { Balance = 1000, Enrollments = new List<Enrollment>() }
            }
        }.AsQueryable();

        var paginatedList = new PaginatedList<AccountHolder>(
            accountHolders.ToList(), 
            2, 
            1, 
            20);

        _mockAccountHolderRepo.Setup(r => r.GetPagging(
                It.IsAny<IQueryable<AccountHolder>>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _service.GetAccountHoldersAsync(1, 20);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(2);
        result.TotalCount.Should().Be(2);
        result.Items[0].FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetAccountHoldersAsync_WhenNoAccountHolders_ReturnsEmptyList()
    {
        // Arrange
        var paginatedList = new PaginatedList<AccountHolder>(
            new List<AccountHolder>(), 
            0, 
            1, 
            20);

        _mockAccountHolderRepo.Setup(r => r.GetPagging(
                It.IsAny<IQueryable<AccountHolder>>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _service.GetAccountHoldersAsync(1, 20);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetAccountHoldersAsync_CalculatesAgeCorrectly()
    {
        // Arrange
        var birthYear = DateTime.Now.Year - 30;
        var accountHolders = new List<AccountHolder>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Test",
                LastName = "User",
                NRIC = "S1234567A",
                DateOfBirth = new DateTime(birthYear, 1, 1),
                CreatedAt = DateTime.UtcNow,
                EducationAccount = null
            }
        }.AsQueryable();

        var paginatedList = new PaginatedList<AccountHolder>(
            accountHolders.ToList(), 
            1, 
            1, 
            20);

        _mockAccountHolderRepo.Setup(r => r.GetPagging(
                It.IsAny<IQueryable<AccountHolder>>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _service.GetAccountHoldersAsync(1, 20);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items[0].Age.Should().Be(30);
    }

    [Fact]
    public async Task GetAccountHoldersAsync_ReturnsPaginatedResults()
    {
        // Arrange
        var accountHolders = Enumerable.Range(1, 25)
            .Select(i => new AccountHolder
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = $"User{i}",
                LastName = "Test",
                NRIC = $"S{i:D7}A",
                DateOfBirth = new DateTime(1990, 1, 1),
                CreatedAt = DateTime.UtcNow,
                EducationAccount = new EducationAccount { Balance = 100 * i }
            })
            .AsQueryable();

        var pageSize = 10;
        var page1 = accountHolders.Take(pageSize).ToList();
        var paginatedList = new PaginatedList<AccountHolder>(page1, 25, 1, pageSize);

        _mockAccountHolderRepo.Setup(r => r.GetPagging(
                It.IsAny<IQueryable<AccountHolder>>(), 
                1, 
                pageSize))
            .ReturnsAsync(paginatedList);

        // Act
        var result = await _service.GetAccountHoldersAsync(1, pageSize);

        // Assert
        result.Items.Count.Should().Be(pageSize);
        result.TotalCount.Should().Be(25);
        result.PageIndex.Should().Be(1);
    }

    #endregion

    #region AddAccountHolderAsync Tests

    [Fact]
    public async Task AddAccountHolderAsync_CreatesAccountHolderSuccessfully()
    {
        // Arrange
        var request = new CreateAccountHolderRequest
        {
            NRIC = "S1234567A",
            FullName = "John Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "john.doe@email.com",
            ContactNumber = "91234567"
        };

        AccountHolder? capturedAccountHolder = null;
        EducationAccount? capturedEducationAccount = null;

        _mockAccountHolderRepo.Setup(r => r.Entities)
            .Returns(new List<AccountHolder>().AsQueryable());

        _mockAccountHolderRepo.Setup(r => r.InsertAsync(It.IsAny<AccountHolder>()))
            .Callback<AccountHolder>(ah => capturedAccountHolder = ah)
            .Returns(Task.CompletedTask);

        _mockEducationAccountRepo.Setup(r => r.InsertAsync(It.IsAny<EducationAccount>()))
            .Callback<EducationAccount>(ea => capturedEducationAccount = ea)
            .Returns(Task.CompletedTask);

        _mockPasswordService.Setup(p => p.GenerateRandomPassword(It.IsAny<int>()))
            .Returns("TempPassword123");

        _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns("HashedPassword");

        _mockUnitOfWork.Setup(u => u.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddAccountHolderAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be("John Doe");
        result.NRIC.Should().Be("S1234567A");
        
        _mockAccountHolderRepo.Verify(r => r.InsertAsync(It.IsAny<AccountHolder>()), Times.Once);
        _mockEducationAccountRepo.Verify(r => r.InsertAsync(It.IsAny<EducationAccount>()), Times.Once);
    }

    [Fact]
    public async Task AddAccountHolderAsync_WhenNRICAlreadyExists_ThrowsValidationException()
    {
        // Arrange
        var request = new CreateAccountHolderRequest
        {
            NRIC = "S1234567A",
            FullName = "John Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "john.doe@email.com",
            ContactNumber = "91234567"
        };

        var existingAccountHolder = new AccountHolder
        {
            NRIC = "S1234567A",
            FirstName = "Jane",
            LastName = "Smith"
        };

        _mockAccountHolderRepo.Setup(r => r.Entities)
            .Returns(new List<AccountHolder> { existingAccountHolder }.AsQueryable());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(
            () => _service.AddAccountHolderAsync(request));

        exception.Message.Should().Contain("already exists");
    }

    [Fact]
    public async Task AddAccountHolderAsync_ParsesFullNameCorrectly()
    {
        // Arrange
        var request = new CreateAccountHolderRequest
        {
            NRIC = "S1234567A",
            FullName = "John Michael Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "john.doe@email.com",
            ContactNumber = "91234567"
        };

        AccountHolder? capturedAccountHolder = null;

        _mockAccountHolderRepo.Setup(r => r.Entities)
            .Returns(new List<AccountHolder>().AsQueryable());

        _mockAccountHolderRepo.Setup(r => r.InsertAsync(It.IsAny<AccountHolder>()))
            .Callback<AccountHolder>(ah => capturedAccountHolder = ah)
            .Returns(Task.CompletedTask);

        _mockEducationAccountRepo.Setup(r => r.InsertAsync(It.IsAny<EducationAccount>()))
            .Returns(Task.CompletedTask);

        _mockPasswordService.Setup(p => p.GenerateRandomPassword(It.IsAny<int>()))
            .Returns("TempPassword123");

        _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>()))
            .Returns("HashedPassword");

        _mockUnitOfWork.Setup(u => u.SaveAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.AddAccountHolderAsync(request);

        // Assert
        capturedAccountHolder.Should().NotBeNull();
        capturedAccountHolder!.FirstName.Should().Be("John");
        capturedAccountHolder.LastName.Should().Be("Michael Doe");
    }

    #endregion

    #region GetResidentAccountHolderByNRICAsync Tests

    [Fact]
    public async Task GetResidentAccountHolderByNRICAsync_WhenResidentExists_ReturnsResidentInfo()
    {
        // Arrange
        var nric = "S1234567A";
        var resident = new Resident
        {
            NRIC = nric,
            PrincipalName = "John Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            EmailAddress = "john@test.com",
            MobileNumber = "91234567",
            RegisteredAddress = "123 Test Street"
        };

        var residentRepo = new Mock<IGenericRepository<Resident>>();
        residentRepo.Setup(r => r.Entities)
            .Returns(new List<Resident> { resident }.AsQueryable());

        _mockUnitOfWork.Setup(u => u.GetRepository<Resident>())
            .Returns(residentRepo.Object);

        // Act
        var result = await _service.GetResidentAccountHolderByNRICAsync(nric);

        // Assert
        result.Should().NotBeNull();
        result.FullName.Should().Be("John Doe");
        result.Email.Should().Be("john@test.com");
    }

    [Fact]
    public async Task GetResidentAccountHolderByNRICAsync_WhenResidentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var nric = "S9999999Z";

        var residentRepo = new Mock<IGenericRepository<Resident>>();
        residentRepo.Setup(r => r.Entities)
            .Returns(new List<Resident>().AsQueryable());

        _mockUnitOfWork.Setup(u => u.GetRepository<Resident>())
            .Returns(residentRepo.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _service.GetResidentAccountHolderByNRICAsync(nric));

        exception.Message.Should().Contain("not found");
    }

    #endregion
}
