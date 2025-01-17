using CommandLine;

namespace DanubeRS.CANServerUtils.CLI;

[Verb("download", false, new[] { "dl", "d" }, HelpText = "Downloads CAN server log files")]
public class DownloadOptions
{
    [Option('s', "source", Required = true, HelpText = "CAN server address")]
    public string Address { get; set; }

    [Option('o', "output", Required = true, HelpText = "CAN server address")]
    public string OutputPath { get; set; }

    [Option("rm", Required = false, HelpText = "Remove files after download", Default = false)]
    public bool Remove { get; set; }
}

[Verb("parse")]
public class ParseOptions
{
    [Option('i', "input", Required = false, HelpText = "Remove files after download")]
    public string? InputPath { get; set; }

    [Option('d', "database", Required = true, HelpText = "Database files to use for message decoding")]
    public IEnumerable<string> Databases { get; set; }
}