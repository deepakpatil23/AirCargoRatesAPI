using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<RoleModule> RoleModules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<RoleModule>()
            .HasKey(rm => new { rm.RoleId, rm.ModuleId });

        modelBuilder.Entity<Module>()
            .HasOne(m => m.ParentModule)
            .WithMany(m => m.ChildModules)
            .HasForeignKey(m => m.ParentModuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
