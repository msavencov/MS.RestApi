using MediatR;
using MS.RestApi.Abstractions;

namespace Templates.Attachment;

[EndPoint("attachment", "Test")]
public record AttachmentRequest : IRequest
{
    public required IAttachment Attachment { get; init; }
    public required AttachmentsCollection Attachments { get; init; }
}

