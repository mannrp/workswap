using Microsoft.AspNetCore.Mvc;
using workswap.Common;

namespace workswap.Controllers;

/// <summary>
/// Base controller providing helper methods to map Result objects to ActionResult.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is null ? NoContent() : Ok(result.Value);
        }

        return result.StatusCode switch
        {
            System.Net.HttpStatusCode.NotFound => NotFound(new { error = result.Error }),
            System.Net.HttpStatusCode.Unauthorized => Unauthorized(new { error = result.Error }),
            System.Net.HttpStatusCode.Forbidden => Forbid(),
            _ => BadRequest(new { error = result.Error })
        };
    }
}
