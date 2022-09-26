namespace Contracts;

public static class Conventions
{
    public static string CommandEndpointName<TCommand>()
        where TCommand: ICommand
    {
        Type commandType = typeof(TCommand);
        return $"{commandType.Namespace}:{commandType.Name}";
    }

    public static Uri CommandEndpointUri<TCommand>()
        where TCommand: ICommand
    {
        return new Uri($"exchange:{CommandEndpointName<TCommand>()}");
    }
}