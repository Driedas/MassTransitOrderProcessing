using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;

namespace Hosting.OpenTelemetry;

public class UsernameEnrichingProcessor
    : BaseProcessor<Activity>
{
    private readonly IServiceProvider _container;

    public UsernameEnrichingProcessor(IServiceProvider container)
    {
        _container = container;
    }

    public override void OnEnd(Activity data)
    {
        var http = _container.GetService<IHttpContextAccessor>();

        var principal = http?.HttpContext?.User;
        if (principal is null) return;
        
        data.SetTag("enduser.id", principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value);
        data.SetTag("enduser.token_issuer", principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Issuer);
    }
}