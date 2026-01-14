using Xunit;
using Moq;
using MOE_System.EService.Domain.Entities;
using MOE_System.EService.Application.Services;
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Domain.Common;
using MockQueryable; // <--- QUAN TRỌNG: Phải có dòng này

public class AccountHolderServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<AccountHolder>> _mockRepo; 
    private readonly AccountHolderService _service;

    public AccountHolderServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IGenericRepository<AccountHolder>>();

        // Setup: Khi Service xin Repo, đưa cho nó cái Repo giả của mình
        _mockUnitOfWork.Setup(u => u.GetRepository<AccountHolder>())
                       .Returns(_mockRepo.Object);

        _service = new AccountHolderService(_mockUnitOfWork.Object);
    }

    // --- HÀM 1: TEST LỖI INPUT RỖNG ---
    [Fact]
    public async Task GetAccountHolder_ShouldThrowBadRequest_WhenIdIsEmpty()
    {
        var exception = await Assert.ThrowsAsync<BaseException.BadRequestException>(
            () => _service.GetAccountHolderAsync("")
        );
        Assert.Equal("ID must not be empty or null!", exception.Message);
    }

    // --- HÀM 2: TEST KHÔNG TÌM THẤY ---
    [Fact]
    public async Task GetAccountHolder_ShouldThrowNotFound_WhenIdDoesNotExist()
    {// Arrange
     // 1. Tạo một List rỗng (đại diện cho DB không có dữ liệu khớp)
        var emptyList = new List<AccountHolder>();

        // 2. "Phù phép" nó thành Async bằng BuildMock() <-- QUAN TRỌNG
        var mockDbSet = emptyList.BuildMock();

        // 3. Setup Mock
        // (LƯU Ý: Chỗ .Entities này bạn dùng tên gì ở hàm Success thì ở đây dùng y chang vậy nhé)
        _mockRepo.Setup(x => x.Entities).Returns(mockDbSet);

        // Act & Assert
        // Lúc này Service chạy query vào list rỗng -> Trả về null -> Ném Exception -> Test Xanh
        var exception = await Assert.ThrowsAsync<BaseException.NotFoundException>(
            () => _service.GetAccountHolderAsync("id-tao-lao")
        );

        Assert.Equal("This account is not found!", exception.Message);
    }

    // --- HÀM 3: TEST LẤY DỮ LIỆU THÀNH CÔNG (Fix lỗi NullReference ở đây) ---
    [Fact]
    public async Task GetAccountHolder_ShouldReturnData_WhenIdExists()
    {
        // Arrange
        var accountId = "user-001";

        // Tạo cục data giả đầy đủ (bao gồm cả bảng con)
        var fakeList = new List<AccountHolder>
        {
            new AccountHolder
            {
                Id = accountId,
                FirstName = "Nguyen",
                LastName = "Van A",
                // Quan trọng: Tạo sẵn dữ liệu bảng con để tránh lỗi NullReference
                EducationAccount = new EducationAccount
                {
                    Id = "edu-01",
                    Balance = 100,
                    UserName = "test",
                    Password = "123"
                }
            }
        }
        ;
        // --- KHẮC PHỤC LỖI IAsyncQueryProvider TẠI ĐÂY ---

        // 1. Biến List thường thành "List Async giả" bằng .BuildMock()
        var mockDbSet = fakeList.BuildMock();

        // 2. Setup: Nếu Service gọi hàm trả về IQueryable (ví dụ GetQueryable hoặc GetAll)
        // Lưu ý: Nếu Service của bạn gọi .GetQueryable() thì Setup cái này:
        _mockRepo.Setup(x => x.Entities).Returns(mockDbSet);

        // Hoặc nếu Service gọi .Where(...).FirstOrDefaultAsync() trực tiếp trên Repo:
        // (Tùy thuộc vào interface IGenericRepository của bạn viết thế nào)

        // *** TRƯỜNG HỢP SERVICE DÙNG GetByIdAsync (Task) ***
        // Nếu Service chỉ gọi: await _repo.GetByIdAsync(id) -> Thì dùng cái này (KHÔNG CẦN BuildMock):
        // _mockRepo.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync(fakeList.First());

        // Act
        var result = await _service.GetAccountHolderAsync(accountId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(100, result.EducationAccountBalance);
    }
}