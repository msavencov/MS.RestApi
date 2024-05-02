using System.IO;
using MS.RestApi.Abstractions;
using MS.RestApi.Client.Extensions;

namespace MS.RestApi.Client;

public class Attachment(Stream stream) : IAttachment
{
    public required string FileName { get; set; }
    public string? ContentType { get; set; }
    public long? ContentLength { get; set; }
    
    public Stream GetFileStream()
    {
        return stream;
    }

    public static Attachment FromFile(FileInfo file)
    {
        return new Attachment(file.OpenRead())
        {
            FileName = file.Name,
            ContentType = ContentTypeProvider.Default.TryGetContentType(file.Name, out var ct) ? ct : "application/octet-stream",
            ContentLength = file.Length,
        };
    }

    public static implicit operator Attachment(FileInfo fileInfo) => FromFile(fileInfo);
}