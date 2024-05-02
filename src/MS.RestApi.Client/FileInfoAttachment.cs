using System.IO;
using MS.RestApi.Abstractions;
using MS.RestApi.Client.Extensions;

namespace MS.RestApi.Client;

public class FileInfoAttachment(FileInfo file) : IAttachment
{
    public string GetFileName() => file.Name;
    public string? GetContentType() => ContentTypeProvider.Default.TryGetContentType(file.Name, out var contentType) ? contentType : "application/octet-stream";
    public long? GetContentLength() => file.Length;
    public Stream GetFileStream() => file.OpenRead();

    public static implicit operator FileInfoAttachment(FileInfo fileInfo) => new(fileInfo);
}