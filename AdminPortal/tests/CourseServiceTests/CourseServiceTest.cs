using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MOE_System.Application.Common;
using MOE_System.Application.Common.Course;
using MOE_System.Application.Common.Interfaces;
using MOE_System.Application.DTOs.Course.Request;
using MOE_System.Application.DTOs.Course.Response;
using MOE_System.Application.Services;
using MOE_System.Domain.Entities;
using Xunit;

namespace MOE_System.Application.Tests.CourseServiceTests;

public class CourseServiceTest
{
    #region Setup and Mocks
    
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

    private void SetupMockRepository(List<Course> courses, int pageNumber = 1, int pageSize = 10)
    {
        var queryable = courses.AsQueryable();
        _courseRepositoryMock.Setup(r => r.Entities).Returns(queryable);
        
        // Mock GetPagging method
        var totalCount = courses.Count;
        var pagedItems = courses
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        var paginatedResult = new PaginatedList<Course>(pagedItems, totalCount, pageNumber, pageSize);
        
        _courseRepositoryMock.Setup(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), pageNumber, pageSize))
            .ReturnsAsync(paginatedResult);
    }
    
    #endregion

    #region Basic Tests

    [Fact]
    public async Task GetCoursesAsync_WithNoFilters_ReturnsAllCourses()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Computer Science 101", 
                Provider = new Provider { Name = "Tech Academy" }, 
                StartDate = now, 
                EndDate = now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 500m, 
                CreatedAt = now, 
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "MATH101", 
                CourseName = "Mathematics 101", 
                Provider = new Provider { Name = "Math Academy" }, 
                StartDate = now, 
                EndDate = now.AddMonths(3), 
                PaymentType = "One-time", 
                FeeAmount = 1000m, 
                CreatedAt = now.AddDays(-1), 
                Enrollments = new List<Enrollment>() 
            }
        };

        SetupMockRepository(courses);
        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(2);
        result.Items[0].CourseCode.Should().Be("CS101");
        result.Items[0].CourseName.Should().Be("Computer Science 101");
        result.Items[0].ProviderName.Should().Be("Tech Academy");
        result.Items[0].TotalFee.Should().Be(500m);
        result.Items[0].EnrolledCount.Should().Be(0);
    }

    [Fact]
    public async Task GetCoursesAsync_WithEmptyList_ReturnsEmptyResult()
    {
        // Arrange
        SetupMockRepository(new List<Course>());
        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }
    
    #endregion

    #region Filter Tests

    [Fact]
    public async Task GetCoursesAsync_WithSearchTerm_FiltersByCourseName()
    {
        // Arrange
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Python Programming", 
                Provider = new Provider { Name = "Tech Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 500m, 
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "MATH101", 
                CourseName = "Advanced Mathematics", 
                Provider = new Provider { Name = "Math Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "One-time", 
                FeeAmount = 1000m, 
                Enrollments = new List<Enrollment>() 
            }
        };

        // Mock the filtering in repository
        var filteredCourses = courses.Where(c => c.CourseName.Contains("Python")).ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            SearchTerm = "Python" 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Contain("Python");
    }

    [Fact]
    public async Task GetCoursesAsync_WithProviderFilter_ReturnsFilteredResults()
    {
        // Arrange
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Course 1", 
                Provider = new Provider { Name = "Tech Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 500m, 
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Course 2", 
                Provider = new Provider { Name = "Math Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 600m, 
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.Provider?.Name == "Tech Academy").ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            Provider = "Tech Academy" 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].ProviderName.Should().Be("Tech Academy");
    }

    [Fact]
    public async Task GetCoursesAsync_WithPaymentTypeFilter_ReturnsFilteredResults()
    {
        // Arrange
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Course 1", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Recurring", 
                FeeAmount = 500m, 
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Course 2", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "One-time", 
                FeeAmount = 600m, 
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.PaymentType == "Recurring").ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            PaymentType = "Recurring" 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].PaymentType.Should().Be("Recurring");
    }

    [Fact]
    public async Task GetCoursesAsync_WithBillingCycleFilter_ReturnsFilteredResults()
    {
        // Arrange
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Course 1", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Recurring", 
                FeeAmount = 500m,
                BillingCycle = "Monthly",
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Course 2", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Recurring", 
                FeeAmount = 600m,
                BillingCycle = "Quarterly",
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.BillingCycle == "Quarterly").ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            BillingCycle = "Quarterly" 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Course 2");
    }

    #region Start Date Filtering Tests
    
    [Fact]
    public async Task GetCoursesAsync_WithStartDateFromFilter_ReturnsFilteredResults()
    {
        // Arrange
        var baseDate = new DateTime(2026, 3, 1);
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Early Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 2, 15),
                EndDate = new DateTime(2026, 5, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Late Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 3, 15),
                EndDate = new DateTime(2026, 6, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 600m,
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.StartDate >= baseDate).ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            StartDateFrom = baseDate 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Late Course");
        result.Items[0].StartDate.Should().BeOnOrAfter(baseDate);
    }

    [Fact]
    public async Task GetCoursesAsync_WithStartDateToFilter_ReturnsFilteredResults()
    {
        // Arrange
        var baseDate = new DateTime(2026, 3, 1);
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Early Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 2, 15),
                EndDate = new DateTime(2026, 5, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Late Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 3, 15),
                EndDate = new DateTime(2026, 6, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 600m,
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.StartDate <= baseDate).ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            StartDateTo = baseDate 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Early Course");
        result.Items[0].StartDate.Should().BeOnOrBefore(baseDate);
    }

    [Fact]
    public async Task GetCoursesAsync_WithStartDateRangeFilter_ReturnsFilteredResults()
    {
        // Arrange
        var startFrom = new DateTime(2026, 2, 20);
        var startTo = new DateTime(2026, 3, 10);
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Early Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 2, 15),
                EndDate = new DateTime(2026, 5, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Mid Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 6, 1), 
                PaymentType = "Recurring", 
                FeeAmount = 600m,
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS103", 
                CourseName = "Late Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 3, 15),
                EndDate = new DateTime(2026, 6, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 700m,
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.StartDate >= startFrom && c.StartDate <= startTo).ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            StartDateFrom = startFrom,
            StartDateTo = startTo 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Mid Course");
        result.Items[0].StartDate.Should().BeOnOrAfter(startFrom);
        result.Items[0].StartDate.Should().BeOnOrBefore(startTo);
    }
    
    #endregion

    #region End Date Filtering Tests
    
    [Fact]
    public async Task GetCoursesAsync_WithEndDateFromFilter_ReturnsFilteredResults()
    {
        // Arrange
        var baseDate = new DateTime(2026, 6, 1);
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Short Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 2, 15),
                EndDate = new DateTime(2026, 5, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Long Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 3, 15),
                EndDate = new DateTime(2026, 6, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 600m,
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.EndDate >= baseDate).ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            EndDateFrom = baseDate 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Long Course");
        result.Items[0].EndDate.Should().BeOnOrAfter(baseDate);
    }

    [Fact]
    public async Task GetCoursesAsync_WithEndDateToFilter_ReturnsFilteredResults()
    {
        // Arrange
        var baseDate = new DateTime(2026, 6, 1);
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Short Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 2, 15),
                EndDate = new DateTime(2026, 5, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Long Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 3, 15),
                EndDate = new DateTime(2026, 6, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 600m,
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.EndDate <= baseDate).ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            EndDateTo = baseDate 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Short Course");
        result.Items[0].EndDate.Should().BeOnOrBefore(baseDate);
    }

    [Fact]
    public async Task GetCoursesAsync_WithEndDateRangeFilter_ReturnsFilteredResults()
    {
        // Arrange
        var endFrom = new DateTime(2026, 5, 20);
        var endTo = new DateTime(2026, 6, 10);
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Early End Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 2, 15),
                EndDate = new DateTime(2026, 5, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Mid End Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 6, 1), 
                PaymentType = "Recurring", 
                FeeAmount = 600m,
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS103", 
                CourseName = "Late End Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = new DateTime(2026, 3, 15),
                EndDate = new DateTime(2026, 6, 15), 
                PaymentType = "Recurring", 
                FeeAmount = 700m,
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.EndDate >= endFrom && c.EndDate <= endTo).ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            EndDateFrom = endFrom,
            EndDateTo = endTo 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Mid End Course");
        result.Items[0].EndDate.Should().BeOnOrAfter(endFrom);
        result.Items[0].EndDate.Should().BeOnOrBefore(endTo);
    }
    
    #endregion

    [Fact]
    public async Task GetCoursesAsync_WithFeeRangeFilter_ReturnsCorrectResults()
    {
        // Arrange
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Cheap Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 500m, 
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Mid Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 1500m, 
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS103", 
                CourseName = "Expensive Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 2500m, 
                Enrollments = new List<Enrollment>() 
            }
        };

        var filteredCourses = courses.Where(c => c.FeeAmount >= 1000m && c.FeeAmount <= 2000m).ToList();
        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            TotalFeeMin = 1000m,
            TotalFeeMax = 2000m
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].TotalFee.Should().Be(1500m);
        result.Items[0].CourseName.Should().Be("Mid Course");
    }
    
    #endregion

    #region Sorting Tests

    [Fact]
    public async Task GetCoursesAsync_WithSortByName_ReturnsSortedResults()
    {
        // Arrange
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS103", 
                CourseName = "Zebra Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 500m, 
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Alpha Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 600m, 
                Enrollments = new List<Enrollment>() 
            }
        };

        var sortedCourses = courses.OrderBy(c => c.CourseName).ToList();
        SetupMockRepository(sortedCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10, 
            SortBy = CourseSortField.CourseName,
            SortDirection = SortDirection.Asc
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(2);
        result.Items[0].CourseName.Should().Be("Alpha Course");
        result.Items[1].CourseName.Should().Be("Zebra Course");
    }
    
    #endregion

    #region Pagination Tests

    [Fact]
    public async Task GetCoursesAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var courses = new List<Course>();
        for (int i = 1; i <= 25; i++)
        {
            courses.Add(new Course 
            { 
                CourseCode = $"CS{i:D3}", 
                CourseName = $"Course {i}",
                Provider = new Provider { Name = "Academy" },
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3),
                PaymentType = "Monthly",
                FeeAmount = 500m,
                Enrollments = new List<Enrollment>()
            });
        }

        SetupMockRepository(courses, 2, 10);

        var request = new GetCourseRequest 
        { 
            PageNumber = 2, 
            PageSize = 10 
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(10);
        result.TotalCount.Should().Be(25);
        result.PageIndex.Should().Be(2);
        result.Items[0].CourseName.Should().Be("Course 11");
    }
    
    #endregion

    #region Multiple Filters Tests

    [Fact]
    public async Task GetCoursesAsync_WithMultipleFilters_AppliesAllFilters()
    {
        // Arrange
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Python Programming", 
                Provider = new Provider { Name = "Tech Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 1500m, 
                Enrollments = new List<Enrollment>() 
            },
            new() 
            { 
                CourseCode = "CS102", 
                CourseName = "Java Programming", 
                Provider = new Provider { Name = "Code School" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "One-time", 
                FeeAmount = 2500m, 
                Enrollments = new List<Enrollment>() 
            }
        };

        // Apply multiple filters
        var filteredCourses = courses
            .Where(c => c.CourseName.Contains("Python"))
            .Where(c => c.Provider?.Name == "Tech Academy")
            .Where(c => c.PaymentType == "Monthly")
            .Where(c => c.FeeAmount >= 1000m && c.FeeAmount <= 2000m)
            .ToList();

        SetupMockRepository(filteredCourses);

        var request = new GetCourseRequest 
        { 
            PageNumber = 1, 
            PageSize = 10,
            SearchTerm = "Python",
            Provider = "Tech Academy",
            PaymentType = "Monthly",
            TotalFeeMin = 1000m,
            TotalFeeMax = 2000m
        };

        // Act
        var result = await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        result.Items.Count.Should().Be(1);
        result.Items[0].CourseName.Should().Be("Python Programming");
        result.Items[0].ProviderName.Should().Be("Tech Academy");
        result.Items[0].PaymentType.Should().Be("Monthly");
        result.Items[0].TotalFee.Should().Be(1500m);
    }

    #endregion

    #region Mock Verification Tests

    [Fact]
    public async Task GetCoursesAsync_VerifyRepositoryMethodsCalled()
    {
        // Arrange
        var courses = new List<Course>
        {
            new() 
            { 
                CourseCode = "CS101", 
                CourseName = "Test Course", 
                Provider = new Provider { Name = "Academy" }, 
                StartDate = DateTime.Now, 
                EndDate = DateTime.Now.AddMonths(3), 
                PaymentType = "Monthly", 
                FeeAmount = 500m, 
                Enrollments = new List<Enrollment>() 
            }
        };

        SetupMockRepository(courses);
        var request = new GetCourseRequest { PageNumber = 1, PageSize = 10 };

        // Act
        await _courseService.GetCoursesAsync(request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.GetRepository<Course>(), Times.Once);
        _courseRepositoryMock.Verify(r => r.Entities, Times.Once);
        _courseRepositoryMock.Verify(r => r.GetPagging(It.IsAny<IQueryable<Course>>(), 1, 10), Times.Once);
    }
    
    #endregion
}