using GameballTask.Models;
using Microsoft.EntityFrameworkCore;

namespace GameballTask.Data;

public class ApiDbContext : DbContext{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) 
        : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
}