using LibraryManagement.Application.Services;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Moq;
using Xunit;

namespace LibraryManagement.Tests;

public class LoanServiceTests
{
    private readonly Mock<ILoanRepository> _loanRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IMemberRepository> _memberRepositoryMock;
    private readonly LoanService _service;

    public LoanServiceTests()
    {
        _loanRepositoryMock = new Mock<ILoanRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _memberRepositoryMock = new Mock<IMemberRepository>();

        _service = new LoanService(
            _loanRepositoryMock.Object,
            _bookRepositoryMock.Object,
            _memberRepositoryMock.Object);
    }

    [Fact]
    public async Task GetPagedAsync_WhenCalled_ReturnsPagedLoans()
    {
        var borrowedAt = DateTime.UtcNow.AddDays(-5);
        var dueDate = DateTime.UtcNow.AddDays(5);

        var loans = new List<Loan>
        {
            new()
            {
                Id = 1,
                BookId = 1,
                MemberId = 1,
                BorrowedAt = borrowedAt,
                DueDate = dueDate
            },
            new()
            {
                Id = 2,
                BookId = 2,
                MemberId = 2,
                BorrowedAt = borrowedAt,
                DueDate = dueDate
            }
        };

        _loanRepositoryMock
            .Setup(repository => repository.GetPagedAsync(
                1,
                2,
                "dueDate",
                "asc"))
            .ReturnsAsync((loans, 3));

        var query = new ListQueryDto
        {
            PageNumber = 1,
            PageSize = 2,
            SortBy = "dueDate",
            SortDirection = "asc"
        };

        var result = await _service.GetPagedAsync(query);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal(1, result.Items[0].BookId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenLoanExists_ReturnsLoan()
    {
        var loan = new Loan
        {
            Id = 1,
            BookId = 2,
            MemberId = 3,
            BorrowedAt = DateTime.UtcNow.AddDays(-3),
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _loanRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(loan);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(2, result.BookId);
        Assert.Equal(3, result.MemberId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenLoanDoesNotExist_ReturnsNull()
    {
        _loanRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Loan?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenDataIsValid_CreatesLoan()
    {
        var dueDate = DateTime.UtcNow.AddDays(14);

        var dto = new LoanCreateDto
        {
            BookId = 1,
            MemberId = 1,
            DueDate = dueDate
        };

        var book = CreateBook();
        var member = CreateActiveMember();

        _bookRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(book);

        _memberRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(member);

        var createdLoan = new Loan
        {
            Id = 5,
            BookId = 1,
            MemberId = 1,
            BorrowedAt = DateTime.UtcNow,
            DueDate = dueDate,
            ReturnedAt = null
        };

        _loanRepositoryMock
            .Setup(repository => repository.CreateAsync(
                It.Is<Loan>(loan =>
                    loan.BookId == 1 &&
                    loan.MemberId == 1 &&
                    loan.DueDate == dueDate &&
                    loan.ReturnedAt == null)))
            .ReturnsAsync(createdLoan);

        var result = await _service.CreateAsync(dto);

        Assert.Equal(5, result.Id);
        Assert.Equal(1, result.BookId);
        Assert.Equal(1, result.MemberId);
        Assert.Equal(dueDate, result.DueDate);
        Assert.Null(result.ReturnedAt);

        _loanRepositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Loan>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenDueDateIsInvalid_ThrowsException()
    {
        var dto = new LoanCreateDto
        {
            BookId = 1,
            MemberId = 1,
            DueDate = DateTime.UtcNow.AddDays(-1)
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));

        _loanRepositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Loan>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenBookDoesNotExist_ThrowsException()
    {
        var dto = new LoanCreateDto
        {
            BookId = 99,
            MemberId = 1,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _bookRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Book?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CreateAsync(dto));

        _loanRepositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Loan>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenMemberDoesNotExist_ThrowsException()
    {
        var dto = new LoanCreateDto
        {
            BookId = 1,
            MemberId = 99,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        _bookRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(CreateBook());

        _memberRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Member?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.CreateAsync(dto));

        _loanRepositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Loan>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenMemberIsInactive_ThrowsException()
    {
        var dto = new LoanCreateDto
        {
            BookId = 1,
            MemberId = 1,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var inactiveMember = CreateActiveMember();
        inactiveMember.IsActive = false;

        _bookRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(CreateBook());

        _memberRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(inactiveMember);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto));

        _loanRepositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Loan>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenDataIsValid_UpdatesLoan()
    {
        var borrowedAt = DateTime.UtcNow.AddDays(-5);
        var dueDate = DateTime.UtcNow.AddDays(5);

        var loan = new Loan
        {
            Id = 1,
            BookId = 2,
            MemberId = 2,
            BorrowedAt = DateTime.UtcNow.AddDays(-10),
            DueDate = DateTime.UtcNow.AddDays(-2)
        };

        var dto = new LoanUpdateDto
        {
            BookId = 1,
            MemberId = 1,
            BorrowedAt = borrowedAt,
            DueDate = dueDate,
            ReturnedAt = null
        };

        _loanRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(loan);

        _bookRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(CreateBook());

        _memberRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(CreateActiveMember());

        _loanRepositoryMock
            .Setup(repository => repository.UpdateAsync(loan))
            .Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(1, dto);

        Assert.True(result);
        Assert.Equal(1, loan.BookId);
        Assert.Equal(1, loan.MemberId);
        Assert.Equal(borrowedAt, loan.BorrowedAt);
        Assert.Equal(dueDate, loan.DueDate);

        _loanRepositoryMock.Verify(
            repository => repository.UpdateAsync(loan),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenLoanDoesNotExist_ReturnsFalse()
    {
        _loanRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Loan?)null);

        var dto = new LoanUpdateDto
        {
            BookId = 1,
            MemberId = 1,
            BorrowedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var result = await _service.UpdateAsync(99, dto);

        Assert.False(result);

        _loanRepositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Loan>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenReturnedDateIsInvalid_ThrowsException()
    {
        var borrowedAt = DateTime.UtcNow.AddDays(-5);

        var loan = new Loan
        {
            Id = 1,
            BookId = 1,
            MemberId = 1,
            BorrowedAt = borrowedAt,
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        var dto = new LoanUpdateDto
        {
            BookId = 1,
            MemberId = 1,
            BorrowedAt = borrowedAt,
            DueDate = DateTime.UtcNow.AddDays(5),
            ReturnedAt = borrowedAt.AddDays(-1)
        };

        _loanRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(loan);

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UpdateAsync(1, dto));

        _loanRepositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Loan>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenLoanExists_DeletesLoan()
    {
        var loan = new Loan
        {
            Id = 1,
            BookId = 1,
            MemberId = 1,
            BorrowedAt = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(5)
        };

        _loanRepositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(loan);

        _loanRepositoryMock
            .Setup(repository => repository.DeleteAsync(loan))
            .Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);

        _loanRepositoryMock.Verify(
            repository => repository.DeleteAsync(loan),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenLoanDoesNotExist_ReturnsFalse()
    {
        _loanRepositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Loan?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);

        _loanRepositoryMock.Verify(
            repository => repository.DeleteAsync(
                It.IsAny<Loan>()),
            Times.Never);
    }

    private static Book CreateBook()
    {
        return new Book
        {
            Id = 1,
            Title = "Ali ve Nino",
            BookCode = "AZ-001",
            PublishedYear = 1937,
            TotalCopies = 5,
            AvailableCopies = 3
        };
    }

    private static Member CreateActiveMember()
    {
        return new Member
        {
            Id = 1,
            FirstName = "Aysel",
            LastName = "Memmedova",
            Email = "aysel.memmedova@example.com",
            PhoneNumber = "+994501112233",
            MembershipDate = DateTime.UtcNow,
            IsActive = true
        };
    }
}