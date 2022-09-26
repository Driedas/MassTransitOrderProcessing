using System.Reflection;
using MassTransit;
using Serilog;

namespace Contracts;

public static class CommandConventions
{
    public static void Apply(Assembly contractAssembly)
    {
        var mapMethod = typeof(EndpointConvention).GetMethod("Map",
                BindingFlags.Public | BindingFlags.Static,
                new Type[] { typeof(Uri)})!
            .GetGenericMethodDefinition();

        foreach (var type in contractAssembly.GetExportedTypes()
            .Where(t => t.IsAssignableTo(typeof(ICommand))))
        {
            var uri = $"exchange:{type.Namespace}:{type.Name}";
            
            Log.Logger.Information("Registering {Queue} as destination for {Type}", uri, type.FullName);
            
            mapMethod.MakeGenericMethod(type)
                .Invoke(null, new object[] { new Uri(uri) });
        }
    }
}