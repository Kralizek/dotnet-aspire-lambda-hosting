using System.Text.Json;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FindNationalityWeb.Pages;

public class IndexModel(IAmazonLambda lambda) : PageModel
{
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken, string name = "Renato")
    {
        
        var request = new InvokeRequest
        {
            FunctionName = "GetCountriesAsync",
            InvocationType = InvocationType.RequestResponse,
            Payload = JsonSerializer.Serialize(name)
        };

        var response = await lambda.InvokeAsync(request, cancellationToken);

        using var sr = new StreamReader(response.Payload);

        var result = await sr.ReadToEndAsync(cancellationToken);

        return Content(result, "application/json");
    }
}
