using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace FindNationalityFunction;

public record Response(
    [property: JsonPropertyName("country")] Country[] Countries,
    [property: JsonPropertyName("name")] string Name
);

public record Country(
    [property: JsonPropertyName("country_id")] string CountryCode,
    [property: JsonPropertyName("probability")] double Probability
);

public record FindNationalityOptions
{
    public double MinimumThreshold { get; init; }
}

[LambdaStartup]
public class Startup
{
    public IConfiguration Configuration { get; init; } = CreateConfiguration();

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);

        services.AddLogging(logging => logging.AddConsole());

        services.AddOptions();

        services.Configure<FindNationalityOptions>(Configuration.GetSection("FindNationality"));

        services.AddHttpClient("Nationality", http => http.BaseAddress = new Uri("https://api.nationalize.io"));

        services.AddTransient<HttpClient>(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Nationality"));
    }

    private static IConfiguration CreateConfiguration()
    {
        var builder = new ConfigurationBuilder();

        builder.AddEnvironmentVariables();

        return builder.Build();
    }
}

public class Functions(IOptions<FindNationalityOptions> options, ILogger<Functions> logger)
{
    private readonly FindNationalityOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly ILogger<Functions> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [LambdaFunction]
    public async Task<List<Country>> GetCountriesAsync([FromServices] HttpClient http, string name)
    {
        _logger.LogInformation("Fetching countries for name {Name}", name);

        var response = await http.GetFromJsonAsync<Response>($"/?name={name}");

        return response?.Countries.Where(c => c.Probability >= _options.MinimumThreshold).ToList() ?? new List<Country>();
    }
}