using QuickCLI;

internal class Program
{
    private static void Main(string[] args)
    {
        ConsoleAdapter
            .For(ExampleConsoleMethod)
            .Execute(args);
    }

    static string ExampleConsoleMethod(string first, string second, bool toUpper)
    {
        string result = $"{first} {second}";
        return toUpper ? result.ToUpper() : result;
    }
}