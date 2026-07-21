using System.ComponentModel.DataAnnotations;

namespace Chat.Infrastructure.Options;

public class IdentityProviderOptions
{
    public const string SectionName = "IdentityProvider";
    
    [Required]
    public required string Authority {get; set;}
    [Required]
    public required string Issuer {get; set;}
    [Required]
    public required string Audience { get; set; }
    [Required]
    public required string TokenEndpoint { get; set; }
    [Required]
    public required string LogoutEndpoint { get; set; }
    [Required]
    public required string ManagementApiEndpoint { get; set; }
    [Required]
    public required string MetadataAddress { get; set; }
    [Required]
    public required string Pkce { get; set; }
    [Required]
    public required string[] SelectedScopes {get; set;}
}
