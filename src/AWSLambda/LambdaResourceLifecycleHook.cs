using Aspirant.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Kralizek.Aspire.Hosting;

public class LambdaResourceLifecycleHook(
    IHostEnvironment hostEnvironment,
    DistributedApplicationExecutionContext executionContext,
    ResourceNotificationService resourceNotificationService,
    ResourceLoggerService resourceLoggerService) : WebApplicationResourceLifecycleHook<LambdaResource>(hostEnvironment, executionContext, resourceNotificationService, resourceLoggerService)

{
    protected override string ResourceTypeName => "AWS.Lambda";

    protected override ValueTask ConfigureBuilderAsync(WebApplicationBuilder builder, LambdaResource resource, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    protected override ValueTask ConfigureApplicationAsync(WebApplication app, LambdaResource resource, CancellationToken cancellationToken)
    {
        var endpoint = new LambdaEndpoint(resource, app.Logger);
        
        app.MapPost("/2015-03-31/functions/{functionName:required}/invocations", endpoint.InvokeFunction);

        return ValueTask.CompletedTask;
    }
}