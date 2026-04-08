using federated_identity_provider.Data;
using federated_identity_provider.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using federated_identity_provider.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Leemos la cadena de conexión del appsettings.json
    options.UseSqlServer(builder.Configuration.GetConnectionString("sqlServerDb"));
    
    // Le decimos a EF Core que registre las tablas de OpenIddict
    options.UseOpenIddict();
});

// builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<AppUser, IdentityRole>(options => 
    {
        options.SignIn.RequireConfirmedAccount = true; 
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddOpenIddict()
    // 1. Configurar la capa de Core (Integración con Base de Datos)
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
    })
    // 2. Configurar el Servidor OIDC
    .AddServer(options =>
    {
        // Definimos las rutas a las que los clientes irán a pedir login y tokens
        options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetTokenEndpointUris("/connect/token");

        // Configuramos flujos permitidos (Authorization Code para web, Client Credentials para APIs)
        options.AllowAuthorizationCodeFlow()
               .AllowClientCredentialsFlow()
               .AllowRefreshTokenFlow()
               .AllowPasswordFlow();

        // Para desarrollo: Usamos certificados falsos automáticos (para no lidiar con SSL reales aún)
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();
               
        // Registramos los scopes que nuestro SSO va a entender, permite tomar el control manual del proceso de validación antes de que se emita el token.
        options.RegisterScopes("openid", "profile", "email", "roles");
        
        // Enlazamos OpenIddict con las rutas de ASP.NET Core
        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough();
    })
    // 3. Configurar la Validación (Para que el propio servidor entienda los tokens que emite)
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

builder.Services.AddControllers();
// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication(); // Lee el JWT y lo decodifica
app.UseAuthorization(); // Revisa si el usuario tiene permisos tipo Admin u otro

app.MapStaticAssets();
app.MapControllers(); // Enciende las rutas de las APIs

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
