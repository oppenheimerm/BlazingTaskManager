using BlazingTaskManager.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlazingTaskManager.AuthenticationAPI.Data
{
    public class AuthenticationDataContext : DbContext
    {
        public AuthenticationDataContext(DbContextOptions<AuthenticationDataContext> options)
            :base(options)
        { }

        public DbSet<BTUser> Users { get; set; } = default!;
        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<UserRole> UserRoles { get; set; } = default!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            //  add unique constraints to a property via the EF Fluent API
            builder.HasIndex(r => r.RoleCode)
                .IsUnique();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { }
    }
}
