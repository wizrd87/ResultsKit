namespace ResultsKit;

/// <summary>
/// Represents a conflict error.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConflictError"/> class.
/// </remarks>
/// <param name="resource">The resource associated with the conflict.</param>
/// <param name="reference">The reference associated with the attempted record.</param>
/// <param name="conflictingReference">The reference associated with the conflicting record.</param>
public class ConflictError(string resource, object? reference = null, object? conflictingReference = null)
    : Error(ResultType.CONFLICT_ERROR, "The request conflicts with an existing resource.")
{
    /// <summary>
    /// Gets the resource associated with the conflict.
    /// </summary>
    public string Resource { get; } = resource;

    /// <summary>
    /// Gets the reference associated with the attempted record.
    /// </summary>
    public object? Reference { get; } = reference;

    /// <summary>
    /// Gets the reference associated with the conflicting record.
    /// </summary>
    public object? ConflictingReference { get; } = conflictingReference;
}
