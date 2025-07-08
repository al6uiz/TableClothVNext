using System.IO.Compression;

namespace TableCloth3.Shared.Services;

public sealed class ArchiveExpander
{
    public async Task ExpandArchiveAsync(Stream zipStream, string destinationDirectoryPath, CancellationToken cancellationToken = default)
    {
        if (!zipStream.CanRead)
            throw new ArgumentException("Selected zip stream is not readable.", nameof(zipStream));
        if (!zipStream.CanSeek)
            throw new ArgumentException("Selected zip stream is not seekable.", nameof(zipStream));

        using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

        foreach (var eachEntry in zipArchive.Entries)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eachEntry.Name))
                    continue;

                var destPath = Path.Combine(destinationDirectoryPath, eachEntry.Name);

                using var outputStream = File.OpenWrite(destPath);
                using var eachStream = eachEntry.Open();
                await eachStream.CopyToAsync(outputStream, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new IOException($"Cannot extract the file '{eachEntry.Name}' to '{destinationDirectoryPath}'.", ex);
            }
        }
    }
}
