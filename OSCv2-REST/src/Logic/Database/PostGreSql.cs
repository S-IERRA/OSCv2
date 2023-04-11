using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Shared.Layouts;

namespace OSCv2.Logic.Database;

public class EntityFrameworkFactory : IDesignTimeDbContextFactory<EntityFramework>
{
    public EntityFramework CreateDbContext(string[]? args = null)
    {
        DbContextOptionsBuilder builder = new DbContextOptionsBuilder<EntityFramework>();
        builder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=root;Database=OSCv2"); //ToDo: Convert this to a appsettings.json
        
        return new(builder.Options);
    }
}

public class EntityFramework : DbContext
{
    public EntityFramework(DbContextOptions options) : base(options) { }
    
    public DbSet<Account> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}