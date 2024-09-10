using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kralizek.Aspire.Hosting;

public class LambdaEndpoint(LambdaResource resource, ILogger logger)
{
    public async Task<IResult> InvokeFunction(HttpContext context, string functionName, CancellationToken ct)
    {
        var match = Regexes.FunctionNameRegex().Match(functionName);
            
        if (!match.Success)
        {
            return TypedResults.BadRequest();
        }
            
        functionName = match.Groups[7].Value;

        if (!resource.Functions.TryGetValue(functionName, out var function))
        {
            logger.LogWarning("Function {FunctionName} not found", functionName);
            
            return TypedResults.BadRequest();
        }

        var generatedClassName = $"{function.Type.Name}_{function.Method.Name}_Generated";
        var generatedClassType = function.Type.Assembly.DefinedTypes.First(c => c.Name == generatedClassName);
        var generatedClass = ActivatorUtilities.CreateInstance(context.RequestServices, generatedClassType);
        var generatedMethod = generatedClassType.GetMethod(function.Method.Name)!;
        
        var parameterType = generatedMethod.GetParameters().First().ParameterType;
        var deserializedPayload = await JsonSerializer.DeserializeAsync(context.Request.Body, parameterType, cancellationToken: ct);

        var result = generatedMethod.Invoke(generatedClass, [deserializedPayload]);

        if (result is not Task task)
        {
            return TypedResults.BadRequest();
        }

        await task;

        var taskType = task.GetType();

        var resultProperty = taskType.GetProperty("Result");

        if (resultProperty is null)
        {
            return TypedResults.BadRequest();
        }
        
        var taskResult = resultProperty.GetValue(task);

        return Results.Ok(taskResult);
    }
}

internal static partial class Regexes
{
    [GeneratedRegex(@"^(arn:(aws[a-zA-Z-]*)?:lambda:)?([a-z]{2}(-gov)?-[a-z]+-\d{1}:)?(\d{12}:)?(function:)?([a-zA-Z0-9-_\.]+)(:(\$LATEST|[a-zA-Z0-9-_]+))?$")]
    public static partial Regex FunctionNameRegex();
}