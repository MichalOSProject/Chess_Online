using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Chess_Online.Server.Data.Entity;
using Microsoft.AspNetCore.Identity;

namespace Chess_Online.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<JwtTokens> JwtTokens { get; set; }
        public virtual DbSet<GameInstanceEntity> GameInstancesEntity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(x => x.UserName)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
