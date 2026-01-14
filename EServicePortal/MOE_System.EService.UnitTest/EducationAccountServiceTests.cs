using MockQueryable;
using MockQueryable.Moq; // Để Mock IQueryable
using MOE_System.Domain.Common;
using MOE_System.Domain.Entities;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Application.Services;
using Moq;
using Xunit;

public class EducationAccountServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<EducationAccount>> _mockEduRepo;
    private readonly Mock<IGenericRepository<Enrollment>> _mockEnrollmentRepo;
    private readonly EducationAccountService _service;

    public EducationAccountServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockEduRepo = new Mock<IGenericRepository<EducationAccount>>();
        _mockEnrollmentRepo = new Mock<IGenericRepository<Enrollment>>();

        // Setup: Khi Service xin Repo nào thì trả về Mock tương ứng
        _mockUnitOfWork.Setup(u => u.GetRepository<EducationAccount>())
                       .Returns(_mockEduRepo.Object);

        _mockUnitOfWork.Setup(u => u.GetRepository<Enrollment>())
                       .Returns(_mockEnrollmentRepo.Object);

        _service = new EducationAccountService(_mockUnitOfWork.Object);
    }

    // ==========================================================
    // TEST GROUP 1: GET BALANCE (Lấy số dư)
    // ==========================================================

    [Fact]
    public async Task GetBalance_ShouldThrowBadRequest_WhenIdIsEmpty()
    {
        var exception = await Assert.ThrowsAsync<BaseException.BadRequestException>(
            () => _service.GetBalanceAsync("")
        );
        Assert.Equal("ID must not be empty or null!", exception.Message);
    }

    [Fact]
    public async Task GetBalance_ShouldThrowNotFound_WhenAccountNotExist()
    {
        // Setup Repo trả về null
        _mockEduRepo.Setup(x => x.GetByIdAsync("id-tao-lao"))
                    .ReturnsAsync((EducationAccount)null);

        var exception = await Assert.ThrowsAsync<BaseException.NotFoundException>(
            () => _service.GetBalanceAsync("id-tao-lao")
        );
        Assert.Equal("This education account is not found!", exception.Message);
    }

    [Fact]
    public async Task GetBalance_ShouldReturnCorrectData_WhenFound()
    {
        // Arrange
        var eduId = "edu-01";
        var fakeAccount = new EducationAccount
        {
            Id = eduId,
            AccountHolderId = "user-01",
            Balance = 5000,
            IsActive = true,
            UpdatedAt = DateTime.Now
        };

        _mockEduRepo.Setup(x => x.GetByIdAsync(eduId))
                    .ReturnsAsync(fakeAccount);

        // Act
        var result = await _service.GetBalanceAsync(eduId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(eduId, result.EducationAccountId);
        Assert.Equal(5000, result.Balance);
        Assert.True(result.IsActive);
    }

    // ==========================================================
    // TEST GROUP 2: GET OUTSTANDING FEE (Tính công nợ)
    // ==========================================================

    [Fact]
    public async Task GetOutstandingFee_ShouldThrowNotFound_WhenEducationAccountNotExist()
    {
        // Bước 1: Check Education Account trước
        _mockEduRepo.Setup(x => x.GetByIdAsync("id-tao-lao"))
                    .ReturnsAsync((EducationAccount)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BaseException.NotFoundException>(
            () => _service.GetOutstandingFeeAsync("id-tao-lao")
        );
        Assert.Equal("This education account is not found!", exception.Message);
    }

    [Fact]
    public async Task GetOutstandingFee_ShouldCalculateCorrectly()
    {// --- ARRANGE ---
        var eduId = "edu-01";

        // 1. Setup Education Account
        var fakeEduAccount = new EducationAccount { Id = eduId, AccountHolderId = "user-01" };
        _mockEduRepo.Setup(x => x.GetByIdAsync(eduId)).ReturnsAsync(fakeEduAccount);

        // 2. Setup Dữ liệu lồng nhau
        // Tạo biến enrollment trước để gán vào invoice bên dưới
        var enrollment1 = new Enrollment
        {
            Id = "enrol-01",
            EducationAccountId = eduId,
            Course = new Course { CourseName = "C# Advanced" }
        };

        // Gán danh sách Invoices vào Enrollment
        enrollment1.Invoices = new List<Invoice>
        {
            new Invoice
            {
                Id = "inv-01",
                Amount = 1000,
                Status = "Partial",
                EnrollmentId = "enrol-01",
                
                // --- QUAN TRỌNG NHẤT: Gán ngược lại cha ---
                Enrollment = enrollment1, // <--- FIX LỖI NULL REFERENCE Ở ĐÂY
                
                Transactions = new List<Transaction>
                {
                    new Transaction { Amount = 400, Status = "Success" }
                }
            },
            new Invoice
            {
                Id = "inv-02",
                Amount = 500,
                Status = "Pending",
                EnrollmentId = "enrol-01",
                
                // --- Gán ngược lại cha ---
                Enrollment = enrollment1, // <--- FIX LỖI NULL REFERENCE Ở ĐÂY

                Transactions = new List<Transaction>
                {
                    new Transaction { Amount = 500, Status = "Failed" }
                }
            }
        };

        // Đóng gói vào List
        var fakeEnrollments = new List<Enrollment> { enrollment1 };

        // 3. Setup MockQueryable
        var mockDbSet = fakeEnrollments.BuildMock();
        _mockEnrollmentRepo.Setup(x => x.Entities).Returns(mockDbSet);

        // --- ACT ---
        var result = await _service.GetOutstandingFeeAsync(eduId);

        // --- ASSERT ---
        Assert.NotNull(result);
        Assert.Equal(1100, result.TotalOutstandingFee);
        Assert.Equal("C# Advanced", result.OutstandingInvoices[0].CourseName);
    }
}