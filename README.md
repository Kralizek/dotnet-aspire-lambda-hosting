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

## How to run the sample

This repository comes with a sample.

Assuming you are in the root of the repo, you can launch the sample as it follows:

```bash
$ cd ./samples/FindNationalityAnnotations/

$ dotnet run --project ./tools/AppHost/
Building...
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 8.2.0+75fdcff28495bdd643f6323133a7d411df71ab70
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Application host directory is: C:\Users\rg1844\Development\My\AWSLambdaAspireHosting\samples\FindNationalityAnnotations\tools\AppHost
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17078
info: Aspire.Hosting.DistributedApplication[0]
      Login to the dashboard at https://localhost:17078/login?t=0831ba5f23b9a83f435fd19ba599ee75
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application started. Press Ctrl+C to shut down.
```

Clicking on the link, will give you the usual Aspire dashboard.

## Disclaimer

I used some code taken from the [Aspirant proejct](https://github.com/aspirant-project/aspirant/) repository. Hopefully in the future I can just reference it as a dependency rather than copying it ;)
