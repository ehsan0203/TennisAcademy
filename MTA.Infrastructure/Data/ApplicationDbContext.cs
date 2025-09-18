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

        // --- Level ---
        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Title).IsUnique();
        });

        // --- Role ---
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Title).IsUnique();
        });

        // --- Permission ---
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.Title).IsUnique();
        });

        // --- UserProfile ---
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DateOfBirth).IsRequired();
            entity.Property(e => e.Experience).IsRequired();

            entity.HasOne(e => e.Account)
                .WithOne(a => a.UserProfile)
                .HasForeignKey<UserProfile>(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.SkillLevel)
                .WithMany()
                .HasForeignKey(e => e.SkillLevelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Account ---
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Image).HasMaxLength(500);

            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne(e => e.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Course ---
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

            // Icon MediaFile
            entity.HasOne(e => e.IconMediaFile)
                  .WithMany()
                  .HasForeignKey(e => e.IconMediaFileId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Poster MediaFile
            entity.HasOne(e => e.PosterMediaFile)
                  .WithMany()
                  .HasForeignKey(e => e.PosterMediaFileId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Level)
                .WithMany(e => e.Courses)
                .HasForeignKey(e => e.LevelId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Lesson ---
        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.MediaFiles)
                .WithOne(m => m.Lesson)
                .HasForeignKey(m => m.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- MediaFile ---
        modelBuilder.Entity<MediaFile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(1000);

            entity.HasOne(e => e.Type)
                .WithMany()
                .HasForeignKey(e => e.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Lesson)
                .WithMany(e => e.MediaFiles)
                .HasForeignKey(e => e.LessonId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Message)
                .WithMany(e => e.MediaFiles)
                .HasForeignKey(m => m.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- Package ---
        modelBuilder.Entity<Package>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TicketCount).IsRequired();
            entity.Property(e => e.MessageCount).IsRequired();
            entity.Property(e => e.Duration).IsRequired();

            entity.HasOne(e => e.DurationUnit)
                .WithMany()
                .HasForeignKey(e => e.DurationUnitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Ticket ---
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Topic).IsRequired().HasMaxLength(200);

            entity.HasOne(e => e.Status)
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Package)
                .WithMany()
                .HasForeignKey(e => e.PackageId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Message ---
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired().HasMaxLength(2000);

            entity.HasOne(e => e.Ticket)
                .WithMany(e => e.Messages)
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Sender)
                .WithMany()
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.MediaFiles)
                .WithOne(e => e.Message)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- PermissionsRole ---
        modelBuilder.Entity<PermissionsRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Role)
                .WithMany(e => e.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(e => e.PermissionsRoles)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- UserCourseHistory ---
        modelBuilder.Entity<UserCourseHistory>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Course)
                .WithMany(e => e.UserCourseHistory)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Account)
                .WithMany(e => e.UserCourseHistory)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- PackageHistory ---
        modelBuilder.Entity<PackageHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ExpiredDate).IsRequired();
            entity.Property(e => e.RemainingTickets).IsRequired();
            entity.Property(e => e.RemainingMessages).IsRequired();

            entity.HasOne(e => e.Package)
                .WithMany(e => e.PackageHistories)
                .HasForeignKey(e => e.PackageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Account)
                .WithMany(e => e.PackageHistory)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Lookup entity
        modelBuilder.Entity<Lookup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(200);
            
            // Composite unique index on Category and Key
            entity.HasIndex(e => new { e.Category, e.Key }).IsUnique();
        });

        // Configure FAQCategory entity
        modelBuilder.Entity<FAQCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.SortOrder).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
            
            // Relationship with Questions
            entity.HasMany(e => e.Questions)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure QuestionFAQ entity
        modelBuilder.Entity<QuestionFAQ>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuestionText).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.AnswerText).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.IsActive).IsRequired();
            
            // Relationship with Category
            entity.HasOne(e => e.Category)
                .WithMany(e => e.Questions)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        modelBuilder.Entity<Lookup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(200);
        });

        // --- FAQCategory ---
        modelBuilder.Entity<FAQCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        });

        // --- QuestionFAQ ---
        modelBuilder.Entity<QuestionFAQ>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuestionText).IsRequired().HasMaxLength(500);
            entity.Property(e => e.AnswerText).IsRequired();

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Questions)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        //LookupSeed.Seed(modelBuilder);
        //RoleSeed.Seed(modelBuilder);
        //LevelSeed.Seed(modelBuilder);
        //PermissionSeed.Seed(modelBuilder);
    }
}
