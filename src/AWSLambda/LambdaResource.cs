using System.Reflection;
using Aspirant.Hosting;

namespace Kralizek.Aspire.Hosting;

public class LambdaResource(string name) : WebApplicationResource(name)
{
    public Dictionary<string, Function> Functions { get; } = [];
}

public record struct Function(string Name, Type Type, MethodInfo Method);