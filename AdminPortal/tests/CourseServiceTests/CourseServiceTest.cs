using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;
using MOE_System.Application.Services;
using MOE_System.Domain.Entities;
using Xunit;

namespace MOE_System.Application.Tests.CourseServiceTests;

public class CourseServiceTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IGenericRepository<Course>> _courseRepositoryMock;
    private readonly CourseService _courseService;

    public CourseServiceTest()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _courseRepositoryMock = new Mock<IGenericRepository<Course>>();
        
        _unitOfWorkMock.Setup(u => u.GetRepository<Course>())
            .Returns(_courseRepositoryMock.Object);
        
        _courseService = new CourseService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetCoursesAsync_WithNoFilters_ReturnsAllCourses()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockCourses = new List<Course>
        {
            new()
            {
                CourseCode = "MATH101",
                CourseName = "Mathematics 101",
                Provider = new Provider { Name = "Tech Academy" },
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment> { new(), new() }
            },
            new()
            {
                CourseCode = "ENG101",
                CourseName = "English 101",
                Provider = new Provider { Name = "Language Center" },
                StartDate = now.AddMonths(-2),
                EndDate = now.AddMonths(2),
                PaymentType = "One-time",
                FeeAmount = 300m,
                Enrollments = new List<Enrollment> { new() }
            }
        };

        var paginatedCourses = new PaginatedList<Course>(mockCourses, 2, 1, 1);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(mockCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10, SearchTerm = null };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(2);
        result.Items[0].CourseName.Should().Be("Mathematics 101");
        result.Items[1].CourseName.Should().Be("English 101");
    }

    [Fact]
    public async Task GetCoursesAsync_WithSearchTerm_FiltersCoursesByName()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var allCourses = new List<Course>
        {
            new()
            {
                CourseCode = "MATH101",
                CourseName = "Mathematics 101",
                Provider = new Provider { Name = "Tech Academy" },
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment> { new() }
            },
            new()
            {
                CourseCode = "ENG101",
                CourseName = "English 101",
                Provider = new Provider { Name = "Language Center" },
                StartDate = now.AddMonths(-2),
                EndDate = now.AddMonths(2),
                PaymentType = "One-time",
                FeeAmount = 300m,
                Enrollments = new List<Enrollment>()
            }
        };

        var filteredCourses = allCourses.Where(c => c.CourseName.ToLower().Contains("math")).ToList();
        var paginatedCourses = new PaginatedList<Course>(filteredCourses, 1, 1, 1);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(allCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10, SearchTerm = "math" };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Mathematics 101");
    }

    [Fact]
    public async Task GetCoursesAsync_WithCaseInsensitiveSearch_FindsCourse()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var allCourses = new List<Course>
        {
            new()
            {
                CourseCode = "MATH101",
                CourseName = "Mathematics 101",
                Provider = new Provider { Name = "Tech Academy" },
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>()
            }
        };

        var filteredCourses = allCourses.Where(c => c.CourseName.ToLower().Contains("MATHEMATICS".ToLower())).ToList();
        var paginatedCourses = new PaginatedList<Course>(filteredCourses, 1, 1, 1);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(allCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10, SearchTerm = "MATHEMATICS" };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Contain("Mathematics");
    }

    [Fact]
    public async Task GetCoursesAsync_OrdersByCreatedAtDescending()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockCourses = new List<Course>
        {
            new()
            {
                CourseCode = "COURSE1",
                CourseName = "First Course",
                Provider = new Provider { Name = "Academy" },
                CreatedAt = now,
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>()
            },
            new()
            {
                CourseCode = "COURSE2",
                CourseName = "Second Course",
                Provider = new Provider { Name = "Academy" },
                CreatedAt = now.AddDays(-5),
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>()
            }
        }.OrderByDescending(c => c.CreatedAt).ToList();

        var paginatedCourses = new PaginatedList<Course>(mockCourses, 2, 1, 1);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(mockCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items[0].CourseName.Should().Be("First Course");
        result.Items[1].CourseName.Should().Be("Second Course");
    }

    [Fact]
    public async Task GetCoursesAsync_ReturnsPaginatedResults()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockCourses = new List<Course>
        {
            new()
            {
                CourseCode = "MATH101",
                CourseName = "Mathematics 101",
                Provider = new Provider { Name = "Tech Academy" },
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>()
            }
        };

        var paginatedCourses = new PaginatedList<Course>(mockCourses, 50, 1, 5);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(mockCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(50);
        result.PageIndex.Should().Be(1);
        result.TotalPages.Should().Be(5);
    }

    [Fact]
    public async Task GetCoursesAsync_MapsEnrollmentsCount()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockCourses = new List<Course>
        {
            new()
            {
                CourseCode = "MATH101",
                CourseName = "Mathematics 101",
                Provider = new Provider { Name = "Tech Academy" },
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment> { new(), new(), new() }
            }
        };

        var paginatedCourses = new PaginatedList<Course>(mockCourses, 1, 1, 1);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(mockCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items[0].EnrolledCount.Should().Be(3);
    }

    [Fact]
    public async Task GetCoursesAsync_MapsAllCourseProperties()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var startDate = now.AddMonths(-1);
        var endDate = now.AddMonths(3);
        
        var mockCourses = new List<Course>
        {
            new()
            {
                CourseCode = "MATH101",
                CourseName = "Mathematics 101",
                Provider = new Provider { Name = "Tech Academy" },
                StartDate = startDate,
                EndDate = endDate,
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>()
            }
        };

        var paginatedCourses = new PaginatedList<Course>(mockCourses, 1, 1, 1);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(mockCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var course = result.Items[0];
        course.CourseCode.Should().Be("MATH101");
        course.CourseName.Should().Be("Mathematics 101");
        course.ProviderName.Should().Be("Tech Academy");
        course.StartDate.Should().Be(startDate);
        course.EndDate.Should().Be(endDate);
        course.PaymentType.Should().Be("Monthly");
        course.TotalFee.Should().Be(500m);
    }

    [Fact]
    public async Task GetCoursesAsync_WithEmptySearchResult_ReturnsEmptyList()
    {
        // Arrange
        var mockCourses = new List<Course>
        {
            new()
            {
                CourseCode = "MATH101",
                CourseName = "Mathematics 101",
                Provider = new Provider { Name = "Tech Academy" },
                StartDate = DateTime.UtcNow.AddMonths(-1),
                EndDate = DateTime.UtcNow.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>()
            }
        };

        var emptyResult = new List<Course>();
        var paginatedCourses = new PaginatedList<Course>(emptyResult, 0, 1, 0);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(mockCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10, SearchTerm = "nonexistent" };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(0);
        result.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetCoursesAsync_WithWhitespaceSearchTerm_TrimsAndSearches()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockCourses = new List<Course>
        {
            new()
            {
                CourseCode = "MATH101",
                CourseName = "Mathematics 101",
                Provider = new Provider { Name = "Tech Academy" },
                StartDate = now.AddMonths(-1),
                EndDate = now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>()
            }
        };

        var filteredCourses = mockCourses.Where(c => c.CourseName.ToLower().Contains("math")).ToList();
        var paginatedCourses = new PaginatedList<Course>(filteredCourses, 1, 1, 1);

        _courseRepositoryMock.Setup(r => r.Entities)
            .Returns(mockCourses.AsQueryable());

        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10))
            .ReturnsAsync(paginatedCourses);

        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10, SearchTerm = "  math  " };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(1);
    }
}
