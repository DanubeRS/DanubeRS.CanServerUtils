using System.CommandLine.Parsing;

namespace DanubeRS.CANServer.CLI;

public static class OptionParsers
{
    public static Func<ArgumentResult, Uri?> UriParser { get; } = result =>
    {
        var value = result.Tokens.SingleOrDefault()?.Value;
        if (string.IsNullOrEmpty(value))
            return null;
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            result.AddError($"Invalid URI: {value}");
            return null;
        }
        return uri;
    };
}
