using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResultsKit;

namespace ResultsKit.Http;

public static partial class Process
{
    public static ActionResult MvcResult(Result result)
    {
        return result switch
        {
            _ when result.Success => new OkResult(),
            _ when result.IsFailure && result.Error is not null => ToObjectResult(result.Error),
            _ => new BadRequestObjectResult(
                CreateProblemDetails(
                    new Error("bad_request", "The request could not be processed."),
                    StatusCodes.Status400BadRequest))
        };
    }

    public static ActionResult<T> MvcResult<T>(Result<T> result)
    {
        return result switch
        {
            _ when result.Success && result.Value is not null => new OkObjectResult(result.Value),
            _ => MvcResult(result as Result)
        };
    }

    private static ObjectResult ToObjectResult(Error error)
    {
        var statusCode = GetStatusCode(error);
        return new ObjectResult(CreateProblemDetails(error, statusCode))
        {
            StatusCode = statusCode
        };
    }

    private static ProblemDetails CreateProblemDetails(Error error, int statusCode)
    {
        var details = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Message
        };

        switch (error)
        {
            case NotFoundError notFound:
                details.Extensions["resource"] = notFound.Resource;
                details.Extensions["reference"] = notFound.Reference;
                break;
            case ConflictError conflict:
                details.Extensions["resource"] = conflict.Resource;
                details.Extensions["reference"] = conflict.Reference;
                details.Extensions["conflictingReference"] = conflict.ConflictingReference;
                break;
            case ValidationError validation:
                details.Extensions["messages"] = validation.Messages;
                break;
            case UnauthorizedError unauthorized when unauthorized.Action is not null:
                details.Extensions["action"] = unauthorized.Action;
                break;
        }

        return details;
    }
}
