using System.Reflection;
using ActualLab.Fusion.Authentication.Services;
using ActualLab.Fusion.EntityFramework.Operations;

namespace SmartQuiz.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContextBase(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // builder.Entity<ApplicationUser>().ToTable("Users");
        // builder.Entity<IdentityRole>().ToTable("Roles");
        // builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        // builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        // builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        // builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        // builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public DbSet<DbUser<string>> Users { get; protected set; } = null!;
    public DbSet<DbUserIdentity<string>> UserIdentities { get; protected set; } = null!;
    public DbSet<DbSessionInfo<string>> Sessions { get; protected set; } = null!;

    // Operations framework tables (required)
    public DbSet<DbOperation> Operations { get; protected set; } = null!;
    public DbSet<DbEvent> Events { get; protected set; } = null!;

    public DbSet<Flashcard> Flashcards { get; protected set; } = null!;
    public DbSet<FlashcardSet> FlashcardSets { get; protected set; } = null!;
}