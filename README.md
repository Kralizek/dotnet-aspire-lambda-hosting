> [!CAUTION]
> **This library is very much at an experimental stage!**  
> Things seem to work but the exposed API might change

# AWS Lambda Hosting on .NET Aspire

A .NET Aspire Hosting extension that allows AWS Lambda to be launched within the context of the AppHost to facilitate the local development.

## Objectives of this library

- [x] Be able to host your function locally
- [ ] Be able to host more than one function
- [ ] Be able to host functions from more than one project
- [x] Be able to set a breakpoint into your function
- [x] Be able to invoke the function from other parts of the solution seamlessly
- [ ] Support different Lambda programming models
  - [x] Annotation Framework
  - [ ] Classic Lambda model
  - [ ] [AWSLambdaSharpTemplate](https://github.com/Kralizek/AWSLambdaSharpTemplate)
  - [ ] More programming models
- [ ] Be able to publish traces, logs and metrics to the Aspire dashboard
- [ ] Be published as a NuGet package to be added to your AppHost
- [ ] Be well-tested
- [ ] Make you forget about hassles caused by working with Lambda

## What we got so far

This is what you can do so far

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var lambda = builder.AddLambdaLocalHosting("lambda")
    .WithEndpoint(scheme: "http", port: 52000);

lambda.AddAnnotationFunction(typeof(Functions), nameof(Functions.GetCountriesAsync));

builder.AddProject<Projects.FindNationalityWeb>("web")
    .WithReference(lambda)
    .WithEnvironment("AWS_ACCESS_KEY_ID", "fake")
    .WithEnvironment("AWS_SECRET_ACCESS_KEY", "fake")
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

