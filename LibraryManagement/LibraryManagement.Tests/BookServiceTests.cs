using LibraryManagement.Application.Services;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Moq;
using Xunit;

namespace LibraryManagement.Tests;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _repositoryMock;
    private readonly BookService _service;

    public BookServiceTests()
    {
        _repositoryMock = new Mock<IBookRepository>();
        _service = new BookService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetPagedAsync_WhenCalled_ReturnsPagedBooks()
    {
        var books = new List<Book>
        {
            new()
            {
                Id = 1,
                Title = "Kitab 1",
                BookCode = "BK-001",
                PublishedYear = 1937,
                TotalCopies = 5,
                AvailableCopies = 3
            },
            new()
            {
                Id = 2,
                Title = "Clean CODE",
                BookCode = "BK-002",
                PublishedYear = 1967,
                TotalCopies = 4,
                AvailableCopies = 2
            }
        };

        _repositoryMock
            .Setup(repository => repository.GetPagedAsync(
                1,
                2,
                "title",
                "asc"))
            .ReturnsAsync((books, 3));

        var query = new ListQueryDto
        {
            PageNumber = 1,
            PageSize = 2,
            SortBy = "title",
            SortDirection = "asc"
        };

        var result = await _service.GetPagedAsync(query);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal("Kitab 1", result.Items[0].Title);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBookExists_ReturnsBook()
    {
        var book = CreateBook();

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(book);

        var result = await _service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Kitab 1", result.Title);
        Assert.Equal("BK-001", result.BookCode);
    }

    [Fact]
    public async Task GetByIdAsync_WhenBookDoesNotExist_ReturnsNull()
    {
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Book?)null);

        var result = await _service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenDataIsValid_CreatesBook()
    {
        var dto = new BookCreateDto
        {
            Title = "BOOK",
            BookCode = "BK-003",
            PublishedYear = 1300,
            TotalCopies = 6,
            AvailableCopies = 6
        };

        _repositoryMock
            .Setup(repository => repository.BookCodeExistsAsync(
                "BK-003",
                null))
            .ReturnsAsync(false);

        var createdBook = new Book
        {
            Id = 3,
            Title = dto.Title,
            BookCode = dto.BookCode,
            PublishedYear = dto.PublishedYear,
            TotalCopies = dto.TotalCopies,
            AvailableCopies = dto.AvailableCopies
        };

        _repositoryMock
            .Setup(repository => repository.CreateAsync(
                It.Is<Book>(book =>
                    book.Title == dto.Title &&
                    book.BookCode == dto.BookCode &&
                    book.TotalCopies == dto.TotalCopies &&
                    book.AvailableCopies ==
                        dto.AvailableCopies)))
            .ReturnsAsync(createdBook);

        var result = await _service.CreateAsync(dto);

        Assert.Equal(3, result.Id);
        Assert.Equal("BOOK", result.Title);
        Assert.Equal("BK-003", result.BookCode);

        _repositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Book>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenAvailableCopiesExceedTotal_ThrowsException()
    {
        var dto = new BookCreateDto
        {
            Title = "Sehv Kitab",
            BookCode = "BK-004",
            PublishedYear = 2020,
            TotalCopies = 2,
            AvailableCopies = 5
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateAsync(dto));

        _repositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Book>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenBookCodeExists_ThrowsException()
    {
        var dto = new BookCreateDto
        {
            Title = "Yeni Kitab",
            BookCode = "BK-001",
            PublishedYear = 2024,
            TotalCopies = 5,
            AvailableCopies = 5
        };

        _repositoryMock
            .Setup(repository => repository.BookCodeExistsAsync(
                "BK-001",
                null))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto));

        _repositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Book>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenDataIsValid_UpdatesBook()
    {
        var book = CreateBook();

        var dto = new BookUpdateDto
        {
            Title = "Kitab 1 Yenilenmis",
            BookCode = "BK-001-NEW",
            PublishedYear = 1937,
            TotalCopies = 10,
            AvailableCopies = 7
        };

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(book);

        _repositoryMock
            .Setup(repository => repository.BookCodeExistsAsync(
                "BK-001-NEW",
                1))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(repository => repository.UpdateAsync(book))
            .Returns(Task.CompletedTask);

        var result = await _service.UpdateAsync(1, dto);

        Assert.True(result);
        Assert.Equal("Kitab 1 Yenilenmis", book.Title);
        Assert.Equal("BK-001-NEW", book.BookCode);
        Assert.Equal(10, book.TotalCopies);
        Assert.Equal(7, book.AvailableCopies);

        _repositoryMock.Verify(
            repository => repository.UpdateAsync(book),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenBookDoesNotExist_ReturnsFalse()
    {
        var dto = new BookUpdateDto
        {
            Title = "Yeni Kitab",
            BookCode = "BK-005",
            PublishedYear = 2025,
            TotalCopies = 5,
            AvailableCopies = 3
        };

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Book?)null);

        var result = await _service.UpdateAsync(99, dto);

        Assert.False(result);

        _repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Book>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenAvailableCopiesExceedTotal_ThrowsException()
    {
        var dto = new BookUpdateDto
        {
            Title = "Sehv Kitab",
            BookCode = "BK-006",
            PublishedYear = 2025,
            TotalCopies = 2,
            AvailableCopies = 6
        };

        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.UpdateAsync(1, dto));

        _repositoryMock.Verify(
            repository => repository.GetByIdAsync(
                It.IsAny<int>()),
            Times.Never);

        _repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Book>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenBookCodeExists_ThrowsException()
    {
        var book = CreateBook();

        var dto = new BookUpdateDto
        {
            Title = "Diger Kitab",
            BookCode = "BK-002",
            PublishedYear = 2025,
            TotalCopies = 5,
            AvailableCopies = 3
        };

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(book);

        _repositoryMock
            .Setup(repository => repository.BookCodeExistsAsync(
                "BK-002",
                1))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.UpdateAsync(1, dto));

        _repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Book>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenBookExists_DeletesBook()
    {
        var book = CreateBook();

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(book);

        _repositoryMock
            .Setup(repository => repository.DeleteAsync(book))
            .Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);

        _repositoryMock.Verify(
            repository => repository.DeleteAsync(book),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenBookDoesNotExist_ReturnsFalse()
    {
        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Book?)null);

        var result = await _service.DeleteAsync(99);

        Assert.False(result);

        _repositoryMock.Verify(
            repository => repository.DeleteAsync(
                It.IsAny<Book>()),
            Times.Never);
    }

    private static Book CreateBook()
    {
        return new Book
        {
            Id = 1,
            Title = "Kitab 1",
            BookCode = "BK-001",
            PublishedYear = 1937,
            TotalCopies = 5,
            AvailableCopies = 3
        };
    }
}