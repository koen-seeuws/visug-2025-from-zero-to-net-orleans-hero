using System.Net;
using Microsoft.AspNetCore.Mvc;
using TheCodeKitchen.Application.Contracts.Errors;
using TheCodeKitchen.Application.Contracts.Results;

namespace TheCodeKitchen.Presentation;

public static class ControllerBaseExtensions
{
    public static IActionResult MatchActionResult<T>(
        this ControllerBase controllerBase,
        Result<T> result
    ) where T : notnull
        => result.Match<IActionResult>(
            onSuccess: value => controllerBase.Ok(value),
            onFail: controllerBase.Fail
        );

    public static IActionResult MatchActionResult<T>(
        this ControllerBase controllerBase,
        Result<T> result,
        Func<ControllerBase, T, ActionResult> onSuccess
    ) where T : notnull
        => result.Match(
            onSuccess: value => onSuccess(controllerBase, value),
            onFail: controllerBase.Fail
        );

    public static IActionResult MatchActionResult(
        this ControllerBase controllerBase,
        Result<TheCodeKitchenUnit> result
    )
        => result.Match(
            onSuccess: _ => controllerBase.NoContent(),
            onFail: controllerBase.Fail
        );

    private static IActionResult Fail(this ControllerBase controllerBase, Error error)
    {
        switch (error)
        {
            case NotFoundError:
                return controllerBase.NotFound(error.Message);
            case ValidationError validationError:
                return controllerBase.BadRequest(validationError);
            case BusinessError:
                return controllerBase.BadRequest(error.Message);
            case UnauthorizedError:
                return controllerBase.StatusCode((int) HttpStatusCode.Unauthorized, error.Message);
            case NotImplementedError:
                return controllerBase.StatusCode((int) HttpStatusCode.NotImplemented, "This operation has not been implemented yet.");
            default:
                return controllerBase.StatusCode((int) HttpStatusCode.InternalServerError, error.Message);
        }
    }
}