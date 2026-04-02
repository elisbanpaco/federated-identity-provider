using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthServer.Host.Entities;

namespace AuthServer.Host.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); // Crea las tablas de usuarios y roles

        // ¡ATENCIÓN AQUÍ!
        // Esta línea inyecta las tablas especiales (Tokens, Aplicaciones, Autorizaciones)
        // que OpenIddict necesita para manejar a tus múltiples sistemas independientes.
        builder.UseOpenIddict();
    }
}