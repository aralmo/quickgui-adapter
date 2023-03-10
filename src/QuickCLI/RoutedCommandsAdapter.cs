namespace QuickCLI;

public class RoutedCommandsAdapter : ICommandAdapter
{
    HashSet<(string key, CommandAdapter adapter)> adapters;
    internal RoutedCommandsAdapter(string key, Delegate @delegate)
    {
        adapters = new(new[] { (key, new CommandAdapter(@delegate)) });
    }

    public RoutedCommandsAdapter Map(string command, Delegate @delegate)
    {
        adapters.Add((command, new CommandAdapter(@delegate)));
        return this;
    }

    public object? Execute(string[] args)
    {
        if (args.Any() && args[0].TrimStart('-').ToLower() == "help")
        {
            Output.Write(GetHelp(args));
            return null;
        }

        var match = adapters.FirstOrDefault(d => d.key.ToLower() == args[0].ToLower());
        if (match == default)
            throw new AdapterException($"command {args[0]} not found");

        return match.adapter.Execute(args.Skip(1).ToArray());
    }

    public string GetHelp(string[] args)
    => String
        .Join("\r\n\r\n", adapters
        .Select(a => $"{a.key}\r\n{a.adapter.GetHelp(args.Skip(1).ToArray())}"));

}


