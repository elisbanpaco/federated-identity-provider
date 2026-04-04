using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using federated_identity_provider.Models;
using Microsoft.AspNetCore;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace federated_identity_provider.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthorizationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("~/connect/token")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                          throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByEmailAsync(request.Username); // Buscamos el Email
                if (user == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "El email o la contrase\u00f1a son incorrectos."
                        })
                    );
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "El email o la contrase\u00f1a son incorrectos."
                        })
                    );
                }

                var principal = await _signInManager.CreateUserPrincipalAsync(user);
                if (!string.IsNullOrEmpty(user.CodigoUniversitario))
                {
                    principal.SetClaim("codigo_universitario", user.CodigoUniversitario);
                }
                principal.SetScopes(request.GetScopes());
                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(GetDestinations(claim, principal));
                }
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        private IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            switch (claim.Type)
            {   
                //case ClaimTypes.Name: //quite el name porque no lo neceisot pero si mantengo su ID del usuario
                case ClaimTypes.NameIdentifier:
                case OpenIddictConstants.Claims.Subject:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case ClaimTypes.Email:
                    if (principal.HasScope(OpenIddictConstants.Scopes.Email))
                    {
                        yield return OpenIddictConstants.Destinations.AccessToken;
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                       
                    }
                     yield break;

                case ClaimTypes.Role:
                    if (principal.HasScope(OpenIddictConstants.Scopes.Roles))
                    {
                        yield return OpenIddictConstants.Destinations.AccessToken;
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    }
                    yield break;

                case "codigo_universitario":
                    if (principal.HasScope(OpenIddictConstants.Scopes.Profile))
                    {
                        yield return OpenIddictConstants.Destinations.AccessToken;
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    }
                    yield break;
                default:
                    yield break;
            }
        }

    }
}