namespace ResultsKit;

/// <summary>
/// Represents a not found error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NotFoundError"/> class.
/// </remarks>
/// <param name="resource">The resource associated with the error.</param>
/// <param name="reference">The reference associated with the missing resource.</param>
public class NotFoundError(string resource, object? reference = null)
    : Error(ResultType.NOT_FOUND_ERROR, "The requested resource was not found.")
{
    /// <summary>
    /// Gets the resource associated with the error.
    /// </summary>
    public string Resource { get; } = resource;

    /// <summary>
    /// Gets the reference associated with the missing resource.
    /// </summary>
    public object? Reference { get; } = reference;
}
