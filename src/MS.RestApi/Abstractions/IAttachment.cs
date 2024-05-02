using System.Collections.Generic;
using System.IO;

namespace MS.RestApi.Abstractions;

public interface IAttachment
{
    string GetFileName();
    string? GetContentType();
    long? GetContentLength();
    Stream GetFileStream();
}