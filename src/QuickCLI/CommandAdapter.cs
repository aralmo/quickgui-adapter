namespace QuickCLI;

internal class CommandAdapter : ICommandAdapter
{
    private readonly Delegate @delegate;

    internal CommandAdapter(Delegate @delegate)
    {
        this.@delegate = @delegate;
    }

    public object? Execute(string[] args)
    {
        if (args.Any() && args[0].TrimStart('-').ToLower() == "help")
        {
            Output.Write(GetHelp(args));
            return null;
        }

        var pars = Parser.Parameters(@delegate);
        var invocation = Parser.GetInvocationParameters(pars, args);
        Parser.EnsureRequiredsAreSet(pars, invocation);
        object? result = null;
        try
        {
            result = @delegate.DynamicInvoke(invocation);
        }
        catch (AdapterException ex)
        {
            Output.AdapterException(ex);
        }
        catch (Exception ex)
        {
            Output.Exception(ex);
        }

        Output.Ok(result);
        return result;
    }

    public string GetHelp(string[] args)
        => String
            .Join("\r\n", Parser
            .Parameters(@delegate)
            .Select(parameterHelp));

    internal static string parameterHelp(Parameter p)
    => p.Type switch
        {
            Type t when t == typeof(bool) => $"-{p.Name} : {p.Description}",
            _ =>  $"-{p.Name} <value> : {p.Description}"
        };
}


