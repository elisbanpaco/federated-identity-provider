using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AuthServer.Host.Entities;
using AuthServer.Host.DTOs;

namespace AuthServer.Host.Controllers;

[ApiController]
[Route("api/[controller]")] // La URL será: http://localhost:5000/api/auth
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    // Inyectamos las herramientas que nos da Identity
    public AuthController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("register")] // POST a api/auth/register
    [Authorize(Roles = "Admin")] // Solo el Admin puede registrar nuevos usuarios
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        // 1. Verificamos si el usuario ya existe
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest("El correo ya está registrado.");
        }

        // 2. Verificamos si el rol solicitado existe en el sistema
        var roleExists = await _roleManager.RoleExistsAsync(request.Role);
        if (!roleExists)
        {
            return BadRequest($"El rol '{request.Role}' no existe.");
        }

        // 3. Creamos nuestra Entidad (Recuerda: IsActive ya es true por defecto)
        var newUser = new AppUser
        {
            Email = request.Email,
            UserName = request.Email
        };

        // 4. Magia de Identity: Guarda al usuario y hashea el password automáticamente
        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            // Si la clave es muy débil, Identity nos devuelve los errores aquí
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errores = errors });
        }

        // 5. Le asignamos el rol oficial en la base de datos
        await _userManager.AddToRoleAsync(newUser, request.Role);

        // 6. ¡Éxito! Devolvemos un 201 Created (No devolvemos la contraseña, por supuesto)
        return Created("", new { 
            Mensaje = "Usuario creado exitosamente", 
            Email = newUser.Email,
            Estado = newUser.IsActive ? "Activo" : "Inactivo"
        });
    }
}