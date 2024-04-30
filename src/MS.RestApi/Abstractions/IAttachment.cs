using System.Collections.Generic;
using System.IO;

namespace MS.RestApi.Abstractions;


public sealed class AttachmentsCollection : List<IAttachment>
{
    public AttachmentsCollection()
    {
    }

    public AttachmentsCollection(IEnumerable<IAttachment> collection) : base(collection)
    {
    }
}

public interface IAttachment 
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long ContentLength { get; set; }
    public Stream GetFileStream();
}