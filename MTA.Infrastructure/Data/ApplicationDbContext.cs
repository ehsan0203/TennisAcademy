using Microsoft.EntityFrameworkCore;
using MTA.Domain.Entities;

namespace MTA.Infrastructure.Data;

/// <summary>
/// Entity Framework Core DbContext for the application
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // DbSet properties
    public DbSet<Level> Levels { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<Package> Packages { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<PermissionsRole> PermissionsRoles { get; set; }
    public DbSet<UserCourseHistory> UserCourseHistories { get; set; }
    public DbSet<PackageHistory> PackageHistories { get; set; }
    public DbSet<Lookup> Lookups { get; set; }
    public DbSet<FAQCategory> FAQCategories { get; set; }
    public DbSet<QuestionFAQ> QuestionFAQs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
