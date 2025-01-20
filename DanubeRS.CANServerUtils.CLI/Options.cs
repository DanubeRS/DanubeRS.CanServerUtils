using CommandLine;

namespace DanubeRS.CANServerUtils.CLI;

public interface IDownloadOptions
{
    string Address { get; }
    string OutputPath { get; }
    bool Remove { get; }
}

[Verb("download", false, new[] { "dl", "d" }, HelpText = "Downloads CAN server log files")]
public class DownloadOptions : IDownloadOptions
{
    public string Address { get; set; }

    public string OutputPath { get; set; }

    public bool Remove { get; set; }
}

public interface IParseOptions
{
    string? InputPath { get; }

    IEnumerable<string> Databases { get; }
}

[Verb("parse")]
public class ParseOptions : IParseOptions
{
    [Option('i', "input", Required = false, HelpText = "Remove files after download")]
    public string? InputPath { get; set; }
    [Option('d', "database", Required = true, HelpText = "Database files to use for message decoding")]
    public IEnumerable<string> Databases { get; set; }
}

[Verb("downloadAndParse")]
public class DownloadAndParseOptions : IDownloadOptions, IParseOptions
{
    [Option('s', "source", Required = true, HelpText = "CAN server address")]
    public string Address { get; set; }

    [Option('o', "output", Required = true, HelpText = "Path to store downloaded files")]
    public string OutputPath { get; set; }

    [Option("rm", Required = false, HelpText = "Remove files from CANServer after download", Default = false)]
    public bool Remove { get; set; }

    [Option('i', "input", Required = false, HelpText = "Remove files after download")]
    public string? InputPath { get; set; }

    [Option('d', "database", Required = true, HelpText = "Database files to use for message decoding")]
    public IEnumerable<string> Databases { get; set; }
}