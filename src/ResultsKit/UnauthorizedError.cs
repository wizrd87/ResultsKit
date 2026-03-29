namespace ResultsKit;

/// <summary>
/// Represents an unauthorized error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UnauthorizedError"/> class.
/// </remarks>
/// <param name="action">The action associated with the unauthorized request.</param>
public class UnauthorizedError(string? action = null)
    : Error(ResultType.UNAUTHORIZED_ERROR, "The request is not authorized.")
{
    public string? Action { get; } = action;
}
