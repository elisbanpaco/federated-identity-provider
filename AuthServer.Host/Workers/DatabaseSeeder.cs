using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using AuthServer.Host.Entities;

namespace AuthServer.Host.Workers;

// Solo inyectamos el IServiceProvider. Todo lo demás se resuelve adentro.
public class DatabaseSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Abrimos un alcance temporal de memoria
        using var scope = serviceProvider.CreateScope();

        // Extraemos las herramientas de manera segura
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var appManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        // 1. Crear los Roles si no existen
        string[] roleNames = { "Admin", "Student" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Crear el Super Administrador inicial
        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new AppUser { UserName = adminEmail, Email = adminEmail };
            var createAdmin = await userManager.CreateAsync(admin, "PasswordSeguro123!");
            
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        // 3. Registrar los Sistemas de la Institución (OpenIddict)
        // Ejemplo: Le damos permiso a un portal web frontend para usar este servidor
        var portalEstudiantesClientId = "portal-estudiantes-web";
        
        if (await appManager.FindByClientIdAsync(portalEstudiantesClientId, cancellationToken) is null)
        {
            await appManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = portalEstudiantesClientId,
                ClientSecret = "secreto-super-seguro-institucion-2026",
                DisplayName = "Portal Web de Estudiantes",
                Permissions =
                {
                    // Permisos básicos para hacer login con usuario y contraseña
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles
                }
            }, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}