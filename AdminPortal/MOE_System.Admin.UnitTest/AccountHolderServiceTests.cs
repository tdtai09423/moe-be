using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using MOE_System.Application.DTOs;
using MOE_System.Application.Services;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.Interfaces;
using MOE_System.Domain.Entities;
using static MOE_System.Domain.Common.BaseException;

namespace MOE_System.Admin.UnitTest
{
    public class AccountHolderServiceTests
    {
        //        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        //        private readonly Mock<IPasswordService> _mockPasswordService;
        //        private readonly Mock<IGenericRepository<AccountHolder>> _mockAccountHolderRepo;
        //        private readonly Mock<IGenericRepository<EducationAccount>> _mockEducationAccountRepo;
        //        private readonly Mock<IDbContextTransaction> _mockTransaction;
        //        private readonly AccountHolderService _service;

        //        public AccountHolderServiceTests()
        //        {
        //            _mockUnitOfWork = new Mock<IUnitOfWork>();
        //            _mockPasswordService = new Mock<IPasswordService>();  
        //            _mockAccountHolderRepo = new Mock<IGenericRepository<AccountHolder>>();
        //            _mockEducationAccountRepo = new Mock<IGenericRepository<EducationAccount>>();
        //            _mockTransaction = new Mock<IDbContextTransaction>();

        //            _mockUnitOfWork.Setup(u => u.GetRepository<AccountHolder>())
        //                .Returns(_mockAccountHolderRepo.Object);
        //            _mockUnitOfWork.Setup(u => u.GetRepository<EducationAccount>())
        //                .Returns(_mockEducationAccountRepo.Object);

        //            //_service = new AccountHolderService(_mockUnitOfWork.Object, _mockPasswordService.Object);
        //        }

        //        #region GetAccountHolderDetailAsync Tests

        //        [Fact]
        //        public async Task GetAccountHolderDetailAsync_WhenAccountHolderExists_ReturnsAccountHolderDetail()
        //        {
        //            // Arrange
        //            var accountHolderId = Guid.NewGuid().ToString();
        //            var accountHolder = new AccountHolder
        //            {
        //                Id = accountHolderId,
        //                FirstName = "John",
        //                LastName = "Doe",
        //                NRIC = "S1234567A",
        //                DateOfBirth = new DateTime(1990, 1, 1),
        //                Email = "john.doe@email.com",
        //                ContactNumber = "91234567",
        //                SchoolingStatus = "In School",
        //                EducationLevel = "Secondary",
        //                CreatedAt = DateTime.UtcNow,
        //                EducationAccount = new EducationAccount
        //                {
        //                    Balance = 1000,
        //                    Enrollments = []
        //                }
        //            };

        //            _mockAccountHolderRepo.Setup(r => r.GetByIdAsync(accountHolderId))
        //                .ReturnsAsync(accountHolder);

        //            // Act
        //            var result = await _service.GetAccountHolderDetailAsync(accountHolderId);

        //            // Assert
        //            Assert.NotNull(result);
        //            Assert.Equal(1000, result.Balance);
        //            Assert.Equal("John Doe", result.StudentInformation.FullName);
        //            Assert.Equal("S1234567A", result.StudentInformation.NRIC);
        //            Assert.Equal("john.doe@email.com", result.StudentInformation.Email);
        //            Assert.Equal("In School", result.StudentInformation.SchoolingStatus);
        //            Assert.Equal("Secondary", result.StudentInformation.EducationLevel);
        //            Assert.True(result.StudentInformation.IsActive);
        //        }

        //        [Fact]
        //        public async Task GetAccountHolderDetailAsync_WhenAccountHolderNotFound_ThrowsNotFoundException()
        //        {
        //            // Arrange
        //            var accountHolderId = Guid.NewGuid().ToString();
        //            _mockAccountHolderRepo.Setup(r => r.GetByIdAsync(accountHolderId))
        //                .ReturnsAsync((AccountHolder?)null);

        //            // Act & Assert
        //            var exception = await Assert.ThrowsAsync<NotFoundException>(
        //                () => _service.GetAccountHolderDetailAsync(accountHolderId));

        //            Assert.Contains("Account holder with ID", exception.Message);
        //            Assert.Contains("not found", exception.Message);
        //        }

        //        [Fact]
        //        public async Task GetAccountHolderDetailAsync_WhenNoEducationAccount_ReturnsZeroBalance()
        //        {
        //            // Arrange
        //            var accountHolderId = Guid.NewGuid().ToString();
        //            var accountHolder = new AccountHolder
        //            {
        //                Id = accountHolderId,
        //                FirstName = "Jane",
        //                LastName = "Smith",
        //                NRIC = "S7654321B",
        //                DateOfBirth = new DateTime(1995, 5, 15),
        //                EducationAccount = null
        //            };

        //            _mockAccountHolderRepo.Setup(r => r.GetByIdAsync(accountHolderId))
        //                .ReturnsAsync(accountHolder);

        //            // Act
        //            var result = await _service.GetAccountHolderDetailAsync(accountHolderId);

            // Assert
        //     Assert.Equal(0, result.Balance);
        //     Assert.Equal(0, result.CourseCount);
        //     Assert.Equal(0, result.OutstandingFees);
        //     Assert.Equal(0, result.TotalFeesPaid);
        //     Assert.Empty(result.EnrolledCourses);
        //     Assert.Empty(result.OutstandingFeesDetails);
        //     Assert.Empty(result.PaymentHistory);
        // }

        //        #endregion

        //        #region GetAccountHoldersAsync Tests

        //        [Fact]
        //        public async Task GetAccountHoldersAsync_ReturnsAllAccountHolders()
        //        {
        //            // Arrange
        //            var accountHolders = new List<AccountHolder>
        //            {
        //                new()
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    FirstName = "John",
        //                    LastName = "Doe",
        //                    NRIC = "S1234567A",
        //                    DateOfBirth = new DateTime(1990, 1, 1),
        //                    SchoolingStatus = "In School",
        //                    EducationLevel = "Secondary",
        //                    EducationAccount = new EducationAccount { Balance = 500, Enrollments = [] }
        //                },
        //                new()
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    FirstName = "Jane",
        //                    LastName = "Smith",
        //                    NRIC = "S7654321B",
        //                    DateOfBirth = new DateTime(1995, 5, 15),
        //                    SchoolingStatus = "Not in School",
        //                    EducationLevel = "Tertiary",
        //                    EducationAccount = new EducationAccount { Balance = 1000, Enrollments = [] }
        //                }
        //            };

        //            var paginatedList = new PaginatedList<AccountHolder>(accountHolders, 2, 1, 20);
        //            _mockAccountHolderRepo.Setup(r => r.GetPagging(It.IsAny<IQueryable<AccountHolder>>(), 1, 20))
        //                .ReturnsAsync(paginatedList);

        //            // Act
        //            var result = await _service.GetAccountHoldersAsync(1, 20);

        //            // Assert
        //            Assert.Equal(2, result.Items.Count);
        //            Assert.Equal("John Doe", result.Items[0].FullName);
        //            Assert.Equal("Jane Smith", result.Items[1].FullName);
        //            Assert.Equal(2, result.TotalCount);
        //        }

        //        [Fact]
        //        public async Task GetAccountHoldersAsync_WhenNoAccountHolders_ReturnsEmptyList()
        //        {
        //            // Arrange
        //            var paginatedList = new PaginatedList<AccountHolder>([], 0, 1, 20);
        //            _mockAccountHolderRepo.Setup(r => r.GetPagging(It.IsAny<IQueryable<AccountHolder>>(), 1, 20))
        //                .ReturnsAsync(paginatedList);

        //            // Act
        //            var result = await _service.GetAccountHoldersAsync(1, 20);

        //            // Assert
        //            Assert.Empty(result.Items);
        //            Assert.Equal(0, result.TotalCount);
        //        }

        //        [Fact]
        //        public async Task GetAccountHoldersAsync_CalculatesAgeCorrectly()
        //        {
        //            // Arrange
        //            var birthYear = DateTime.Now.Year - 30;
        //            var accountHolders = new List<AccountHolder>
        //            {
        //                new()
        //                {
        //                    Id = Guid.NewGuid().ToString(),
        //                    FirstName = "Test",
        //                    LastName = "User",
        //                    NRIC = "S1234567A",
        //                    DateOfBirth = new DateTime(birthYear, 1, 1),
        //                    EducationAccount = null
        //                }
        //            };

        //            var paginatedList = new PaginatedList<AccountHolder>(accountHolders, 1, 1, 20);
        //            _mockAccountHolderRepo.Setup(r => r.GetPagging(It.IsAny<IQueryable<AccountHolder>>(), 1, 20))
        //                .ReturnsAsync(paginatedList);

        //            // Act
        //            var result = await _service.GetAccountHoldersAsync(1, 20);

        //            // Assert
        //            Assert.Single(result.Items);
        //            Assert.Equal(30, result.Items[0].Age);
        //        }

        //        #endregion

        //        #region AddAccountHolderAsync Tests

        //        [Fact]
        //        public async Task AddAccountHolderAsync_CreatesAccountHolderAndEducationAccount()
        //        {
        //            // Arrange
        //            var request = new CreateAccountHolderRequest
        //            {
        //                NRIC = "S1234567A",
        //                FirstName = "John",
        //                LastName = "Doe",
        //                DateOfBirth = new DateTime(1990, 1, 1),
        //                Email = "john.doe@email.com",
        //                ContactNumber = "91234567"
        //            };

        //            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
        //                .ReturnsAsync(_mockTransaction.Object);
        //            _mockPasswordService.Setup(p => p.GenerateRandomPassword(It.IsAny<int>()))
        //                .Returns("RandomPassword123");
        //            _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>()))
        //                .Returns("HashedPassword");

        //            // Act
        //            var result = await _service.AddAccountHolderAsync(request);

            // Assert
            // Assert.NotNull(result);
            // Assert.Equal("John Doe", result.FullName);
            // Assert.Equal("S1234567A", result.NRIC);
            // Assert.Equal(0, result.Balance);
            // Assert.Equal(0, result.CourseCount);
            // Assert.Equal(0, result.OutstandingFees);

        //            _mockAccountHolderRepo.Verify(r => r.InsertAsync(It.IsAny<AccountHolder>()), Times.Once);
        //            _mockEducationAccountRepo.Verify(r => r.InsertAsync(It.IsAny<EducationAccount>()), Times.Once);
        //            _mockUnitOfWork.Verify(u => u.SaveAsync(), Times.Exactly(2));
        //            _mockTransaction.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        //        }

        //        [Fact]
        //        public async Task AddAccountHolderAsync_WhenExceptionOccurs_RollsBackTransaction()
        //        {
        //            // Arrange
        //            var request = new CreateAccountHolderRequest
        //            {
        //                NRIC = "S1234567A",
        //                FirstName = "John",
        //                LastName = "Doe",
        //                DateOfBirth = new DateTime(1990, 1, 1),
        //                Email = "john.doe@email.com",
        //                ContactNumber = "91234567"
        //            };

        //            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
        //                .ReturnsAsync(_mockTransaction.Object);
        //            _mockAccountHolderRepo.Setup(r => r.InsertAsync(It.IsAny<AccountHolder>()))
        //                .ThrowsAsync(new Exception("Database error"));

        //            // Act & Assert
        //            await Assert.ThrowsAsync<Exception>(() => _service.AddAccountHolderAsync(request));

        //            _mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        //            _mockTransaction.Verify(t => t.DisposeAsync(), Times.Once);
        //        }

        //        [Fact]
        //        public async Task AddAccountHolderAsync_GeneratesPasswordCorrectly()
        //        {
        //            // Arrange
        //            var request = new CreateAccountHolderRequest
        //            {
        //                NRIC = "S1234567A",
        //                FirstName = "John",
        //                LastName = "Doe",
        //                DateOfBirth = new DateTime(1990, 1, 1),
        //                Email = "john.doe@email.com",
        //                ContactNumber = "91234567"
        //            };

        //            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
        //                .ReturnsAsync(_mockTransaction.Object);
        //            _mockPasswordService.Setup(p => p.GenerateRandomPassword(It.IsAny<int>()))
        //                .Returns("RandomPass123");
        //            _mockPasswordService.Setup(p => p.HashPassword("RandomPass123"))
        //                .Returns("HashedRandomPass");

        //            // Act
        //            await _service.AddAccountHolderAsync(request);

        //            // Assert
        //            _mockPasswordService.Verify(p => p.GenerateRandomPassword(It.IsAny<int>()), Times.Once);
        //            _mockPasswordService.Verify(p => p.HashPassword("RandomPass123"), Times.Once);
        //        }

        //        [Fact]
        //        public async Task AddAccountHolderAsync_SetsEducationAccountUsernameAsNRIC()
        //        {
        //            // Arrange
        //            var request = new CreateAccountHolderRequest
        //            {
        //                NRIC = "S1234567A",
        //                FirstName = "John",
        //                LastName = "Doe",
        //                DateOfBirth = new DateTime(1990, 1, 1),
        //                Email = "john.doe@email.com",
        //                ContactNumber = "91234567"
        //            };

        //            EducationAccount? capturedEducationAccount = null;
        //            _mockUnitOfWork.Setup(u => u.BeginTransactionAsync())
        //                .ReturnsAsync(_mockTransaction.Object);
        //            _mockPasswordService.Setup(p => p.GenerateRandomPassword(It.IsAny<int>()))
        //                .Returns("RandomPassword");
        //            _mockPasswordService.Setup(p => p.HashPassword(It.IsAny<string>()))
        //                .Returns("HashedPassword");
        //            _mockEducationAccountRepo.Setup(r => r.InsertAsync(It.IsAny<EducationAccount>()))
        //                .Callback<EducationAccount>(ea => capturedEducationAccount = ea)
        //                .Returns(Task.CompletedTask);

        //            // Act
        //            await _service.AddAccountHolderAsync(request);

        //            // Assert
        //            Assert.NotNull(capturedEducationAccount);
        //            Assert.Equal("S1234567A", capturedEducationAccount.UserName);
        //            Assert.Equal(0, capturedEducationAccount.Balance);
        //            Assert.True(capturedEducationAccount.IsActive);
        //        }

        //        #endregion
    }
}
