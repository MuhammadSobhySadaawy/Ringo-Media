using test123.Models;
using Microsoft.EntityFrameworkCore;
namespace test123.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Reminder> Reminders { get; set; }
}
