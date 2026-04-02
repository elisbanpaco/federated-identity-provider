using AuthServer.Host.Components;
using Microsoft.EntityFrameworkCore;
using AuthServer.Host.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Extraemos la cadena de conexión. 
// .NET buscará automáticamente en appsettings.json, User Secrets o Variables de Entorno.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Aplicamos tu excelente validación Fail-Fast
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("FATAL: No se encontró la cadena de conexión 'DefaultConnection'. Revisa tu configuración segura o las variables de entorno.");
}

// 3. Registramos el DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    
    // Opcional para OpenIddict: Usar el paquete nativo de Entity Framework
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    // 1. Configurar la integración con Entity Framework Core
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();
    })
    // 2. Configurar el Servidor en sí
    .AddServer(options =>
    {
        // El endpoint donde los sistemas pedirán el Token (Ej: POST api/connect/token)
        options.SetTokenEndpointUris("/connect/token");

        // Habilitamos los flujos permitidos (Cómo van a iniciar sesión)
        options.AllowPasswordFlow(); // Login clásico con Usuario y Contraseña
        options.AllowRefreshTokenFlow(); // Para mantener la sesión viva sin pedir clave a cada rato

        // ¡Súper importante para desarrollo! Genera certificados falsos para firmar los tokens.
        // En producción, aquí pondrías certificados reales de la institución.
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        // Le decimos que use el pipeline de ASP.NET Core para manejar las peticiones HTTP
        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough();
    });

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
