using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace federated_identity_provider.Controllers; // ProtectedRoutesController.cs

// Este NO es el AuthorizationController, este sería un controlador de tu API de negocio
[ApiController]
[Route("api/documentos")]
public class DocumentosController : ControllerBase
{
    // ¡AQUÍ ESTÁ LA MAGIA! El "Cadenero"
    [Authorize(Roles = "Admin, SuperAdmin")] 
    [HttpDelete("borrar")]
    public IActionResult BorrarDocumento()
    {
        return Ok("Documento borrado con éxito");
    }

    // Esta ruta la puede ver cualquier usuario logueado, sea admin o platano
    [Authorize] 
    [HttpGet("leer")]
    public IActionResult LeerDocumento()
    {
        return Ok("Aquí está tu documento");
    }
}