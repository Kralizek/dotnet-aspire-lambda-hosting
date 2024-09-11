using Aspirant.Hosting;

using Kralizek.Aspire.Hosting;
// ReSharper disable CheckNamespace

namespace Aspire.Hosting;

public static class LambdaHostingExtensions
{
    public static IResourceBuilder<LambdaResource> AddLambdaLocalHosting(this IDistributedApplicationBuilder builder, string name)
    {
        var resource = new LambdaResource(name);

        return builder.AddWebApplication<LambdaResource, LambdaResourceLifecycleHook>(resource, excludeFromManifest: true);
    }

    public static IResourceBuilder<LambdaResource> AddAnnotationFunction(this IResourceBuilder<LambdaResource> builder, Type functionClass, string functionName)
    {
        if (functionClass.GetMethod(functionName) is { } method)
        {
            builder.Resource.Functions.Add(functionName, new Function(functionName, functionClass, method));
        }
        
        return builder;
    }

    public static IResourceBuilder<TResource> WithReference<TResource>(this IResourceBuilder<TResource> builder, IResourceBuilder<LambdaResource> lambda) where TResource : IResourceWithEnvironment
    {
        builder.WithEnvironment("AWS_ENDPOINT_URL_Lambda", lambda.GetEndpoint("http"));

        return builder;
    }
}