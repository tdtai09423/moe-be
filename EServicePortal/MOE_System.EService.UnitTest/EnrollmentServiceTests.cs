using MockQueryable.Moq; // Cần cái này để Mock IQueryable
using MOE_System.Domain.Common;
using MOE_System.Domain.Entities;
using MOE_System.EService.Application.Common; // Chứa PaginatedList
using MOE_System.EService.Application.Common.Interfaces;
using MOE_System.EService.Application.DTOs;
using MOE_System.EService.Application.Services;
using Moq;
using Xunit;
using MockQueryable;

public class EnrollmentServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IGenericRepository<Enrollment>> _mockRepo;
    private readonly EnrollmentService _service;

    public EnrollmentServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRepo = new Mock<IGenericRepository<Enrollment>>();

        // Setup: Khi Service gọi GetRepository<Enrollment> thì trả về mockRepo
        _mockUnitOfWork.Setup(u => u.GetRepository<Enrollment>())
                       .Returns(_mockRepo.Object);

        _service = new EnrollmentService(_mockUnitOfWork.Object);
    }

    // --- CASE 1: INPUT RỖNG (THROW BAD REQUEST) ---
    [Fact]
    public async Task GetActiveCourses_ShouldThrowBadRequest_WhenIdIsEmpty()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<BaseException.BadRequestException>(
            () => _service.GetActiveCoursesAsync("", 1, 10)
        );

        Assert.Equal("ID must not be empty or null!", exception.Message);
    }

    // --- CASE 2: LẤY DỮ LIỆU THÀNH CÔNG (HAPPY PATH) ---
    [Fact]
    public async Task GetActiveCourses_ShouldReturnData_WhenValid()
    {
        // 1. Arrange (Chuẩn bị dữ liệu giả)
        var userId = "user-123";

        // Tạo fake data đầy đủ quan hệ (Enrollment -> EduAccount -> AccountHolder)
        var fakeEnrollment = new Enrollment
        {
            Id = "enrol-01",
            Status = "Active", // Quan trọng: Status phải là Active
            EnrollDate = DateTime.Now,
            EducationAccount = new EducationAccount
            {
                AccountHolder = new AccountHolder { Id = userId }
            },
            Course = new Course
            {
                CourseName = "Learn C#",
                CourseCode = "CS101",
                Provider = new Provider { Name = "FPT University" }
            }
        };

        var listData = new List<Enrollment> { fakeEnrollment };

        // 2. Setup Mock cho .Entities (Để tránh lỗi IAsyncQueryProvider khi Service build query)
        var mockDbSet = listData.BuildMock();
        _mockRepo.Setup(x => x.Entities).Returns(mockDbSet);

        // 3. Setup Mock cho hàm GetPagging
        // Vì logic filter nằm ở Service, nhưng kết quả cuối cùng do GetPagging trả về.
        // Ta giả lập: "Dù mày query kiểu gì, tao cũng trả về cái listData này đóng gói trong PaginatedList"
        var pagedResult = new PaginatedList<Enrollment>(listData, 1, 1, 10);

        _mockRepo.Setup(x => x.GetPagging(
                It.IsAny<IQueryable<Enrollment>>(), // Chấp nhận mọi query truyền vào
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _service.GetActiveCoursesAsync(userId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items); // List trả về phải có 1 phần tử

        // Check Mapping DTO có đúng không
        var item = result.Items[0];
        Assert.Equal("enrol-01", item.EnrollmentId);
        Assert.Equal("Learn C#", item.CourseName);
        Assert.Equal("FPT University", item.ProviderName);
        Assert.Equal("Active", item.Status);
    }

    // --- CASE 3: KHÔNG CÓ DỮ LIỆU (EMPTY LIST) ---
    [Fact]
    public async Task GetActiveCourses_ShouldReturnEmpty_WhenNoActiveCourses()
    {
        // Arrange
        var userId = "user-123";
        var emptyList = new List<Enrollment>(); // List rỗng

        // Setup Entities
        var mockDbSet = emptyList.BuildMock();
        _mockRepo.Setup(x => x.Entities).Returns(mockDbSet);

        // Setup GetPagging trả về rỗng
        var emptyPagedResult = new PaginatedList<Enrollment>(emptyList, 0, 1, 10);

        _mockRepo.Setup(x => x.GetPagging(It.IsAny<IQueryable<Enrollment>>(), 1, 10))
                 .ReturnsAsync(emptyPagedResult);

        // Act
        var result = await _service.GetActiveCoursesAsync(userId, 1, 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items); // Phải trả về list rỗng, không được null
        Assert.Equal(0, result.TotalCount);
    }
}