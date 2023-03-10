namespace QuickCLI;

public interface ICommandAdapter
{
    object? Execute(string[] args);
    string GetHelp(string[] args);
}





