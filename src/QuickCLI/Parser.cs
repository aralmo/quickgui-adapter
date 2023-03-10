using System.ComponentModel;
using System.Reflection;

namespace QuickCLI;

internal static class Parser
{
    internal static object?[] GetInvocationParameters(Parameter[] parameters, string[] arguments)
    {
        var args = ParseArguments(arguments).ToArray();
        var invocationArguments = parameters
                       .Select(p => p.Type == typeof(bool) ? false : p.DefaultValue)
                       .ToArray();

        object? parseValue(Parameter p, string? v)
        => p.Type switch
        {
            Type t when t == typeof(bool) => true,
            Type t when t == typeof(DateTime?) => DateTime.Parse(v!),//todo: tryparse
            Type t when t == typeof(DateTime) => DateTime.Parse(v!),
            _ => Convert.ChangeType(v, p.Type)
        };

        for (int i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (a.name == null)
                invocationArguments[i] = parseValue(parameters[i], a.value);
            else
            {
                var idx = parameters.IndexOf(x => x.Name.ToLower() == a.name.ToLower());
                if (idx == -1)
                    throw new AdapterException($"invalid parameter {a.name}");

                invocationArguments[idx] = parseValue(parameters[idx], a.value);
            }
        }
        return invocationArguments;
    }

    internal static IEnumerable<(string? name, string? value)> ParseArguments(string[] arguments)
    {
        bool namedParsed = false;
        for (int i = 0; i < arguments.Length; i++)
        {
            string arg = arguments[i].Trim();
            string? next = i < arguments.Length - 1 ? arguments[i + 1].Trim() : null;

            if (arg.StartsWith('-'))
            {
                if (next != null && !next.StartsWith('-'))
                {
                    namedParsed = true;
                    yield return (arg.TrimStart('-'), next);
                    i++;
                }
                else
                    yield return (arg.TrimStart('-'), null);
            }
            else
            {
                if (namedParsed)
                    throw new AdapterException("named parameters should go after inline parameters");

                yield return (null, arg);
            }
        }
    }

    
    internal static void EnsureRequiredsAreSet(Parameter[] pars, object?[] invocation)
    {
        for (int i = 0; i < pars.Length; i++)
        {
            Parameter? par = pars[i];
            if (par.Optional == false && invocation[i] == par.DefaultValue)
                throw new AdapterException($"{par.Name} must be set");
        }
    }

    internal static Parameter[] Parameters(Delegate del)
    => del.Method
        .GetParameters()
        .Where(p => p.Name != null)
        .Select(p =>
            new Parameter()
            {
                Optional = p.IsOptional,
                DefaultValue = p.DefaultValue,
                Name = p.Name!,
                Type = p.ParameterType,
                Description = getDescription(p)
            })
        .ToArray();

    private static string getDescription(ParameterInfo p)
    => p.CustomAttributes
        .FirstOrDefault(ca => ca.AttributeType == typeof(DescriptionAttribute))?
        .ConstructorArguments[0].Value as string ?? 
        $"{(p.IsOptional || p.ParameterType == typeof(bool)?"optional":"required")} {p.ParameterType.Name}";
        
}
