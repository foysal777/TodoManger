using Microsoft.AspNetCore.Mvc;
using TodoListManager.Application.Common;

namespace TodoListManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult<ApiResponse<T>> OkResult<T>(T data, string message = "Request successful")
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, message));
    }

    protected ActionResult<ApiResponse<T>> CreatedResult<T>(string actionName, object routeValues, T data, string message = "Resource created successfully")
    {
        return CreatedAtAction(actionName, routeValues, ApiResponse<T>.SuccessResponse(data, message));
    }
}
