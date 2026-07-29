using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Chat.Api.Options;

public class EndpointOptions
{
    public const string SectionName = "Endpoints";
    
    [Required]
    [ValidateObjectMembers]
    public required RestEndpointOptions Rest { get; init; }
    [Required]
    [ValidateObjectMembers]
    public required GrpcEndpointOptions Grpc { get; init; }
}

public class RestEndpointOptions
{
    [Required]
    public required int Port { get; init; }
    [Required]
    public required bool UseHttps { get; init; }
}

public class GrpcEndpointOptions
{
    [Required]
    public int Port { get; init; }
}
