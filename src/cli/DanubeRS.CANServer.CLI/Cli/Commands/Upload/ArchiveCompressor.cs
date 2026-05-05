using System.Formats.Tar;
using Microsoft.Extensions.Logging;
using ZstdSharp;

namespace DanubeRS.CANServer.CLI.Commands.Upload;

public static class ArchiveCompressor
{
    public static void CompressDateDirectory(string dateKey, string dateDirPath, string outputDirectory, ILogger logger)
    {
        var tarPath = Path.Combine(outputDirectory, $"{dateKey}.tar.zst");

        var increment = 1;
        while (File.Exists(tarPath))
        {
            tarPath = Path.Combine(outputDirectory, $"{dateKey}_{increment}.tar.zst");
            increment++;
        }

        try
        {
            var tempTar = Path.Combine(outputDirectory, $"{dateKey}.tar.tmp");

            TarFile.CreateFromDirectory(dateDirPath, tempTar, includeBaseDirectory: false);

            CompressFile(tempTar, tarPath);

            File.Delete(tempTar);
            logger.LogInformation("Created archive {ArchivePath}", tarPath);

            Directory.Delete(dateDirPath, recursive: true);
            logger.LogInformation("Removed date directory {DateDir}", dateDirPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create archive {ArchivePath}", tarPath);

            var tempTar = Path.Combine(outputDirectory, $"{dateKey}.tar.tmp");
            if (File.Exists(tempTar))
                File.Delete(tempTar);

            if (File.Exists(tarPath))
            {
                try { File.Delete(tarPath); } catch { }
            }
        }
    }

    private static void CompressFile(string sourcePath, string destPath)
    {
        using var sourceStream = File.OpenRead(sourcePath);
        using var destStream = File.OpenWrite(destPath);
        using var compressionStream = new CompressionStream(destStream, level: 19);

        sourceStream.CopyTo(compressionStream);
    }
}