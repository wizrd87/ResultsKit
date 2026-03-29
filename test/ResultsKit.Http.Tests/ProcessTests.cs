using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ResultsKit.Http.Tests;

public sealed class ProcessTests
{
    [Fact]
    public void ApiResult_ReturnsOk_ForSuccessfulResult()
    {
        var result = Process.ApiResult(Result.Ok());

        Assert.IsType<Ok>(result);
    }

    [Fact]
    public void ApiResult_ReturnsProblem_ForNotFound()
    {
        var result = Assert.IsType<ProblemHttpResult>(Process.ApiResult(Result.Fail(new NotFoundError("widget", 10))));

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("not_found", result.ProblemDetails.Title);
        Assert.Equal("The requested resource was not found.", result.ProblemDetails.Detail);
        Assert.Equal("widget", result.ProblemDetails.Extensions["resource"]);
        Assert.Equal(10, result.ProblemDetails.Extensions["reference"]);
    }

    [Fact]
    public void ApiResult_ReturnsProblem_ForConflict()
    {
        var result = Assert.IsType<ProblemHttpResult>(Process.ApiResult(Result.Fail(new ConflictError("widget", 10, 11))));

        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal("widget", result.ProblemDetails.Extensions["resource"]);
        Assert.Equal(10, result.ProblemDetails.Extensions["reference"]);
        Assert.Equal(11, result.ProblemDetails.Extensions["conflictingReference"]);
    }

    [Fact]
    public void ApiResult_ReturnsProblem_ForValidation()
    {
        var result = Assert.IsType<ProblemHttpResult>(Process.ApiResult(Result.Fail(new ValidationError(["name is required"]))));

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("validation_failed", result.ProblemDetails.Title);
        Assert.Equal(["name is required"], Assert.IsType<string[]>(result.ProblemDetails.Extensions["messages"]));
    }

    [Fact]
    public void ApiResult_ReturnsProblem_ForUnauthorized_WithAction()
    {
        var result = Assert.IsType<ProblemHttpResult>(Process.ApiResult(Result.Fail(new UnauthorizedError("delete-widget"))));

        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        Assert.Equal("delete-widget", result.ProblemDetails.Extensions["action"]);
    }

    [Fact]
    public void ApiResult_ReturnsProblem_ForUnauthorized_WithoutActionExtension()
    {
        var result = Assert.IsType<ProblemHttpResult>(Process.ApiResult(Result.Fail(new UnauthorizedError())));

        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        Assert.DoesNotContain("action", result.ProblemDetails.Extensions.Keys);
    }

    [Fact]
    public void ApiResult_ReturnsProblem_ForBaseError_WithoutExtensions()
    {
        var result = Assert.IsType<ProblemHttpResult>(Process.ApiResult(Result.Fail(new Error("custom", "failure"))));

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("custom", result.ProblemDetails.Title);
        Assert.Empty(result.ProblemDetails.Extensions);
    }

    [Fact]
    public void ApiResult_ReturnsBadRequest_ForFailureWithoutError()
    {
        var result = Process.ApiResult(new TestResult(false, null));

        Assert.IsType<BadRequest>(result);
    }

    [Fact]
    public void ApiResultOfT_ReturnsOkOfT_ForSuccessfulValue()
    {
        var result = Assert.IsType<Ok<string>>(Process.ApiResult(Result.OkResult("value")));

        Assert.Equal("value", result.Value);
    }

    [Fact]
    public void ApiResultOfT_FallsBackToNonGenericHandler()
    {
        var result = Assert.IsType<Ok>(Process.ApiResult(Result.OkResult<string>(null)));

        Assert.NotNull(result);
    }

    [Fact]
    public void ApiResultOfT_FallsBackToNonGenericHandler_ForFailure()
    {
        var result = Assert.IsType<ProblemHttpResult>(Process.ApiResult(Result.FailResult<string>(new Error("custom", "failure"))));

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("custom", result.ProblemDetails.Title);
    }

    [Fact]
    public void MvcResult_ReturnsOk_ForSuccessfulResult()
    {
        var result = Process.MvcResult(Result.Ok());

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public void MvcResult_ReturnsProblemObject_ForNotFound()
    {
        var result = Assert.IsType<ObjectResult>(Process.MvcResult(Result.Fail(new NotFoundError("widget", 10))));
        var details = Assert.IsType<ProblemDetails>(result.Value);

        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.Equal("not_found", details.Title);
        Assert.Equal("widget", details.Extensions["resource"]);
        Assert.Equal(10, details.Extensions["reference"]);
    }

    [Fact]
    public void MvcResult_ReturnsProblemObject_ForConflict()
    {
        var result = Assert.IsType<ObjectResult>(Process.MvcResult(Result.Fail(new ConflictError("widget", 10, 11))));
        var details = Assert.IsType<ProblemDetails>(result.Value);

        Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        Assert.Equal(11, details.Extensions["conflictingReference"]);
    }

    [Fact]
    public void MvcResult_ReturnsProblemObject_ForValidation()
    {
        var result = Assert.IsType<ObjectResult>(Process.MvcResult(Result.Fail(new ValidationError(["name is required"]))));
        var details = Assert.IsType<ProblemDetails>(result.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal(["name is required"], Assert.IsType<string[]>(details.Extensions["messages"]));
    }

    [Fact]
    public void MvcResult_ReturnsProblemObject_ForUnauthorized_WithAction()
    {
        var result = Assert.IsType<ObjectResult>(Process.MvcResult(Result.Fail(new UnauthorizedError("delete-widget"))));
        var details = Assert.IsType<ProblemDetails>(result.Value);

        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        Assert.Equal("delete-widget", details.Extensions["action"]);
    }

    [Fact]
    public void MvcResult_ReturnsProblemObject_ForUnauthorized_WithoutActionExtension()
    {
        var result = Assert.IsType<ObjectResult>(Process.MvcResult(Result.Fail(new UnauthorizedError())));
        var details = Assert.IsType<ProblemDetails>(result.Value);

        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
        Assert.DoesNotContain("action", details.Extensions.Keys);
    }

    [Fact]
    public void MvcResult_ReturnsProblemObject_ForBaseError_WithoutExtensions()
    {
        var result = Assert.IsType<ObjectResult>(Process.MvcResult(Result.Fail(new Error("custom", "failure"))));
        var details = Assert.IsType<ProblemDetails>(result.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.Equal("custom", details.Title);
        Assert.Empty(details.Extensions);
    }

    [Fact]
    public void MvcResult_ReturnsBadRequestObject_ForFailureWithoutError()
    {
        var result = Assert.IsType<BadRequestObjectResult>(Process.MvcResult(new TestResult(false, null)));
        var details = Assert.IsType<ProblemDetails>(result.Value);

        Assert.Equal(StatusCodes.Status400BadRequest, details.Status);
        Assert.Equal("bad_request", details.Title);
    }

    [Fact]
    public void MvcResultOfT_ReturnsOkObject_ForSuccessfulValue()
    {
        var actionResult = Process.MvcResult(Result.OkResult("value"));

        var result = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.Equal("value", result.Value);
    }

    [Fact]
    public void MvcResultOfT_FallsBackToNonGenericHandler()
    {
        var actionResult = Process.MvcResult(Result.OkResult<string>(null));

        Assert.IsType<OkResult>(actionResult.Result);
    }

    private sealed class TestResult : Result
    {
        public TestResult(bool success, Error? error)
            : base(success, error)
        {
        }
    }
}
