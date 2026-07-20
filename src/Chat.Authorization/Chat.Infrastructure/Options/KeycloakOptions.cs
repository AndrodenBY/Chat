using System.ComponentModel.DataAnnotations;

namespace Chat.Infrastructure.Options;

public class KeycloakOptions
{
    public const string SectionName = "Keycloak";

    [Required]
    public required string Authority {get; set;}
    [Required]
    public required string Issuer {get; set;}
    [Required]
    public required string Audience {get; set;}
    [Required] 
    public required string ClientId { get; set; }
    [Required]
    public required string ClientSecret {get; set;}
    [Required]
    public required string Pkce {get; set;}
    [Required]
    public required string[] SelectedScopes {get; set;}
    [Required]
    public required string MetadataAddress {get; set;}
}

