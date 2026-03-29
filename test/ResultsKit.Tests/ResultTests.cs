using System.Text.Json;
using Xunit;

namespace ResultsKit.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Error_StoresCodeAndMessage()
    {
        var error = new Error("code", "message");

        Assert.Equal("code", error.Code);
        Assert.Equal("message", error.Message);

        error.Code = "updated-code";
        error.Message = "updated-message";

        Assert.Equal("updated-code", error.Code);
        Assert.Equal("updated-message", error.Message);
    }

    [Fact]
    public void DerivedErrors_ExposeGenericPublicContract()
    {
        var notFound = new NotFoundError("widget", 42);
        var conflict = new ConflictError("widget", 42, 99);
        var unauthorized = new UnauthorizedError("write-widget");
        var validation = new ValidationError(["name is required"]);

        Assert.Equal("not_found", notFound.Code);
        Assert.Equal("The requested resource was not found.", notFound.Message);
        Assert.Equal("widget", notFound.Resource);
        Assert.Equal(42, notFound.Reference);

        Assert.Equal("conflict", conflict.Code);
        Assert.Equal("The request conflicts with an existing resource.", conflict.Message);
        Assert.Equal("widget", conflict.Resource);
        Assert.Equal(42, conflict.Reference);
        Assert.Equal(99, conflict.ConflictingReference);

        Assert.Equal("unauthorized", unauthorized.Code);
        Assert.Equal("The request is not authorized.", unauthorized.Message);
        Assert.Equal("write-widget", unauthorized.Action);

        Assert.Equal("validation_failed", validation.Code);
        Assert.Equal("One or more validation errors occurred.", validation.Message);
        Assert.Equal(["name is required"], validation.Messages);
    }

    [Fact]
    public void FactoryMethods_CreateExpectedNonGenericResults()
    {
        var error = new Error("failure", "boom");

        var failed = Result.Fail(error);
        var success = Result.Ok();
        var partial = Result.Partial(error);
        Result implicitFailed = error;

        Assert.True(failed.IsFailure);
        Assert.False(failed.Success);
        Assert.Same(error, failed.Error);

        Assert.True(success.Success);
        Assert.False(success.IsFailure);
        Assert.Null(success.Error);

        Assert.True(partial.Success);
        Assert.False(partial.IsFailure);
        Assert.Same(error, partial.Error);

        Assert.False(implicitFailed.Success);
        Assert.Same(error, implicitFailed.Error);
    }

    [Fact]
    public void FactoryMethods_CreateExpectedGenericResults()
    {
        var error = new Error("failure", "boom");

        var failed = Result.FailResult<string>(error);
        var success = Result.OkResult("value");
        var successWithNull = Result.OkResult<string>(null);
        var partial = Result.PartialResult("value", error);
        Result<string> implicitFromError = error;
        Result<string> implicitFromValue = "value";

        Assert.False(failed.Success);
        Assert.Null(failed.Value);
        Assert.Same(error, failed.Error);

        Assert.True(success.Success);
        Assert.Equal("value", success.Value);
        Assert.Null(success.Error);

        Assert.True(successWithNull.Success);
        Assert.Null(successWithNull.Value);
        Assert.Null(successWithNull.Error);

        Assert.True(partial.Success);
        Assert.Equal("value", partial.Value);
        Assert.Same(error, partial.Error);

        Assert.False(implicitFromError.Success);
        Assert.Same(error, implicitFromError.Error);

        Assert.True(implicitFromValue.Success);
        Assert.Equal("value", implicitFromValue.Value);
    }

    [Fact]
    public void ProtectedConstructors_AreReachableForSerializationShapes()
    {
        var baseResult = new TestResult(false, null);
        var genericResult = new TestResult<string>("value", true, null);

        Assert.True(baseResult.IsFailure);
        Assert.Null(baseResult.Error);

        Assert.True(genericResult.Success);
        Assert.Equal("value", genericResult.Value);
        Assert.Null(genericResult.Error);
    }

    [Fact]
    public void Results_CanBeSerializedAndDeserialized()
    {
        var result = Result.PartialResult("value", new ValidationError(["missing field"]));
        var json = JsonSerializer.Serialize(result);

        var deserialized = JsonSerializer.Deserialize<TestResult<string>>(json);

        Assert.NotNull(deserialized);
        Assert.True(deserialized.Success);
        Assert.Equal("value", deserialized.Value);
        var error = Assert.IsType<Error>(deserialized.Error);
        Assert.Equal("validation_failed", error.Code);
        Assert.Equal("One or more validation errors occurred.", error.Message);
    }

    private sealed class TestResult : Result
    {
        public TestResult(bool success, Error? error)
            : base(success, error)
        {
        }
    }

    private sealed class TestResult<T> : Result<T>
    {
        public TestResult(T? value, bool success, Error? error)
            : base(value, success, error)
        {
        }
    }
}
