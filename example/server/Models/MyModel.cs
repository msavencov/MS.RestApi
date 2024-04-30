using Microsoft.AspNetCore.Mvc;
using MS.RestApi.Abstractions;
using MS.RestApi.Server.Filters;

namespace server.Models;

public class MyModel
{
    public string Description { get; set; }
    public IAttachment Attachment { get; set; }
    public AttachmentsCollection Attachments { get; set; }
}


[ApiController, Route("my")]
public class MyController : ControllerBase
{
    [BindFormFile(nameof(model.Attachment), nameof(model))]
    [BindFormFile(nameof(model.Attachments), nameof(model))]
    [HttpPost("files")]
    public IActionResult Submit([FromForm] MyModel model) => Ok(new
    {
        model.Description,
        model.Attachment?.FileName,
        model.Attachments?.Count,
    });
}