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
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Upload> Uploads => Set<Upload>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Producer> Producers => Set<Producer>();
    public DbSet<UserReport> UserReports => Set<UserReport>();
    public DbSet<PriceByWeight> Prices => Set<PriceByWeight>();
    public DbSet<ApprovalForm> ApprovalForms => Set<ApprovalForm>();
    public DbSet<ProducerReport> ProducerReports => Set<ProducerReport>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasMany(order => order.Products)
            .WithMany(product => product.Orders)
            .UsingEntity(join => join.ToTable("OrderProducts"));

        modelBuilder.Entity<User>()
            .HasMany(user => user.Uploads)
            .WithMany(upload => upload.Users)
            .UsingEntity(join => join.ToTable("UserUploads"));

        modelBuilder.Entity<Producer>()
            .HasMany(producer => producer.Uploads)
            .WithMany(upload => upload.Producers)
            .UsingEntity(join => join.ToTable("ProducerUploads"));

        modelBuilder.Entity<Product>()
            .HasMany(product => product.Uploads)
            .WithMany(upload => upload.Products)
            .UsingEntity(join => join.ToTable("ProductUploads"));

        modelBuilder.Entity<ApprovalForm>()
            .HasMany(approvalForm => approvalForm.Uploads)
            .WithMany(upload => upload.ApprovalForms)
            .UsingEntity(join => join.ToTable("ApprovalFormUploads"));

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

        modelBuilder.Entity<ApprovalForm>()
            .Property(x => x.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Upload>()
            .Property(x => x.DateUploaded)
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
