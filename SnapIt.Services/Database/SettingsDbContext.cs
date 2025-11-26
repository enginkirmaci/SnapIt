using Microsoft.EntityFrameworkCore;
using SnapIt.Services.Database.Entities;

namespace SnapIt.Services.Database;

/// <summary>
/// Database context for storing SnapIt settings in SQLite
/// </summary>
public class SettingsDbContext : DbContext
{
    private readonly string _databasePath;

    public DbSet<SettingsEntity> Settings { get; set; }
    public DbSet<ExcludedApplicationSettingsEntity> ExcludedApplicationSettings { get; set; }
    public DbSet<ApplicationGroupSettingsEntity> ApplicationGroupSettings { get; set; }
    public DbSet<StandaloneLicenseEntity> StandaloneLicenses { get; set; }
    public DbSet<LayoutEntity> Layouts { get; set; }

    public SettingsDbContext(string databasePath)
    {
        _databasePath = databasePath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_databasePath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Settings configuration - only one instance
        modelBuilder.Entity<SettingsEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).IsRequired();
            entity.Property(e => e.JsonData).IsRequired();
            entity.Property(e => e.LastModified).IsRequired();
        });

        // ExcludedApplicationSettings configuration - only one instance
        modelBuilder.Entity<ExcludedApplicationSettingsEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).IsRequired();
            entity.Property(e => e.JsonData).IsRequired();
            entity.Property(e => e.LastModified).IsRequired();
        });

        // ApplicationGroupSettings configuration - only one instance
        modelBuilder.Entity<ApplicationGroupSettingsEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Version).IsRequired();
            entity.Property(e => e.JsonData).IsRequired();
            entity.Property(e => e.LastModified).IsRequired();
        });

        // StandaloneLicense configuration - only one instance
        modelBuilder.Entity<StandaloneLicenseEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JsonData).IsRequired();
            entity.Property(e => e.LastModified).IsRequired();
        });

        // Layout configuration - multiple instances indexed by Guid
        modelBuilder.Entity<LayoutEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Guid).IsUnique();
            entity.Property(e => e.Guid).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Version).IsRequired();
            entity.Property(e => e.JsonData).IsRequired();
            entity.Property(e => e.LastModified).IsRequired();
        });
    }
}
