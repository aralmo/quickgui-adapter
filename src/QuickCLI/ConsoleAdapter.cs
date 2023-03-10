namespace QuickCLI;

public static class ConsoleAdapter
{
    
    public static ICommandAdapter For(Delegate @delegate)
        => new CommandAdapter(@delegate);

    public static RoutedCommandsAdapter Map(string command, Delegate @delegate)
        => new RoutedCommandsAdapter(command, @delegate);
}