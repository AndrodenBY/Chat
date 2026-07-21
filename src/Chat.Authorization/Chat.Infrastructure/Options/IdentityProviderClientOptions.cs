using System.ComponentModel.DataAnnotations;

namespace Chat.Infrastructure.Options;

public class IdentityProviderClientOptions
{
    public const string UserClient = "UserClient";
    public const string AdminClient = "AdminClient";
    
    [Required]
    public required string ClientId { get; set; }
    [Required]
    public required string ClientSecret { get; set; }
}
