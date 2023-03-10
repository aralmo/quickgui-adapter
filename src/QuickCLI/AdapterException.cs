namespace QuickCLI;

[System.Serializable]
public class AdapterException : System.Exception
{
    public override string? StackTrace => null;
    public AdapterException() { }
    public AdapterException(string message) : base(message) { }
    public AdapterException(string message, System.Exception inner) : base(message, inner) { }
    protected AdapterException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}