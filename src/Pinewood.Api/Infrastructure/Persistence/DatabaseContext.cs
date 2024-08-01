using Microsoft.EntityFrameworkCore;
using Pinewood.Api.Entities;

namespace Pinewood.Api.Infrastructure.Persistence;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var customers = Enumerable.Range(1, 25).Select(i => new Customer
        {
            Id = i,
            Name = $"Customer {i}",
            Email = $"customer{i}@example.com",
            PhoneNumber = $"0123456789",
            DateOfBirth = DateTime.Now.AddYears(-30 - i).Date
        }).ToList();

        modelBuilder.Entity<Customer>().HasData(customers);
    }
}