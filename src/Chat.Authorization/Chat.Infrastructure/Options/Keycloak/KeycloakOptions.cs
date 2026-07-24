using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Options.Keycloak;

public class KeycloakOptions
{
    public const string SectionName = "IdentityProvider:Keycloak";

    [Required]
    public required string TokenEndpoint { get; set; }

    [Required]
    public required string LogoutEndpoint { get; set; }

    [Required]
    public required string ManagementApiEndpoint { get; set; }
    
    [Required]
    public required string Pkce { get; set; }

    [Required]
    [ValidateObjectMembers]
    public required IdentityProviderClientOptions UserClient { get; set; }

    [Required]
    [ValidateObjectMembers]
    public required IdentityProviderClientOptions AdminClient { get; set; }
}
