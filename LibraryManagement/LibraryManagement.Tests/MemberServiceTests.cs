using LibraryManagement.Application.Services;
using LibraryManagement.Core.DTOs;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Moq;
using Xunit;

namespace LibraryManagement.Tests;

public class MemberServiceTests
{
    [Fact]
    public async Task GetPagedAsync_WhenCalled_ReturnsPagedMembers()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        var members = new List<Member>
        {
            new()
            {
                Id = 1,
                FirstName = "Aysel",
                LastName = "Memmedova",
                Email = "aysel.memmedova@example.com",
                PhoneNumber = "+994501112233",
                MembershipDate = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = 2,
                FirstName = "Elvin",
                LastName = "Aliyev",
                Email = "elvin.aliyev@example.com",
                PhoneNumber = "+994502223344",
                MembershipDate = DateTime.UtcNow,
                IsActive = true
            }
        };

        repositoryMock
            .Setup(repository => repository.GetPagedAsync(
                1,
                2,
                "firstName",
                "asc"))
            .ReturnsAsync((members, 3));

        var service = new MemberService(repositoryMock.Object);

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
        Assert.Equal("Aysel", result.Items[0].FirstName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMemberExists_ReturnsMember()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        var member = new Member
        {
            Id = 1,
            FirstName = "Aysel",
            LastName = "Memmedova",
            Email = "aysel.memmedova@example.com",
            PhoneNumber = "+994501112233",
            MembershipDate = DateTime.UtcNow,
            IsActive = true
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(member);

        var service = new MemberService(repositoryMock.Object);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Aysel", result.FirstName);
        Assert.Equal("aysel.memmedova@example.com", result.Email);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_WhenMemberDoesNotExist_ReturnsNull()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Member?)null);

        var service = new MemberService(repositoryMock.Object);

        var result = await service.GetByIdAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailIsUnique_CreatesMember()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        var dto = new MemberCreateDto
        {
            FirstName = "Leyla",
            LastName = "Huseynova",
            Email = "  LEYLA.HUSEYNOVA@EXAMPLE.COM  ",
            PhoneNumber = "+994503334455"
        };

        repositoryMock
            .Setup(repository => repository.EmailExistsAsync(
                "leyla.huseynova@example.com",
                null))
            .ReturnsAsync(false);

        var createdMember = new Member
        {
            Id = 3,
            FirstName = "Leyla",
            LastName = "Huseynova",
            Email = "leyla.huseynova@example.com",
            PhoneNumber = "+994503334455",
            MembershipDate = DateTime.UtcNow,
            IsActive = true
        };

        repositoryMock
            .Setup(repository => repository.CreateAsync(
                It.Is<Member>(member =>
                    member.FirstName == "Leyla" &&
                    member.LastName == "Huseynova" &&
                    member.Email ==
                        "leyla.huseynova@example.com" &&
                    member.IsActive)))
            .ReturnsAsync(createdMember);

        var service = new MemberService(repositoryMock.Object);

        var result = await service.CreateAsync(dto);

        Assert.Equal(3, result.Id);
        Assert.Equal("leyla.huseynova@example.com", result.Email);
        Assert.True(result.IsActive);

        repositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Member>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ThrowsException()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        var dto = new MemberCreateDto
        {
            FirstName = "Leyla",
            LastName = "Huseynova",
            Email = "leyla.huseynova@example.com",
            PhoneNumber = "+994503334455"
        };

        repositoryMock
            .Setup(repository => repository.EmailExistsAsync(
                "leyla.huseynova@example.com",
                null))
            .ReturnsAsync(true);

        var service = new MemberService(repositoryMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateAsync(dto));

        repositoryMock.Verify(
            repository => repository.CreateAsync(
                It.IsAny<Member>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenMemberExists_UpdatesMember()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        var member = new Member
        {
            Id = 1,
            FirstName = "Kohne",
            LastName = "Ad",
            Email = "kohne@example.com",
            PhoneNumber = "+994504445566",
            MembershipDate = DateTime.UtcNow,
            IsActive = true
        };

        var dto = new MemberUpdateDto
        {
            FirstName = "Aysel",
            LastName = "Memmedova",
            Email = "  AYSEL.MEMMEDOVA@EXAMPLE.COM  ",
            PhoneNumber = "+994505556677",
            IsActive = false
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(member);

        repositoryMock
            .Setup(repository => repository.EmailExistsAsync(
                "aysel.memmedova@example.com",
                1))
            .ReturnsAsync(false);

        repositoryMock
            .Setup(repository => repository.UpdateAsync(member))
            .Returns(Task.CompletedTask);

        var service = new MemberService(repositoryMock.Object);

        var result = await service.UpdateAsync(1, dto);

        Assert.True(result);
        Assert.Equal("Aysel", member.FirstName);
        Assert.Equal("Memmedova", member.LastName);
        Assert.Equal(
            "aysel.memmedova@example.com",
            member.Email);
        Assert.False(member.IsActive);

        repositoryMock.Verify(
            repository => repository.UpdateAsync(member),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenMemberDoesNotExist_ReturnsFalse()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Member?)null);

        var service = new MemberService(repositoryMock.Object);

        var dto = new MemberUpdateDto
        {
            FirstName = "Aysel",
            LastName = "Memmedova",
            Email = "aysel.memmedova@example.com",
            PhoneNumber = "+994501112233",
            IsActive = true
        };

        var result = await service.UpdateAsync(99, dto);

        Assert.False(result);

        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Member>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmailAlreadyExists_ThrowsException()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        var member = new Member
        {
            Id = 1,
            FirstName = "Aysel",
            LastName = "Memmedova",
            Email = "aysel@example.com",
            PhoneNumber = "+994501112233",
            MembershipDate = DateTime.UtcNow,
            IsActive = true
        };

        var dto = new MemberUpdateDto
        {
            FirstName = "Aysel",
            LastName = "Memmedova",
            Email = "elvin.aliyev@example.com",
            PhoneNumber = "+994501112233",
            IsActive = true
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(member);

        repositoryMock
            .Setup(repository => repository.EmailExistsAsync(
                "elvin.aliyev@example.com",
                1))
            .ReturnsAsync(true);

        var service = new MemberService(repositoryMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.UpdateAsync(1, dto));

        repositoryMock.Verify(
            repository => repository.UpdateAsync(
                It.IsAny<Member>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenMemberExists_DeletesMember()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        var member = new Member
        {
            Id = 1,
            FirstName = "Aysel",
            LastName = "Memmedova",
            Email = "aysel.memmedova@example.com"
        };

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(member);

        repositoryMock
            .Setup(repository => repository.DeleteAsync(member))
            .Returns(Task.CompletedTask);

        var service = new MemberService(repositoryMock.Object);

        var result = await service.DeleteAsync(1);

        Assert.True(result);

        repositoryMock.Verify(
            repository => repository.DeleteAsync(member),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenMemberDoesNotExist_ReturnsFalse()
    {
        var repositoryMock = new Mock<IMemberRepository>();

        repositoryMock
            .Setup(repository => repository.GetByIdAsync(99))
            .ReturnsAsync((Member?)null);

        var service = new MemberService(repositoryMock.Object);

        var result = await service.DeleteAsync(99);

        Assert.False(result);

        repositoryMock.Verify(
            repository => repository.DeleteAsync(
                It.IsAny<Member>()),
            Times.Never);
    }
}