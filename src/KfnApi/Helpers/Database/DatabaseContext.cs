using KfnApi.Abstractions;
using KfnApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KfnApi.Helpers.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Producer> Producers => Set<Producer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<PriceByWeight> Prices => Set<PriceByWeight>();
    public DbSet<UserReport> UserReports => Set<UserReport>();
    public DbSet<ProducerReport> ProducerReports => Set<ProducerReport>();
    public DbSet<ProducerApprovalForm> ProducerApprovalForms => Set<ProducerApprovalForm>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(left => left.Products)
            .WithMany(right => right.Orders)
            .UsingEntity(join => join.ToTable("OrderProducts"));

        modelBuilder.Entity<Producer>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Product>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Order>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<UserReport>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ProducerReport>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<ProducerApprovalForm>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Modified)
            .Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
            if (modifiedEntry is IAuditable auditableEntity)
                auditableEntity.UpdatedAt = DateTime.UtcNow;

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Modified)
            .Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
            if (modifiedEntry is IAuditable auditableEntity)
                auditableEntity.UpdatedAt = DateTime.UtcNow;

        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Modified)
            .Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
            if (modifiedEntry is IAuditable auditableEntity)
                auditableEntity.UpdatedAt = DateTime.UtcNow;

        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override int SaveChanges()
    {
        var modifiedEntries = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Modified)
            .Select(x => x.Entity);

        foreach (var modifiedEntry in modifiedEntries)
            if (modifiedEntry is IAuditable auditableEntity)
                auditableEntity.UpdatedAt = DateTime.UtcNow;

        return base.SaveChanges();
    }
}
