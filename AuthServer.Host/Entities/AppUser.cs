using Microsoft.AspNetCore.Identity;
namespace AuthServer.Host.Entities;

public class AppUser : IdentityUser
{
    public bool IsActive { get; private set; } = true;


    public void DeactivateAccount()
    {
        IsActive = false;
    }

    public void ActivateAccount()
    {
        IsActive = true;
    }

}