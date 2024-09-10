using FindNationalityFunction;

var builder = DistributedApplication.CreateBuilder(args);

var lambda = builder.AddLambdaLocalHosting("lambda")
    .WithEndpoint(scheme: "http", port: 52000);

lambda.AddFunction(typeof(Functions), nameof(Functions.GetCountriesAsync));

builder.AddProject<Projects.FindNationalityWeb>("web")
    .WithReference(lambda)
    .WithEnvironment("AWS_ACCESS_KEY_ID", "fake")
    .WithEnvironment("AWS_SECRET_ACCESS_KEY", "fake")
    .WithExternalHttpEndpoints();

builder.Build().Run();