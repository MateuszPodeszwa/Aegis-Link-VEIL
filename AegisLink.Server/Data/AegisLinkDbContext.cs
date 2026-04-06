using Microsoft.EntityFrameworkCore;

namespace AegisLink.Server.Data
{
    public class AegisLinkDbContext : DbContext
    {
        public AegisLinkDbContext(DbContextOptions<AegisLinkDbContext> options) : base(options)
        {
        }

        public DbSet<UserKey> UserKeys => Set<UserKey>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserKey>(entity =>
            {
                entity.HasKey(e => e.AegisId);
                entity.HasIndex(e => e.PublicKey).IsUnique();
            });
        }
    }
}
