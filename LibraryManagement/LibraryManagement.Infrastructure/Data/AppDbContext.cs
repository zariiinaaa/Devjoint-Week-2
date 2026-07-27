using LibraryManagement.Core.Common;
using LibraryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();

    public DbSet<Book> Books => Set<Book>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<User> Users => Set<User>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>()
            .ToTable("Books");

        modelBuilder.Entity<Book>()
            .HasKey(book => book.Id);

        modelBuilder.Entity<Book>()
            .Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Book>()
            .Property(book => book.BookCode)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Book>()
            .HasIndex(book => book.BookCode)
            .IsUnique();

        modelBuilder.Entity<Book>()
            .Property(book => book.PublishedYear)
            .IsRequired();

        modelBuilder.Entity<Book>()
            .Property(book => book.TotalCopies)
            .IsRequired();

        modelBuilder.Entity<Book>()
            .Property(book => book.AvailableCopies)
            .IsRequired();

        modelBuilder.Entity<Book>()
            .HasMany(book => book.Authors)
            .WithMany(author => author.Books);

        modelBuilder.Entity<Loan>()
            .HasOne(loan => loan.Book)
             .WithMany(book => book.Loans)
             .HasForeignKey(loan => loan.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(loan => loan.Member)
            .WithMany(member => member.Loans)
            .HasForeignKey(loan => loan.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Member>()
            .HasIndex(member => member.Email)
            .IsUnique();
        modelBuilder.Entity<User>(userBuilder =>
        {
            userBuilder.ToTable("Users");

            userBuilder.HasKey(user => user.Id);

            userBuilder.Property(user => user.Username)
                .IsRequired()
                .HasMaxLength(100);

            userBuilder.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(255);

            userBuilder.Property(user => user.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            userBuilder.HasIndex(user => user.Username)
                .IsUnique();

            userBuilder.HasIndex(user => user.Email)
                .IsUnique();

            userBuilder.Property(user => user.Role)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue(UserRoles.User);

    
        });
    }
}   