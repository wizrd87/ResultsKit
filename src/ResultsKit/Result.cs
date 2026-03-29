using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ResultsKit;

/// <summary>
/// Represents the types of results.
/// </summary>
internal static class ResultType
{
    public static string NOT_FOUND_ERROR = "not_found";
    public static string VALIDATION_ERROR = "validation_failed";
    public static string UNAUTHORIZED_ERROR = "unauthorized";
    public static string CONFLICT_ERROR = "conflict";
}

/// <summary>
/// Represents the result of an operation, indicating success or failure.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="success">A value indicating whether the operation was successful.</param>
    /// <param name="error">The error associated with the operation, if any.</param>
    [JsonConstructor]
    protected Result(bool success, Error? error)
    {
        Success = success;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// Gets the error associated with the operation, if any.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// Gets a value indicating whether the operation is a failure.
    /// </summary>
    public bool IsFailure => !Success;

    /// <summary>
    /// Creates a failure result with the specified error.
    /// </summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A failure result.</returns>
    public static Result Fail(Error error)
    {
        return new Result(false, error);
    }

    /// <summary>
    /// Creates a failure result with the specified error and value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A failure result.</returns>
    public static Result<T> FailResult<T>(Error error)
    {
        return new Result<T>(default, false, error);
    }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    /// <returns>A success result.</returns>
    public static Result Ok()
    {
        return new Result(true, null);
    }

    /// <summary>
    /// Creates a success result with the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value associated with the success.</param>
    /// <returns>A success result.</returns>
    public static Result<T> OkResult<T>(T? value)
    {
        if (value is null)
        {
            return new Result<T>(default, true, null);
        }

        return new Result<T>(value, true, null);
    }

    /// <summary>
    /// Creates a partial success result with the specified value and error.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value associated with the success.</param>
    /// <param name="error">The error associated with the partial success.</param>
    /// <returns>A partial success result.</returns>
    public static Result<T> PartialResult<T>(T value, Error error)
    {
        return new Result<T>(value, true, error);
    }

    /// <summary>
    /// Creates a partial success result with the specified error.
    /// </summary>
    /// <param name="error">The error associated with the partial success.</param>
    /// <returns>A partial success result.</returns>
    public static Result Partial(Error error)
    {
        return new Result(true, error);
    }

    public static implicit operator Result(Error error) => Fail(error);
}

/// <summary>
/// Represents the result of an operation with a value, indicating success or failure.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class Result<T> : Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result{T}"/> class.
    /// </summary>
    /// <param name="value">The value associated with the result.</param>
    /// <param name="success">A value indicating whether the operation was successful.</param>
    /// <param name="error">The error associated with the operation, if any.</param>
    [JsonConstructor]
    protected internal Result(T? value, bool success, Error? error)
        : base(success, error)
    {
        Value = value;
    }

    /// <summary>
    /// Gets or sets the value associated with the result.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Implicitly converts an <see cref="Error"/> to a <see cref="Result{T}"/> with failure.
    /// </summary>
    /// <param name="error">The error associated with the failure.</param>
    /// <returns>A failure result.</returns>
    public static implicit operator Result<T>(Error error) => FailResult<T>(error);

    /// <summary>
    /// Implicitly converts a value to a <see cref="Result{T}"/> with success.
    /// </summary>
    /// <param name="value">The value associated with the success.</param>
    /// <returns>A success result.</returns>
    public static implicit operator Result<T>(T value) => OkResult(value);
}
