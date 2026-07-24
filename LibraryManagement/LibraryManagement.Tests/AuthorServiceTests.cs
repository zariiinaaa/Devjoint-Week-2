using LibraryManagement.Application.Services;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Moq;
using Xunit;

namespace LibraryManagement.Tests;

public class AuthorServiceTests
{
    [Fact]
    public async Task GetPagedAsync_WhenCalled_ReturnsPagedAuthors()
    {
        var repositoryMock = new Mock<IAuthorRepository>();

        var authors = new List<Author>
        {
            new()
            {
                Id = 1,
                FirstName = "Fyodor",
                LastName = "Dostoevsky",
                Biography = "Russian writer"
            },
            new()
            {
                Id = 2,
                FirstName = "George",
                LastName = "Orwell",
                Biography = "English writer"
            }
        };

        repositoryMock
            .Setup(repository => repository.GetPagedAsync(
                1,
                2,
                "firstName",
                "asc"))
            .ReturnsAsync((authors, 3));

        var service = new AuthorService(repositoryMock.Object);

        var query = new ListQueryDto
        {
            PageNumber = 1,
            PageSize = 2,
            SortBy = "firstName",
            SortDirection = "asc"
        };

        var result = await service.GetPagedAsync(query);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(2, result.TotalPages);
        Assert.Equal("Fyodor", result.Items[0].FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAuthorExists_ReturnsAuthor()
    {
        var repositoryMock = new Mock<IAuthorRepository>();

        var author = new Author
        {
            Id = 1,
            FirstName = "Fyodor",
            LastName = "Dostoevsky",
            Biography = "Russian writer"
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(author);

        var service = new AuthorService(repositoryMock.Object);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Fyodor", result.FirstName);
        Assert.Equal("Dostoevsky", result.LastName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenAuthorDoesNotExist_ReturnsNull()
    {
        var repositoryMock = new Mock<IAuthorRepository>();

        repositoryMock.Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Author?)null);

        var service = new AuthorService(repositoryMock.Object);

        var result = await service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenCalled_CreatesAndReturnsAuthor()
    {
        var repositoryMock = new Mock<IAuthorRepository>();

        var dto = new AuthorCreateDto
        {
            FirstName = "George",
            LastName = "Orwell",
            Biography = "English writer"
        };

        var createdAuthor = new Author
        {
            Id = 5,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Biography = dto.Biography
        };

        repositoryMock
            .Setup(repository => repository.CreateAsync(
                It.Is<Author>(author =>
                    author.FirstName == dto.FirstName &&
                    author.LastName == dto.LastName &&
                    author.Biography == dto.Biography)))
            .ReturnsAsync(createdAuthor);

        var service = new AuthorService(repositoryMock.Object);

        var result = await service.CreateAsync(dto);

        Assert.Equal(5, result.Id);
        Assert.Equal("George", result.FirstName);
        Assert.Equal("Orwell", result.LastName);

        repositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Author>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenAuthorExists_UpdatesAndReturnsTrue()
    {
        var repositoryMock = new Mock<IAuthorRepository>();

        var author = new Author
        {
            Id = 1,
            FirstName = "Old",
            LastName = "Name",
            Biography = "Old biography"
        };

        var dto = new AuthorUpdateDto
        {
            FirstName = "New",
            LastName = "Name",
            Biography = "New biography"
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(author);

        repositoryMock
            .Setup(repository => repository.UpdateAsync(author))
            .Returns(Task.CompletedTask);

        var service = new AuthorService(repositoryMock.Object);

        var result = await service.UpdateAsync(1, dto);

        Assert.True(result);
        Assert.Equal("New", author.FirstName);
        Assert.Equal("Name", author.LastName);
        Assert.Equal("New biography", author.Biography);

        repositoryMock.Verify(
repository => repository.UpdateAsync(author),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenAuthorDoesNotExist_ReturnsFalse()
    {
        var repositoryMock = new Mock<IAuthorRepository>();

        repositoryMock.Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Author?)null);

        var service = new AuthorService(repositoryMock.Object);

        var dto = new AuthorUpdateDto
        {
            FirstName = "New",
            LastName = "Author",
            Biography = "Biography"
        };

        var result = await service.UpdateAsync(99, dto);

        Assert.False(result);

        repositoryMock.Verify(
            repository => repository.UpdateAsync( It.IsAny<Author>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenAuthorExists_DeletesAndReturnsTrue()
    {
        var repositoryMock = new Mock<IAuthorRepository>();

        var author = new Author
        {
            Id = 1,
            FirstName = "George",
            LastName = "Orwell"
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(author);

        repositoryMock
            .Setup(repository => repository.DeleteAsync(author))
            .Returns(Task.CompletedTask);

        var service = new AuthorService(repositoryMock.Object);

        var result = await service.DeleteAsync(1);

        Assert.True(result);

        repositoryMock.Verify(
            repository => repository.DeleteAsync(author),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenAuthorDoesNotExist_ReturnsFalse()
    {
        var repositoryMock = new Mock<IAuthorRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Author?)null);

        var service = new AuthorService(repositoryMock.Object);

        var result = await service.DeleteAsync(99);

        Assert.False(result);

        repositoryMock.Verify(repository => repository.DeleteAsync(It.IsAny<Author>()),
            Times.Never);
    }
}