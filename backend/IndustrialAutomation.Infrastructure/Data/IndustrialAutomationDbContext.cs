using IndustrialAutomation.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace IndustrialAutomation.Infrastructure.Data;

public class IndustrialAutomationDbContext : DbContext
{
    public IndustrialAutomationDbContext(DbContextOptions<IndustrialAutomationDbContext> options) : base(options)
    {
    }

        public DbSet<AutomationJob> AutomationJobs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TestExecution> TestExecutions { get; set; }
        public DbSet<WebAutomation> WebAutomations { get; set; }
        public DbSet<JobSchedule> JobSchedules { get; set; }
        public DbSet<MLModel> MLModels { get; set; }
        public DbSet<AITrainingData> AITrainingData { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure AutomationJob
        modelBuilder.Entity<AutomationJob>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.StatusId).IsRequired();
            entity.Property(e => e.JobTypeId).IsRequired();
            entity.Property(e => e.Configuration).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.AssignedTo).HasMaxLength(100);
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure TestExecution with performance optimizations
        modelBuilder.Entity<TestExecution>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TestName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TestTypeId).IsRequired();
            entity.Property(e => e.StatusId).IsRequired();
            entity.Property(e => e.TestSuite).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TestData).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ExpectedResult).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ActualResult).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.AIAnalysis).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ConfidenceScore).HasMaxLength(10);
            entity.Property(e => e.TestEnvironment).HasMaxLength(100);
            entity.Property(e => e.Browser).HasMaxLength(50);
            entity.Property(e => e.Device).HasMaxLength(50);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            entity.Property(e => e.ExecutedBy).HasMaxLength(100);
            
            // Performance indexes
            entity.HasIndex(e => e.StatusId).HasDatabaseName("IX_TestExecutions_StatusId");
            entity.HasIndex(e => e.TestTypeId).HasDatabaseName("IX_TestExecutions_TestTypeId");
            entity.HasIndex(e => e.TestSuite).HasDatabaseName("IX_TestExecutions_TestSuite");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_TestExecutions_CreatedAt");
            entity.HasIndex(e => new { e.StatusId, e.TestTypeId }).HasDatabaseName("IX_TestExecutions_StatusId_TestTypeId");
            entity.HasIndex(e => new { e.CreatedAt, e.StatusId }).HasDatabaseName("IX_TestExecutions_CreatedAt_StatusId");
        });

        // Configure WebAutomation with performance optimizations
        modelBuilder.Entity<WebAutomation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AutomationName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.WebsiteUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.StatusId).IsRequired();
            entity.Property(e => e.JobTypeId).IsRequired();
            entity.Property(e => e.TargetElement).HasMaxLength(500);
            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.InputData).HasColumnType("nvarchar(max)");
            entity.Property(e => e.OutputData).HasColumnType("nvarchar(max)");
            entity.Property(e => e.AIPrompt).HasColumnType("nvarchar(max)");
            entity.Property(e => e.AIResponse).HasColumnType("nvarchar(max)");
            entity.Property(e => e.ElementSelector).HasMaxLength(1000);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.Browser).HasMaxLength(50);
            entity.Property(e => e.Device).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.ViewportSize).HasMaxLength(20);
            entity.Property(e => e.ConfidenceScore).HasMaxLength(10);
            entity.Property(e => e.CreatedBy).HasMaxLength(100);
            
            // Performance indexes
            entity.HasIndex(e => e.StatusId).HasDatabaseName("IX_WebAutomations_StatusId");
            entity.HasIndex(e => e.JobTypeId).HasDatabaseName("IX_WebAutomations_JobTypeId");
            entity.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_WebAutomations_CreatedAt");
            entity.HasIndex(e => new { e.StatusId, e.JobTypeId }).HasDatabaseName("IX_WebAutomations_StatusId_JobTypeId");
        });

        // Configure JobSchedule with performance optimizations
        modelBuilder.Entity<JobSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JobName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.JobType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CronExpression).HasMaxLength(100);
            entity.Property(e => e.Configuration).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Priority).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TimeZone).HasMaxLength(50);
            entity.Property(e => e.ErrorMessage).HasMaxLength(2000);
            entity.Property(e => e.ExecutionHistory).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Notifications).HasColumnType("nvarchar(max)");
            entity.Property(e => e.Dependencies).HasColumnType("nvarchar(max)");
            
            // Performance indexes
            entity.HasIndex(e => e.Status).HasDatabaseName("IX_JobSchedules_Status");
            entity.HasIndex(e => e.JobType).HasDatabaseName("IX_JobSchedules_JobType");
            entity.HasIndex(e => e.IsEnabled).HasDatabaseName("IX_JobSchedules_IsEnabled");
            entity.HasIndex(e => e.NextRunTime).HasDatabaseName("IX_JobSchedules_NextRunTime");
            entity.HasIndex(e => new { e.Status, e.IsEnabled }).HasDatabaseName("IX_JobSchedules_Status_Enabled");
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed admin user
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@bosch.com",
                PasswordHash = "AQAAAAEAACcQAAAAEExampleHash", // This should be properly hashed
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
