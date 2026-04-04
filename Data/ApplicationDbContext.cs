using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using federated_identity_provider.Models;
namespace federated_identity_provider.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.CodigoUniversitario).IsRequired();
                entity.HasIndex(e => e.CodigoUniversitario).IsUnique();
            });

            // ¡Importante! Aquí le decimos a EF Core que incluya las tablas de OpenIddict
            builder.UseOpenIddict();
        }
    }
}