using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace federated_identity_provider.Models;

public class AppUser : IdentityUser
{
    [Required(ErrorMessage = "El campo {0} es requerido")]
    public string CodigoUniversitario { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public DateTime DateCreatedAccount { get; set; } = DateTime.Now;
}   