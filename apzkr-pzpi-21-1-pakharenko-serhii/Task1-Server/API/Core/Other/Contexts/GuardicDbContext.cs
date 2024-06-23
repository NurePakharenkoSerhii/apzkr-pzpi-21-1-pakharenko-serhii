using API.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Core.Contexts;

public class GuardicDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Guard> Guards { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Thing> Things { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLog> OrderLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public GuardicDbContext(DbContextOptions<GuardicDbContext> options)  : base(options) {}
}
