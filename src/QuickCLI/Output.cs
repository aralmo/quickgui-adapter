namespace QuickCLI;

public static class Output
{
    internal static Action<string> Write { get; set; }
    = (string text) => Console.Write(text);

    internal static Action<object?> Ok { get; set; }
    = res =>
    {
        if (res != null)
            Console.WriteLine(res.ToString());
    };

    internal static Action<AdapterException> AdapterException { get; set; }
    = (ex) => throw ex;

    internal static Action<Exception> Exception { get; set; }
    = (ex) => throw ex;
}