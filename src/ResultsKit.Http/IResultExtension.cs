using Microsoft.AspNetCore.Http;
using ResultsKit;

namespace ResultsKit.Http;

public static partial class Process
{
    public static IResult ApiResult(Result result)
    {
        return result switch
        {
            _ when result.Success => Results.Ok(),
            _ when result.IsFailure && result.Error is not null => ToProblemResult(result.Error),
            _ => Results.BadRequest()
        };
    }

    public static IResult ApiResult<T>(Result<T> result)
    {
        return result switch
        {
            _ when result.Success => Results.Ok(result.Value),
            _ => ApiResult(result as Result)
        };
    }

    private static IResult ToProblemResult(Error error)
    {
        return Results.Problem(
            detail: error.Message,
            title: error.Code,
            statusCode: GetStatusCode(error),
            extensions: CreateExtensions(error));
    }

    private static Dictionary<string, object?> CreateExtensions(Error error)
    {
        var extensions = new Dictionary<string, object?>();

        switch (error)
        {
            case NotFoundError notFound:
                extensions["resource"] = notFound.Resource;
                extensions["reference"] = notFound.Reference;
                break;
            case ConflictError conflict:
                extensions["resource"] = conflict.Resource;
                extensions["reference"] = conflict.Reference;
                extensions["conflictingReference"] = conflict.ConflictingReference;
                break;
            case ValidationError validation:
                extensions["messages"] = validation.Messages;
                break;
            case UnauthorizedError unauthorized when unauthorized.Action is not null:
                extensions["action"] = unauthorized.Action;
                break;
        }

        return extensions;
    }

    private static int GetStatusCode(Error error)
    {
        return error switch
        {
            UnauthorizedError => StatusCodes.Status401Unauthorized,
            NotFoundError => StatusCodes.Status404NotFound,
            ConflictError => StatusCodes.Status409Conflict,
            ValidationError => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest
        };
    }
}
