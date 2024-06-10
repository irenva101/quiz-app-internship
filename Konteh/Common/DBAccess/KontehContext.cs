using Common.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Common.DBAccess;

public class KontehContext : DbContext
{
    public KontehContext() { }

    public KontehContext(DbContextOptions<KontehContext> options) : base(options)
    {
    }

    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}