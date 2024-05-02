using contract.Account;
using Microsoft.AspNetCore.Mvc;
using MS.RestApi.Server.Filters;

namespace server;

[ApiController, Route("my")]
public class MyController : ControllerBase
{
    [HttpPost("files")]
    public IActionResult Submit([FromForm, MultipartModel] Profile model) => Ok(model);
}